using UnityEngine;

/// <summary>
/// Rigidbodyベースのプッシャー
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PushPlate : MonoBehaviour
{
    [SerializeField, Min(0)] private float _moveDistance = 0.1f;
    [SerializeField, Min(0)] private float _moveSpeed = 1.0f;
    [SerializeField] private Vector3 _moveDirection = Vector3.forward;

    private Rigidbody _rigidBody;

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private float _t;          // 0〜1の補間値
    private int _direction = 1; // 1: 前進, -1: 後退

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.isKinematic = true; // ←重要（押す側はキネマティック）

        _startPosition = transform.position;
        _endPosition = _startPosition + _moveDirection.normalized * _moveDistance;
    }

    private void FixedUpdate()
    {
        if (_moveDistance <= 0f || _moveSpeed <= 0f) return;

        // t更新（時間ベースではなく距離ベース）
        float delta = (_moveSpeed / _moveDistance) * Time.fixedDeltaTime;
        _t += delta * _direction;

        // 端で反転
        if (_t >= 1f)
        {
            _t = 1f;
            _direction = -1;
        }
        else if (_t <= 0f)
        {
            _t = 0f;
            _direction = 1;
        }

        // 補間位置
        Vector3 target = Vector3.Lerp(_startPosition, _endPosition, _t);

        // Rigidbodyで移動（これが重要）
        _rigidBody.MovePosition(target);
    }
}