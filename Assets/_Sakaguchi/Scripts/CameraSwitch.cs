using UnityEngine;

public enum CameraPosition
{
    Default,
    SlotZoom,
    JPCC,
    JPC
}

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] GameObject pip;

    public Transform DefaultCameraPoint;  // 基本位置
    public Transform SlotZoomCameraPoint;  // スロットズーム位置
    public Transform JPCCCameraPoint;  // JPCC位置
    public Transform JPCCameraPoint;  // JPC位置

    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    public GameObject Camera;

    public CameraPosition currentCameraPosition;

    Transform targetPoint;

    private void Start()
    {
        pip.SetActive(false);
        targetPoint = DefaultCameraPoint;  // 初期位置を基本位置に設定
        currentCameraPosition = CameraPosition.Default;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            pip.SetActive(false);
            targetPoint = DefaultCameraPoint;
            currentCameraPosition = CameraPosition.Default;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            pip.SetActive(false);
            targetPoint = SlotZoomCameraPoint;
            currentCameraPosition = CameraPosition.SlotZoom;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            pip.SetActive(true);
            targetPoint = JPCCCameraPoint;
            currentCameraPosition = CameraPosition.JPCC;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            pip.SetActive(false);
            targetPoint = JPCCameraPoint;
            currentCameraPosition = CameraPosition.JPC;
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
