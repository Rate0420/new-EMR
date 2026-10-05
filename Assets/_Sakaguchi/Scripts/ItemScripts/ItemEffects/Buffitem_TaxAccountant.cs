using EMR.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TaxAccountant")]
public class Buffitem_TaxAccountant : ItemEffect
{
    // ラウンドごとの返済額（RoundChangeCostと同じ値を入れる）
    [SerializeField] int[] roundCost;

    // 無凸～3凸の割引率
    [SerializeField] float[] discountRate = { 0.05f, 0.15f, 0.20f, 0.30f };

    public override void OnRoundStart(ItemEffectContext context, int itemlevel)
    {
        // OnRoundStart は返済後なので、1つ前のラウンドの返済額を見る
        int prevRound = GameState.Instance.RoundManager.CurrentRound - 1;

        if (prevRound < 0 || prevRound >= roundCost.Length)
            return;

        int refund = Mathf.RoundToInt(roundCost[prevRound] * discountRate[itemlevel]);

        GameState.Instance.OwnedModel.AddMedal(refund);

        Debug.Log($"税理士の連絡先：{refund}枚返金！");
    }
}