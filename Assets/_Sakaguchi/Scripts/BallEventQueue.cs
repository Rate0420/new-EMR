using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EMR.Core;

public class BallEventQueue : MonoBehaviour
{
    public enum EventType { MiniEvent, JPC, BallSpawn }

    [System.Serializable]
    public class BallEvent
    {
        public EventType type;
        public int value;
    }

    Queue<BallEvent> eventQueue = new Queue<BallEvent>();
    bool isProcessing = false;

    // ボールが転がっている（穴に落ちるまで）フラグ
    bool isBallActive = false;
    public bool IsBallActive => isBallActive;

    // 外部から参照できるようにプロパティ化
    public bool IsQueueProcessing => isProcessing;

    [SerializeField] JPCPayoutManager jpcPayoutManager;
    [SerializeField] SceneChanger sceneChanger;
    [SerializeField] BallTest ballTest;

    public void EnqueueMiniEvent()
    {
        eventQueue.Enqueue(new BallEvent { type = EventType.MiniEvent });
        Debug.Log($"[BallEventQueue] MiniEvent追加 キュー数:{eventQueue.Count}");
        TryStartProcessing();
    }

    public void EnqueueJPC(int payoutCount)
    {
        eventQueue.Enqueue(new BallEvent { type = EventType.JPC, value = payoutCount });
        Debug.Log($"[BallEventQueue] JPC追加 payout:{payoutCount} キュー数:{eventQueue.Count}");
        TryStartProcessing();
    }

    public void EnqueueBallSpawn()
    {
        eventQueue.Enqueue(new BallEvent { type = EventType.BallSpawn });
        Debug.Log($"[BallEventQueue] BallSpawn追加 キュー数:{eventQueue.Count}");
        TryStartProcessing();
    }

    // CroonHoleからボールが穴に落ちたときに呼ぶ
    public void NotifyBallEntered()
    {
        isBallActive = false;
        Debug.Log("[BallEventQueue] ボール穴に落ちた → isBallActive=false");
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

        while (eventQueue.Count > 0)
        {
            BallEvent e = eventQueue.Dequeue();
            Debug.Log($"[BallEventQueue] イベント処理開始 type:{e.type}");

            switch (e.type)
            {
                case EventType.MiniEvent:
                    // sceneChanger側で"Scenario"ロックを取ってから始まる
                    yield return StartCoroutine(WaitForMiniEvent());
                    break;

                case EventType.JPC:
                    // jpcPayoutManager側で"JPC"ロックを取ってから払い出しが始まる
                    jpcPayoutManager.Payout(e.value);
                    yield return new WaitUntil(() => !jpcPayoutManager.IsJPCProcessing);
                    break;

                case EventType.BallSpawn:
                    // SubMonitor扱い：リールの保留消化とは並行して進める。
                    // メニュー・シナリオ・ラウンドチェンジの開始だけはブロックされる。
                    yield return GameState.Instance.GameLock.Acquire("Ball", GameLockKind.SubMonitor);
                    isBallActive = true;
                    ballTest.StartJPCC();
                    Debug.Log("[BallEventQueue] ボール生成完了 isBallActive=true");
                    yield return new WaitUntil(() => !isBallActive);
                    Debug.Log("[BallEventQueue] ボール着地確認");
                    GameState.Instance.GameLock.Release("Ball");
                    break;
            }

            Debug.Log($"[BallEventQueue] イベント処理完了 残り:{eventQueue.Count}");
            yield return new WaitForSeconds(0.5f);
        }

        isProcessing = false;
    }
    IEnumerator WaitForMiniEvent()
    {
        sceneChanger.StartMiniEvent();
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => sceneChanger.IsSceneActive);
        Debug.Log("[BallEventQueue] ミニイベント開始確認");
        yield return new WaitUntil(() => !sceneChanger.IsSceneActive);
        Debug.Log("[BallEventQueue] ミニイベント終了");
    }
}