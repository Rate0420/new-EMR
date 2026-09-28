using EMR.Core;
using EMR.Medal.Refund;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshPro payoutText;
    [SerializeField] VideoEffectPlayer videoPlayer;

    [SerializeField] MedalRefundBehaviour refundBehaviour;
    [SerializeField] MedalRefundBehaviour refundBehaviour2;
    [SerializeField] PriseGenerator priseGenerator;

    public bool isPayout = false;

    struct WinRequest
    {
        public int payout;
        public bool isJPC;
    }

    // 通常当たり(スロット本体)とJPC(玉入れ)を共通で扱うキュー。
    // 両方が同時に走ると winUI / payoutText / refundBehaviour を取り合ってしまうため、
    // 必ずここで1件ずつ直列に処理する。
    readonly Queue<WinRequest> winQueue = new Queue<WinRequest>();
    bool isProcessing = false;

    /// <summary>
    /// 通常当たり・JPCのどちらかを処理中かどうか。
    /// JPCPayoutManager や BallEventQueue から参照される。
    /// </summary>
    public bool IsWinProcessing => isProcessing;

    // どちらのrefundBehaviourを使うかのフラグ（SetActiveを使わない）
    bool useJPCRefund = false;

    public void SetIsPayout(bool value) => isPayout = value;

    /// <summary>
    /// 通常当たり(スロット本体の結果)をキューに積む。呼びっぱなしでよく、
    /// リールの保留消化を止めずに済む(以前はここをyield returnで待っていたため止まっていた)。
    /// </summary>
    public void EnqueueWin(int resultNumber)
    {
        winQueue.Enqueue(new WinRequest { payout = GetPayout(resultNumber), isJPC = false });
        Debug.Log($"[Win] 通常当たりを予約 resultNumber:{resultNumber} キュー数:{winQueue.Count}");
        TryStartProcessing();
    }

    /// <summary>
    /// JPC(玉入れ)払い出しをキューに積む。呼びっぱなしでよい。
    /// </summary>
    public void EnqueueJPC(int payoutNum)
    {
        winQueue.Enqueue(new WinRequest { payout = payoutNum, isJPC = true });
        Debug.Log($"[Win] JPC払い出しを予約 payout:{payoutNum} キュー数:{winQueue.Count}");
        TryStartProcessing();
    }

    void TryStartProcessing()
    {
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (winQueue.Count > 0)
        {
            WinRequest req = winQueue.Dequeue();
            string lockName = req.isJPC ? "JPC" : "Win";

            // SubMonitor扱い：リールの保留消化とは並行に進める。
            // メニュー・シナリオ・ラウンドチェンジの開始だけはブロックされる。
            yield return GameState.Instance.GameLock.Acquire(lockName, GameLockKind.SubMonitor);

            useJPCRefund = req.isJPC;
            refundBehaviour.gameObject.SetActive(!req.isJPC);
            refundBehaviour2.gameObject.SetActive(req.isJPC);

            yield return StartCoroutine(PlayWinInternal(req.payout));

            if (!req.isJPC)
            {
                priseGenerator.DisChargeBall();
                priseGenerator.PriseLottely();
            }

            GameState.Instance.GameLock.Release(lockName);

            if (winQueue.Count > 0)
            {
                yield return new WaitForSeconds(1.0f);
            }
        }

        isProcessing = false;
    }

    IEnumerator PlayWinInternal(int payout)
    {
        isPayout = true;

        yield return videoPlayer.PlayVideoNoFadeCoroutine(0, 1.0f);

        winUI.SetActive(true);
        payoutText.text = $"{payout}枚";

        bool isFinished = false;
        System.Action<int> spawnHandler = (remaining) => { payoutText.text = $"{remaining}枚"; };
        System.Action finishHandler = () => { isFinished = true; };

        // useJPCRefundに応じて使うrefundBehaviourを切り替え
        MedalRefundBehaviour target = useJPCRefund ? refundBehaviour2 : refundBehaviour;

        if (target == null)
        {
            Debug.LogError($"{nameof(WinManager)}: refundBehaviour が未設定です");
            winUI.SetActive(false);
            isPayout = false;
            yield break;
        }

        target.OnMedalSpawned += spawnHandler;
        target.OnRefundFinished += finishHandler;

        GameState.Instance.RefundNotifier.RequestRefund(payout);

        float timeout = Mathf.Max(10f, payout * 0.5f + 5f);
        float elapsed = 0f;
        while (!isFinished && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.OnMedalSpawned -= spawnHandler;
        target.OnRefundFinished -= finishHandler;

        if (!isFinished)
        {
            Debug.LogWarning($"{nameof(WinManager)}: Refund did not finish within {timeout:0.0}s");
        }

        winUI.SetActive(false);
        isPayout = false;
    }

    int GetPayout(int number)
    {
        if (number == 7) return 100;
        if (number % 2 == 0) return 30;
        else return 50;
    }
}
