using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/FlashyHat")]
public class Buffitem_FlashyHat : ItemEffect
{
    [SerializeField] float[] sideHoleSaveChance;

    public override void OnInventoryChanged(ItemEffectContext context, int itemLevel)
    {
       context.slotManager.sideHoleSaveChance = sideHoleSaveChance[itemLevel];

        Debug.Log($"”hè‚È–XqF‰¡ŒŠ‹~ÏŠm—¦ {sideHoleSaveChance[itemLevel] * 100}%");
    }
}