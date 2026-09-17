using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Transform DefaultCameraPoint;  // 基本位置
    public Transform SlotZoomCameraPoint;  // スロットズーム位置
    public Transform JPCCCameraPoint;  // JPCC位置
    public Transform JPCCameraPoint;  // JPC位置

    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    public GameObject Camera;

    Transform targetPoint;

    private void Start()
    {
        targetPoint = DefaultCameraPoint;  // 初期位置を基本位置に設定
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            targetPoint = DefaultCameraPoint;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            targetPoint = SlotZoomCameraPoint;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            targetPoint = JPCCCameraPoint;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            targetPoint = JPCCameraPoint;
        }

        // なめらかに移動
        transform.position = Vector3.Lerp(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime);

        // なめらかに回転
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetPoint.rotation,
            rotateSpeed * Time.deltaTime);
    }
}
