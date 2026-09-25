using UnityEngine;

public class JPCPayoutManager : MonoBehaviour
{
    public WinManager winManager;

    // WinManager側で通常当たりと共通のキューとして処理されるため、
    // ここでは独自のキュー/ロックは持たず、そのまま委譲する。
    // (以前はここにも別のキューがあり、通常当たりの演出と別々に動けてしまっていたため、
    //  winUI等の取り合いが起きる余地があった)

    /// <summary>SceneChanger/BallEventQueue等から参照できるようにプロパティ化</summary>
    public bool IsJPCProcessing => winManager.IsWinProcessing;

    public void Payout(int payoutCount)
    {
        Debug.Log($"[JPC] 払い出しを予約 payoutCount:{payoutCount}");
        winManager.EnqueueJPC(payoutCount);
    }
}