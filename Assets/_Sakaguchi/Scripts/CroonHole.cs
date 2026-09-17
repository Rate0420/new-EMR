using UnityEngine;

public class CroonHole : MonoBehaviour
{
    public int holeIndex;
    public Bounder bounder;
    public JPCCManager jpccManager;
    [SerializeField] BallEventQueue ballEventQueue; // ← 追加

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball entered hole " + holeIndex);
            if (bounder != null)
                bounder.holeOccupied[holeIndex] = true;

            // ボールが穴に落ちたことを通知（BallSpawnの待機を解除）
            ballEventQueue.NotifyBallEntered(); // ← 追加

            jpccManager.JPCCPrise(holeIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball exited hole " + holeIndex);
            if (bounder != null)
                bounder.holeOccupied[holeIndex] = false;
        }
    }
}