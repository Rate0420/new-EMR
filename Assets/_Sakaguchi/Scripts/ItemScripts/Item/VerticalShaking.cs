using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/VerticalShaking")]
public class VerticalShaking : ItemEffect
{
    // 消費アイテム用トリガー
    public override void OnConsumptionItem(ItemEffectContext context, int itemlevel)
    {
        // 3秒間上下に揺らす処理
        context.StartVerticalShaking(3f);
    }
}
