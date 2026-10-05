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

    public Transform DefaultCameraPoint;       // 基本位置
    public Transform SlotZoomCameraPoint;      // スロットズーム位置
    public Transform JPCCCameraPoint;          // JPCC位置
    public Transform JPCCameraPoint;           // JPC位置

    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    public GameObject Camera;

    public CameraPosition currentCameraPosition;

    private Transform targetPoint;

    private void Start()
    {
        pip.SetActive(false);

        targetPoint = DefaultCameraPoint;
        currentCameraPosition = CameraPosition.Default;
    }

    private void Update()
    {
        // なめらかに移動
        transform.position = Vector3.Lerp(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // なめらかに回転
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetPoint.rotation,
            rotateSpeed * Time.deltaTime
        );
    }

    // =========================
    // ボタンから呼び出す
    // =========================

    public void SetDefault()
    {
        pip.SetActive(false);

        targetPoint = DefaultCameraPoint;
        currentCameraPosition = CameraPosition.Default;
    }

    public void SetSlotZoom()
    {
        pip.SetActive(false);

        targetPoint = SlotZoomCameraPoint;
        currentCameraPosition = CameraPosition.SlotZoom;
    }

    public void SetJPCC()
    {
        pip.SetActive(true);

        targetPoint = JPCCCameraPoint;
        currentCameraPosition = CameraPosition.JPCC;
    }

    public void SetJPC()
    {
        pip.SetActive(false);

        targetPoint = JPCCameraPoint;
        currentCameraPosition = CameraPosition.JPC;
    }
}