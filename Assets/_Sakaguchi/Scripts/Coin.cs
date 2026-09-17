using UnityEngine;

public class Coin : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }
}
