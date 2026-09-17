using UnityEngine;
using UnityEngine.InputSystem;

// メダルをマウス方向に飛ばす
public class MedalPower : MonoBehaviour
{
    [SerializeField] private GameObject _medalPrefab;
    [SerializeField] private float _power = 10f;

    [Header("地面判定用")]
    [SerializeField] private LayerMask _groundLayer;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShootMedal();
        }

        // スペースキーを押すことで、ゲーム全体の重力を切り替える
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Physics.gravity == Vector3.zero)
            {
                Physics.gravity = new Vector3(0, -9.81f, 0);
                Debug.Log("重力を有効にしました");
            }
            else
            {
                Physics.gravity = new Vector3(0,0,0);
                Debug.Log("重力を半分にしました");
            }
        }
    }

    private void ShootMedal()
    {
        GameObject medal = Instantiate(
            _medalPrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody rb = medal.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        Vector3 direction = GetMouseDirection();

        rb.AddForce(
            direction * _power,
            ForceMode.Impulse
        );
    }

    private Vector3 GetMouseDirection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = _cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                100f,
                _groundLayer))
        {
            // 発射位置→クリック地点
            Vector3 dir =
                (hit.point - transform.position).normalized;

            return dir;
        }

        // 当たらなかった時の保険
        return transform.forward;
    }
}