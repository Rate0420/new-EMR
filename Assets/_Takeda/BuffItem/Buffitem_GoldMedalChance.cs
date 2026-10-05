using UnityEngine;
using EMR.Medal;
[CreateAssetMenu(menuName = "ItemEffects/GoldMedalChance")]
public class Buffitem_GoldMedalChance : ItemEffect
{
    [SerializeField] private float[] goldMedalChance;

    public override void OnInventoryChanged(ItemEffectContext context, int itemLevel)
    {
        if (itemLevel < 0 || itemLevel >= goldMedalChance.Length)
        {
            MedalBuffData.GoldMedalChance = 0f;
            return;
        }

        MedalBuffData.GoldMedalChance = Mathf.Clamp01(goldMedalChance[itemLevel]);

        Debug.Log($"ã‡ÉÅÉ_Éãämó¶ {MedalBuffData.GoldMedalChance:P0}");
    }
}