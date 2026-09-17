using UnityEngine;

public class Checker : MonoBehaviour
{
    // このスクリプトがアタッチされたオブジェクトにMedalタグが付いているオブジェクトが接触した場合、そのオブジェクトを削除し、外部の関数を呼びスロットの保留を1貯める


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Debug.Log("チャッカー＋１");
            Destroy(other.gameObject);
            // 保留追加処理
            // SlotManager.Instance.AddHold();
        }
    }
}
