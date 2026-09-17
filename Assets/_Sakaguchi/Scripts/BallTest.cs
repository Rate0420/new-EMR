using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    public Vector3 startpos;
    public Quaternion startrot;
    public GameObject BallObject;
    [SerializeField] float force = 300f;
    [SerializeField] GameObject items;
    // ボールのList
    public List<GameObject> Balls = new List<GameObject>();

    private void Update()
    {
        // Sを押すと新しくBallを生成する
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartJPCC();
        }

        // Rを押すと全てのBallを削除する
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetJPCC();
        }
    }

    public void StartJPCC()
    {
        GameObject Ball = Instantiate(BallObject, startpos, startrot,items.transform);
        Rigidbody newRb = Ball.GetComponent<Rigidbody>();
        newRb.AddForce(Ball.transform.forward * force, ForceMode.Impulse);
        Balls.Add(Ball);
    }

    public void ResetJPCC()
    {
        foreach (GameObject ball in Balls)
        {
            Destroy(ball);
        }
        Balls.Clear();
    }
}
