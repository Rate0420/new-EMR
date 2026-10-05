using UnityEngine;
using EMR.Medal;

[CreateAssetMenu(menuName = "ItemEffects/FrictionReduction")]
public class Buffitem_FrictionReduction : ItemEffect
{
    [SerializeField] private float[] frictionReduction;

    public override void OnInventoryChanged(ItemEffectContext context, int itemLevel)
    {
        if (itemLevel < 0 || itemLevel >= frictionReduction.Length)
        {
            MedalBuffData.FrictionReduction = 0f;

            Debug.Log("スポーツ店の靴：摩擦低下率 0%");
            return;
        }

        MedalBuffData.FrictionReduction =
            Mathf.Clamp01(frictionReduction[itemLevel]);

        Debug.Log(
            $"スポーツ店の靴：摩擦低下率 " +
            $"{MedalBuffData.FrictionReduction:P0}"
        );
    }
}