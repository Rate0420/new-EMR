using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/RerollSubscription")]
public class Buffitem_RerollSubscription : ItemEffect
{
    [SerializeField] int[] freeReroll = { 1, 2, 3, 4 };

    public override void OnRoundStart(ItemEffectContext context, int itemlevel)
    {
        context.costShopManager.AddFreeReroll(freeReroll[itemlevel]);
        context.buffshopManager.AddFreeReroll(freeReroll[itemlevel]);
    }
}