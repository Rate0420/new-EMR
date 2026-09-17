using UnityEngine;

public class HoleAssist : MonoBehaviour
{
    /// <summary>
    /// ボールを穴に引き寄せる
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // 衝突オブジェクトのタグがボール以外なら実行しない
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        // ボールのRigidbodyを取得
        Rigidbody rb = other.attachedRigidbody;
        
        if (rb == null)
            return;

        // ボールを穴に引き寄せる
        Vector3 dir = transform.position - rb.position;
        rb.AddForce(dir.normalized * 4.5f, ForceMode.Acceleration);
        rb.linearVelocity *= 0.98f;
    }
}
