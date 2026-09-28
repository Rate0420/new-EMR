using UnityEngine;
using TMPro;

public class CroonHole : MonoBehaviour
{
    public int holeIndex;
    public Bounder bounder;
    public JPCCManager jpccManager;
    [SerializeField] BallEventQueue ballEventQueue; // ← 追加
    public bool isJPC = false;
    [SerializeField] TextMeshPro prizeText;

    private void Start()
    {
        prizeText.text = jpccManager.GetPriseText(holeIndex, isJPC);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isJPC)
        {
            if (other.transform.parent.CompareTag("Ball"))
            {
                Debug.Log("Ball entered hole " + holeIndex);
                if (bounder != null)
                    bounder.holeOccupied[holeIndex] = true;

                // ボールが穴に落ちたことを通知（BallSpawnの待機を解除）
                ballEventQueue.NotifyBallEntered(); // ← 追加

                jpccManager.JPCCPrise(holeIndex);
            }
        }

        else if (other.transform.parent.CompareTag("Ball"))
        {
            // otherを削除
            jpccManager.JPCPrise(holeIndex);
            Destroy(other.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isJPC)
        {
            if (other.transform.parent.CompareTag("Ball"))
            {
                Debug.Log("Ball exited hole " + holeIndex);
                if (bounder != null)
                    bounder.holeOccupied[holeIndex] = false;
            }
        }
    }
}