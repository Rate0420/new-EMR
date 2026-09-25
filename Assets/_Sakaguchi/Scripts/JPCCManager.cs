using UnityEngine;

public class JPCCManager : MonoBehaviour
{
    public JPCPayoutManager payoutManager;

    // ホールにボールが落ちたとき、ホール側スクリプトでこちらの関数を呼んで、引数に自分のインデックス番号を付けることで払い出しが行われる
    [SerializeField] int[] LowPrise;
    [SerializeField] int[] MiddlePrise;
    [SerializeField] int[] HighPrise;
    // 0=JPC,4=High,2&6=Middle,1&3&5&7=Low


    // JPCもこちらで管理する
    [SerializeField] int[] JPCLowPrise;
    [SerializeField] int[] JPCMiddlePrise;
    [SerializeField] int[] JPCHighPrise;

    public int JPCMaxPrise;

    public void JPCCPrise(int index)
    {
        switch (index)
        {
            case 0:
                // JPCスタート。
                break;
            case 4:
                // High
                // ← BallEventQueue経由をやめて直接JPCPayoutManagerへ依頼
                //   (ミニイベント/ボール発射と同じキューに並ぶと払い出しが後回しになるため)
                payoutManager.Payout(HighPrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
            case 2:
            case 6:
                // Middle
                payoutManager.Payout(MiddlePrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
            case 1:
            case 3:
            case 5:
            case 7:
                // Low
                payoutManager.Payout(LowPrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
        }
    }

    public void JPCPrise(int index)
    {
        switch (index)
        {
            case 0:
                payoutManager.Payout(JPCMaxPrise);
                break;
            case 3:
            case 7:
                // High
                // ← BallEventQueue経由をやめて直接JPCPayoutManagerへ依頼
                //   (ミニイベント/ボール発射と同じキューに並ぶと払い出しが後回しになるため)
                payoutManager.Payout(HighPrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
            case 2:
            case 5:
            case 8:
                // Middle
                payoutManager.Payout(MiddlePrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
            case 1:
            case 4:
            case 6:
            case 9:
                // Low
                payoutManager.Payout(LowPrise[0]); // 今は仮だがラウンド進行によって変わるようにする
                break;
        }
    }
}
