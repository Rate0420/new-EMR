using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPCPayoutManager : MonoBehaviour
{
    public ReserveManager reserveManager;
    public WinManager winManager;
    [SerializeField] SceneChanger sceneChanger;
    [SerializeField] RoundChange roundChange; // ← 追加：ラウンドチェンジ中かどうかを見るため

    Queue<int> jpcQueue = new Queue<int>();
    bool isProcessing = false;

    // SceneChangerから参照できるようにプロパティ化
    public bool IsJPCProcessing => isProcessing;

    public void Payout(int payoutCount)
    {
        jpcQueue.Enqueue(payoutCount);
        reserveManager.pauseRequested = true;
        Debug.Log($"[JPC] 予約追加 payoutCount:{payoutCount} キュー数:{jpcQueue.Count}");

        if (!isProcessing)
        {
            StartCoroutine(ProcessJPCQueue());
        }
    }

    IEnumerator ProcessJPCQueue()
    {
        isProcessing = true;

        while (jpcQueue.Count > 0)
        {
            int payoutCount = jpcQueue.Dequeue();
            Debug.Log($"[JPC] 払い出し開始 payoutCount:{payoutCount} 残りキュー:{jpcQueue.Count}");

            yield return StartCoroutine(PayoutCoroutine(payoutCount));

            Debug.Log($"[JPC] 払い出し完了 残りキュー:{jpcQueue.Count}");

            if (jpcQueue.Count > 0)
            {
                yield return new WaitForSeconds(1.0f);
            }
        }

        // 全JPC完了後にスロット再開
        reserveManager.pauseRequested = false;
        isProcessing = false;
    }

    IEnumerator PayoutCoroutine(int payoutCount)
    {
        reserveManager.pauseRequested = true;

        float logTimer = 0f;
        while (
            (reserveManager.isProcessing && !reserveManager.isBetweenReserves)
            || winManager.isPayout
            || sceneChanger.IsSceneActive // ← 追加：メニュー・シナリオ中は払い出しを始めない
            || roundChange.IsRoundChangeProcessing // ← 追加：ラウンドチェンジ中は払い出しを始めない
        )
        {
            logTimer += Time.deltaTime;
            if (logTimer >= 1f)
            {
                Debug.Log($"[JPC] 待機中 isProcessing:{reserveManager.isProcessing} isBetween:{reserveManager.isBetweenReserves} isPayout:{winManager.isPayout} isSceneActive:{sceneChanger.IsSceneActive} isRoundChange:{roundChange.IsRoundChangeProcessing}");
                logTimer = 0f;
            }
            yield return null;
        }

        Debug.Log("[JPC] 条件クリア、払い出し開始");
        yield return StartCoroutine(winManager.PlayWinbyJPC(payoutCount));
    }
}