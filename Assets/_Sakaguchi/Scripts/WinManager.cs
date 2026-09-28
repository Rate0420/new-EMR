using EMR.Core;
using EMR.Medal.Refund;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 当たり演出とメダル払い出しを管理する。
///
/// ・演出(動画)は当たった瞬間に再生する。スロット側は PlayWin を yield return することで
///   「演出が終わるまで次の変動を待つ」ことができる。
/// ・払い出し(メダル排出)は待たない。払い出し中に新しい当たり(通常/JPC/JPCC)が来たら、
///   別の払い出しを始めず、今出している分にそのまま枚数を加算する。
/// </summary>
public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshPro payoutText;
    [SerializeField] VideoEffectPlayer videoPlayer;

    [SerializeField] MedalRefundBehaviour refundBehaviour;   // 通常当たり用
    [SerializeField] MedalRefundBehaviour refundBehaviour2;  // JPC用
    [SerializeField] PriseGenerator priseGenerator;

    [SerializeField] BallTest jpcBallTest;

    // メダルを排出している最中(払い出しセッション中)かどうか
    public bool isPayout = false;

    // 進行中の当たり処理(演出中 + 払い出し完了待ち)の数
    int activeWorkCount = 0;

    /// <summary>演出中または払い出し中の当たりが1件でもあるか。JPCPayoutManager等から参照される。</summary>
    public bool IsWinProcessing => activeWorkCount > 0;

    // 払い出しが止まったまま進まない場合に諦めるまでの秒数
    const float StallTimeout = 10f;

    public void SetIsPayout(bool value) => isPayout = value;

    // ---------------- 演出 ----------------

    // 演出用のvideoPlayerは1つしかないので、同時に複数再生しないよう直列化する
    bool isEffectPlaying = false;

    IEnumerator PlayEffect()
    {
        while (isEffectPlaying) yield return null;

        isEffectPlaying = true;
        try
        {
            yield return videoPlayer.PlayVideoNoFadeCoroutine(0, 1.0f);
        }
        finally
        {
            isEffectPlaying = false;
        }
    }

    // ---------------- 外部から呼ぶ入口 ----------------

    /// <summary>
    /// 通常当たり(スロット本体の結果)。演出が終わるまで待つ。払い出しの完了は待たない。
    /// SlotManagerから yield return StartCoroutine(winManager.PlayWin(...)) で呼ぶ。
    /// </summary>
    public IEnumerator PlayWin(int resultNumber)
    {
        yield return StartCoroutine(HandleWin(GetPayout(resultNumber), false));
    }

    /// <summary>
    /// JPC/JPCCの払い出し。呼びっぱなしでよい(演出は即座に始まる)。
    /// </summary>
    public void EnqueueJPC(int payoutNum)
    {
        StartCoroutine(HandleWin(payoutNum, true));
    }

    // 演出を再生し、終わったら払い出しに枚数を加算する(払い出しの完了は待たずに戻る)
    IEnumerator HandleWin(int payout, bool isJPC)
    {
        string lockName = isJPC ? "JPC" : "Win";

        activeWorkCount++;

        // SubMonitor扱い：リールの保留消化とは並行、メニュー・シナリオ等の開始だけをブロックする。
        // この当たりの払い出し分を出し切るまで保持し続ける。
        yield return GameState.Instance.GameLock.Acquire(lockName, GameLockKind.SubMonitor);

        yield return StartCoroutine(PlayEffect());

        // メダル払い出しの前にボールと景品を排出する。JPC払い出しの場合は、ボールと景品は排出しない。
        if (!isJPC)
        {
            priseGenerator.DisChargeBall();
            priseGenerator.PriseLottely();
        }

        AddPayout(payout, isJPC, () =>
        {
            GameState.Instance.GameLock.Release(lockName);
            activeWorkCount--;
        });
    }

    // ---------------- 払い出し ----------------

    struct Milestone
    {
        public int endPoint;              // このセッションで通算何枚目まで出せばこの当たり分が完了か
        public System.Action onDispensed; // 完了時に呼ぶ処理
    }

    readonly List<Milestone> milestones = new List<Milestone>();
    int sessionTotal = 0;   // このセッションで加算された合計枚数

    void AddPayout(int amount, bool isJPC, System.Action onDispensed)
    {
        if (isPayout)
        {
            // 払い出し中：別の払い出しは始めず、今出している分にそのまま加算する
            sessionTotal += amount;
            milestones.Add(new Milestone { endPoint = sessionTotal, onDispensed = onDispensed });

            var notifier = GameState.Instance.RefundNotifier;
            notifier.RequestRefund(amount);
            payoutText.text = $"{notifier.RefundAmount}枚";

            Debug.Log($"[Win] 払い出し中のため加算 +{amount}枚 (通算:{sessionTotal}枚)");
        }
        else
        {
            StartCoroutine(PayoutSession(amount, isJPC, onDispensed));
        }
    }

    IEnumerator PayoutSession(int payout, bool isJPC, System.Action onDispensed)
    {
        // ※ここから最初のyieldまでは同期実行されるので、
        //   直後に来た追加分は必ず「加算」側に回る
        isPayout = true;
        sessionTotal = payout;
        milestones.Clear();
        milestones.Add(new Milestone { endPoint = sessionTotal, onDispensed = onDispensed });

        MedalRefundBehaviour target = isJPC ? refundBehaviour2 : refundBehaviour;

        if (refundBehaviour == null || refundBehaviour2 == null || target == null)
        {
            Debug.LogError($"{nameof(WinManager)}: refundBehaviour が未設定です");
            EndSession(null, null);
            yield break;
        }

        refundBehaviour.gameObject.SetActive(!isJPC);
        refundBehaviour2.gameObject.SetActive(isJPC);

        // 切り替え直後のOnEnable/Startでイベント購読が完了するのを1フレーム待つ
        yield return null;

        var notifier = GameState.Instance.RefundNotifier;

        winUI.SetActive(true);

        System.Action<int> spawnHandler = OnMedalSpawned;
        target.OnMedalSpawned += spawnHandler;

        notifier.RequestRefund(payout);
        payoutText.text = $"{notifier.RefundAmount}枚";

        // 「残り枚数が0」かつ「排出ループも終了」するまで待つ。
        // その間に加算された分は、動いている排出ループがそのまま拾う。
        int lastRemaining = notifier.RefundAmount;
        float stall = 0f;

        while (notifier.RefundAmount > 0 || target.IsProcessing)
        {
            if (notifier.RefundAmount != lastRemaining)
            {
                lastRemaining = notifier.RefundAmount;
                stall = 0f;
            }
            else
            {
                stall += Time.deltaTime;
            }

            if (stall >= StallTimeout)
            {
                Debug.LogWarning($"{nameof(WinManager)}: 払い出しが{StallTimeout:0}秒間進まなかったため打ち切ります");
                break;
            }

            yield return null;
        }

        EndSession(target, spawnHandler);
    }

    // メダルが1枚出るたびに呼ばれる(remaining = 残り枚数)
    void OnMedalSpawned(int remaining)
    {
        payoutText.text = $"{remaining}枚";

        // 通算で何枚出したか。自分の分を出し切った当たりから順に完了扱いにする
        int dispensed = sessionTotal - remaining;

        while (milestones.Count > 0 && milestones[0].endPoint <= dispensed)
        {
            Milestone m = milestones[0];
            milestones.RemoveAt(0);
            m.onDispensed?.Invoke();
        }
    }

    void EndSession(MedalRefundBehaviour target, System.Action<int> spawnHandler)
    {
        if (target != null && spawnHandler != null)
        {
            target.OnMedalSpawned -= spawnHandler;
        }

        winUI.SetActive(false);

        // 先にセッション状態を片付けてから完了処理を呼ぶ
        // (完了処理の中から新しい払い出しが来ても、新しいセッションとして正しく扱えるように)
        isPayout = false;
        sessionTotal = 0;

        var remainingMilestones = new List<Milestone>(milestones);
        milestones.Clear();

        foreach (var m in remainingMilestones)
        {
            m.onDispensed?.Invoke();
        }
    }

    int GetPayout(int number)
    {
        if (number == 7)
        {
            jpcBallTest.StartJPCC();
            return 0;
        }
        if (number % 2 == 0) return 30;
        else return 50;
    }
}
