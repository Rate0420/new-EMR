using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/RegularCustomerCard")]
public class Buffitem_RegularCustomerCard : ItemEffect
{
    [SerializeField] float[] chanceWinProbability;

    public override void OnInventoryChanged(ItemEffectContext context, int itemlevel)
    {
        context.slotManager.chanceWinProbability = chanceWinProbability[itemlevel];

        Debug.Log($"常連客カード：確変確率 {chanceWinProbability[itemlevel]}");
    }
}