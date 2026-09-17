using UnityEngine;

public class PriseGenerator : MonoBehaviour
{
    // ボールや換金アイテム的な物を生成するクラス
    public GameObject ball;
    public GameObject diamond;

    public Vector3 dischargePos0;
    public Vector3 dischargePos1;

    public int diamondProbability = 20;


    Vector3 dischargePos;

    public GameObject ItemRoot;

    public void PriseLottely()
    { 
        int r = Random.Range(1, 101);
        if (r <= diamondProbability) DisChargeCollectables(diamond);
    }

    public void DisChargeBall()
    {
        Vector3 dischargePos = new Vector3(
    Random.Range(dischargePos0.x, dischargePos1.x),
    Random.Range(dischargePos0.y, dischargePos1.y),
    Random.Range(dischargePos0.z, dischargePos1.z)
);

        Instantiate(ball, dischargePos, Quaternion.identity, ItemRoot.transform);
    }

    public void DisChargeCollectables(GameObject obj)
    {
        Vector3 dischargePos = new Vector3(
    Random.Range(dischargePos0.x, dischargePos1.x),
    Random.Range(dischargePos0.y, dischargePos1.y),
    Random.Range(dischargePos0.z, dischargePos1.z)
);

        Instantiate(obj, dischargePos, Quaternion.identity, ItemRoot.transform);
    }



    void Update()
    {
        
    }
}
