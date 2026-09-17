using UnityEngine;

public class Test : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("coin entered the trigger!");
            // コインを子オブジェクトにする
            other.transform.SetParent(transform, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("coin exited the trigger!");
            // コインを子オブジェクトから外す
            other.transform.SetParent(null, true);
        }
    }
}
