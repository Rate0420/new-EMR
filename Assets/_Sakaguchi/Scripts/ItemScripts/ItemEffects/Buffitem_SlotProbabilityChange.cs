using UnityEngine;
[CreateAssetMenu(menuName = "ItemEffects/SlotProbabilityChange")]
public class Buffitem_SlotProbabilityChange : ItemEffect
{
    [SerializeField] float[] changeWinProbability;
    [SerializeField] float[] chanceChanceWinProbility;


    public override void OnInventoryChanged(ItemEffectContext context,int itemlevel)
    {
        // itemlevelに応じて、スロットの当選確率を変更する
        context.slotManager.winProbability = changeWinProbability[itemlevel];
        context.slotManager.chanceWinProbability = chanceChanceWinProbility[itemlevel];
        Debug.Log($"スロットの当選確率を変更しました。通常: {changeWinProbability[itemlevel]}, チャンス: {chanceChanceWinProbility[itemlevel]}");
    }
}
