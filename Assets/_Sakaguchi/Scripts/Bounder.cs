using UnityEngine;

public class Bounder : MonoBehaviour
{
    // Ballタグを持つオブジェクトが衝突したときに、衝突したオブジェクトを跳ね返す
    // クルーンの中心部のオブジェクトにこのスクリプトをアタッチする
    // Vector3でクルーンの各穴の位置、boolで穴にボールが入っているかどうかを管理する
    // 跳ね返す方向は、空いている穴の中でランダムな穴に向かって跳ね返すようにする

    public GameObject[] HoleObjects; // クルーンの穴のオブジェクトを格納する配列
    Vector3[] holePositions; // クルーンの穴の位置を格納する配列
    public bool[] holeOccupied; // 各穴がボールで占有されているかどうかを格納する配列
    public float bounceForce = 10f; // 跳ね返す力の大きさ

    private void Start()
    {
        // holeOcupied,holeposition配列のサイズをHoleObjectsの数に合わせて初期化
        holeOccupied = new bool[HoleObjects.Length];
        holePositions = new Vector3[HoleObjects.Length];
        for (int i = 0; i < HoleObjects.Length; i++)
        {
            holePositions[i] = HoleObjects[i].transform.position;
            // holeOccupied配列の初期化
            holeOccupied[i] = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 衝突したオブジェクトがBallタグを持つ場合、空いている穴の中でランダムな穴に向かって飛ばすようにする
            int holeIndex = GetRandomHoleIndex();
            if (holeIndex != -1)
            {
                Debug.Log("衝突");

                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 direction = holePositions[holeIndex] - transform.position;
                direction.y = 0;
                direction.Normalize();
                if (rb != null)
                {
                    // 一旦速度、回転をリセットしてから跳ね返す
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddForce(direction * bounceForce, ForceMode.Impulse);
                }
            }
        }
    }

    int GetRandomHoleIndex()
    {
        // 空いている穴の中でランダムな穴のインデックスを返す
        // すべての穴が占有されている場合は-1を返す
        int[] availableHoles = new int[holeOccupied.Length];
        int count = 0;
        for (int i = 0; i < holeOccupied.Length; i++)
        {
            if (!holeOccupied[i])
            {
                availableHoles[count] = i;
                count++;
            }
        }
        if (count > 0)
        {
            int randomIndex = Random.Range(0, count);
            Debug.Log("Available holes: " + count + ", Random hole index: " + availableHoles[randomIndex]);
            return availableHoles[randomIndex];
        }
        else
        {
            return -1;
        }
    }
}
