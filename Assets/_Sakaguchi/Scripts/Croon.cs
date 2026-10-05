using UnityEngine;

public class Croon : MonoBehaviour
{
    public float rps = 0.1f;
    [SerializeField] float rotateTime;
    [SerializeField] float stopTime;
    float timer;
    [SerializeField] bool JPC;
    [SerializeField] bool useTimer;
    float t1, t2, t3, t4;
    Rigidbody rigit;
    public bool isEnding = false;

    void Start()
    {
        rigit = GetComponent<Rigidbody>();
        t1 = rotateTime;
        t2 = rotateTime + stopTime;
        t3 = rotateTime + stopTime + rotateTime;
        t4 = rotateTime + stopTime + rotateTime + stopTime;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (JPC)
        {
            rigit.MoveRotation(rigit.rotation * Quaternion.Euler(0,0, rps * Time.deltaTime * 360));
            return;
        }


        if (!useTimer)
        {
            rigit.MoveRotation(rigit.rotation * Quaternion.Euler(0, rps * Time.deltaTime * 360, 0));
            return;
        }
        if (timer < t1)
        {
            // ³‰ñ“]
            transform.Rotate(0, rps * Time.deltaTime * 360, 0);
        }
        else if (timer < t2)
        {
            // ’âŽ~
        }
        else if (timer < t3)
        {
            // ‹t‰ñ“]
            transform.Rotate(0, -rps * Time.deltaTime * 360, 0);
        }
        else if (timer < t4)
        {
            // ’âŽ~
        }
        else
        {
            timer = 0;
        }
    }
}
