using UnityEngine;

// ItemEffect(各アイテムの効果ロジック)から呼び出す、
// プレイヤー側の実行環境をまとめたクラス。
// 処理が簡単ではないアイテムは直接処理を触らず、必ずここを経由する。
// 新しい処理を追加する場合は、ここに参照とメソッドを追加していく。
public class ItemEffectContext : MonoBehaviour
{
    [Header("消費アイテムの処理")]
    public VerticalShaking_process shakingProcess;
    public MedalExplosion_process medalExplosionProcess;

    public SlotManager slotManager;

    public void StartVerticalShaking(float duration)
    {
        shakingProcess.StartShake(duration);
    }

    public void StartMedalExplosion(GameObject medal)
    {
        medalExplosionProcess.BlowAway(medal);
    }
}
