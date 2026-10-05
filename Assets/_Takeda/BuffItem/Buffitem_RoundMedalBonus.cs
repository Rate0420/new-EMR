using UnityEngine;
using EMR.Medal;
[CreateAssetMenu(menuName = "ItemEffects/RoundMedalBonus")]
public class Buffitem_RoundMedalBonus : ItemEffect
{
    [SerializeField] private float[] roundMedalBonus;

    public override void OnInventoryChanged(ItemEffectContext context, int itemLevel)
    {
        if (itemLevel < 0 || itemLevel >= roundMedalBonus.Length)
        {
            MedalBuffData.RoundMedalBonus = 0f;
            return;
        }

        MedalBuffData.RoundMedalBonus =
            Mathf.Clamp01(roundMedalBonus[itemLevel]);

        Debug.Log($"ラウンドメダル付与率 {MedalBuffData.RoundMedalBonus:P0}");
    }
}