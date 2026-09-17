using UnityEngine;

public class ReachEffectTrigger : MonoBehaviour
{
    [SerializeField] ReelManager reelManager;

    // 当たりかどうかと図柄はSlotManagerから受け取る
    int winIndex;
    bool isWin;

    public void Setup(ReelManager reelManager, int winIndex, bool isWin)
    {
        this.reelManager = reelManager;
        this.winIndex = winIndex;
        this.isWin = isWin;
    }

    // SlotManagerから呼んで事前にセットしておく
    public void Setup(int winIndex, bool isWin)
    {
        this.winIndex = winIndex;
        this.isWin = isWin;
    }

    // ① アニメーションイベント：ガコガコ開始
    public void OnGacoGacoStart()
    {
        StartCoroutine(reelManager.PlayGacoGaco(winIndex));
    }

    // ② アニメーションイベント：当たり確定
    public void OnConfirmWin()
    {
        reelManager.centerReel.ConfirmWin(winIndex);
    }

    // ③ アニメーションイベント：外れ確定
    public void OnConfirmLose()
    {
        reelManager.centerReel.ConfirmLose(winIndex);
    }


    // アニメーションイベントから呼ぶ
    public void OnReelRestart()
    {
        reelManager.NotifyReelRestart();
        reelManager.NotifyReelStopReset();
    }

    public void OnReelStop()
    {
        reelManager.NotifyReelStop();
    }

    public void OnReachEffectEnd()
    {
        reelManager.NotifyReachEffectEnd();
    }
}