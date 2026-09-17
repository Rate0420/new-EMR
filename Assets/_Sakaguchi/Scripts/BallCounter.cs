using UnityEngine;

public class BallCounter : MonoBehaviour
{
    int ballCount = 0;
    public BallEventQueue ballEventQueue; // ← SceneChangerの代わりに
    public BallTest ballTest;

    public int jpcPayoutAmount = 50; // JPC払い出し枚数（Inspector設定）

    public int BallCount => ballCount;

    public void BallCountAdd()
    {
        ballCount++;
        BallCountCheck();
    }

    public void BallCountReset()
    {
        ballCount = 0;
    }

    void BallCountCheck()
    {
        // %2でミニイベント（キューに積む）
        if (ballCount % 2 == 0)
        {
            ballEventQueue.EnqueueMiniEvent();
        }

        // %5でJPC（キューに積む）
        if (ballCount % 5 == 0)
        {
            ballEventQueue.EnqueueBallSpawn();
        }
    }
}