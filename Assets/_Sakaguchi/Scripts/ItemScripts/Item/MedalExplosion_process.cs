using UnityEngine;

public class MedalExplosion_process : MonoBehaviour
{
    public MedalExplosion medalExplosion;

    public void BlowAway(GameObject medal)
    {
        // 爆発の中心点（ここではこのオブジェクトの現在位置）
        Vector3 explosionPos = medal.transform.position;

        // 一定の半径内にあるすべてのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(explosionPos, medalExplosion.explosionRadius);

        // 確認で爆破半径を可視化
        Debug.DrawLine(explosionPos, explosionPos + Vector3.up * medalExplosion.explosionRadius, Color.red,5f);
        Debug.DrawLine(explosionPos, explosionPos + Vector3.down * medalExplosion.explosionRadius, Color.red, 5f);
        Debug.DrawLine(explosionPos, explosionPos + Vector3.left * medalExplosion.explosionRadius, Color.red, 5f);
        Debug.DrawLine(explosionPos, explosionPos + Vector3.right * medalExplosion.explosionRadius, Color.red, 5f);
        Debug.DrawLine(explosionPos, explosionPos + Vector3.forward * medalExplosion.explosionRadius, Color.red, 5f);
        Debug.DrawLine(explosionPos, explosionPos + Vector3.back * medalExplosion.explosionRadius, Color.red, 5f);

        foreach (Collider hit in colliders)
        {
            // Rigidbodyを持つオブジェクトのみ吹き飛ばす
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(hit.name == "Medal") rb = hit.transform.parent.GetComponent<Rigidbody>();
            Debug.Log("爆発に巻き込まれたオブジェクト: " + hit.name);
            if (rb != null)
            {
                // 爆発力を適用
                Debug.Log("爆発力を適用: " + hit.name);
                rb.AddExplosionForce(medalExplosion.explosionForce, explosionPos, medalExplosion.explosionRadius, medalExplosion.upwardsModifier);
            }
        }
    }
}
