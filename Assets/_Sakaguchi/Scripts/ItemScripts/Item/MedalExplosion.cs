using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MedalExprosion")]
public class MedalExplosion : ItemEffect
{
    public float explosionForce = 1000f; // 爆発の威力
    public float explosionRadius = 5f;   // 爆発の半径
    public float upwardsModifier = 2f;   // 上方向への補正値

    public override void OnMedalLanded(ItemEffectContext context, GameObject medal,int itemlevel)
    {
        // 5%の確率で爆発する処理をここに実装
        float chance = Random.Range(0f, 1f);
        Debug.Log(chance);
        if (chance <= 0.9f)
        {
            Debug.Log("爆発");
            context.StartMedalExplosion(medal);
        }
    }
}
