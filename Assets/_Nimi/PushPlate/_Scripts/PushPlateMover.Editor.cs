using UnityEngine;
using EMR.Utility;

namespace EMR.PushPlate
{
    public partial class PushPlateMover
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_moveDistance < 0f) _moveDistance = 0f;
            if (_moveSpeed < 0f) _moveSpeed = 0f;

            _offset = Mathf.Clamp(_offset, 0f, _moveDistance);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_isActive) return;

            Vector3 offset = Vector3.up * _height;
            Vector3 direction = _moveDirection.ToVector3();

            Vector3 start = Application.isPlaying
                ? _startPosition + offset
                : transform.position + offset + (-direction * _offset);

            Vector3 end = start + direction * _moveDistance;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, end);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(start, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(end, 0.05f);
        }
#endif
    }
}
