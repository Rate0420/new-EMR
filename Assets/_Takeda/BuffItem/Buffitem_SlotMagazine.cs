using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/SlotMagazine")]
public class Buffitem_SlotMagazine : ItemEffect
{
    // 5%上がるまでに必要な回転数（無凸～3凸）
    [SerializeField] int[] needRollCount = { 10, 8, 6, 4 };

    [SerializeField] float increaseRate = 0.05f;
    [SerializeField] float maxBonus = 0.25f;

    int rollCount = 0;
    float currentBonus = 0f;

    // スロットを回すたびに呼ばれる
    public override void OnSlotRoll(ItemEffectContext context, int itemlevel)
    {
        rollCount++;

        if (rollCount >= needRollCount[itemlevel])
        {
            rollCount = 0;

            currentBonus = Mathf.Min(currentBonus + increaseRate, maxBonus);

            context.slotManager.winProbability =
                SlotManager.baseWinProbability + currentBonus;

            Debug.Log($"スロット雑誌 発動！ 当選率 {context.slotManager.winProbability:P0}");
        }
    }

    // 当たったらリセット
    public override void OnSlotWin(ItemEffectContext context, int itemlevel)
    {
        rollCount = 0;
        currentBonus = 0f;

        context.slotManager.winProbability = SlotManager.baseWinProbability;

        Debug.Log("スロット雑誌 リセット");
    }

    // ラウンド開始でもリセット
    public override void OnRoundStart(ItemEffectContext context, int itemlevel)
    {
        rollCount = 0;
        currentBonus = 0f;

        context.slotManager.winProbability = SlotManager.baseWinProbability;
    }
}