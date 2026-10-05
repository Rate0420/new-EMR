using UnityEngine;
using EMR.PushPlate;

public class Pusher_process : MonoBehaviour
{
    [SerializeField] private PushPlateMover pushPlateMover;

    [Header("びっくりプッシャー")]
    [SerializeField] private float pushMultiplier = 1.5f;

    private float originalMoveDistance;

    private void Awake()
    {
        if (pushPlateMover == null)
        {
            Debug.LogWarning(
                "【びっくりプッシャー】PushPlateMoverが設定されていません。",
                this
            );

            return;
        }

        originalMoveDistance = pushPlateMover.MoveDistance;
    }

    public void StartBigPush()
    {
        if (pushPlateMover == null)
            return;

        float newDistance =
            originalMoveDistance * pushMultiplier;

        pushPlateMover.SetMoveDistance(newDistance);

        Debug.Log(
            $"【びっくりプッシャー】発動\n" +
            $"通常移動距離：{originalMoveDistance:F2}\n" +
            $"倍率：{pushMultiplier:F1}倍\n" +
            $"今回の移動距離：{newDistance:F2}"
        );
    }

    public void ResetPush()
    {
        if (pushPlateMover == null)
            return;

        pushPlateMover.SetMoveDistance(originalMoveDistance);

        Debug.Log(
            $"【びっくりプッシャー】通常状態に戻しました\n" +
            $"移動距離：{originalMoveDistance:F2}"
        );
    }
}