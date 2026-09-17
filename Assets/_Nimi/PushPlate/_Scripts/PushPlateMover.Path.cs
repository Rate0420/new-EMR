using UnityEngine;
using EMR.Utility;

namespace EMR.PushPlate
{
    public partial class PushPlateMover
    {
        private void CachePositions()
        {
            Vector3 direction = _moveDirection.ToVector3();

            _startPosition = transform.position + (-direction * _offset);
            _endPosition = _startPosition + direction * _moveDistance;

            float positionRate = _moveDistance > 0f ? Mathf.Clamp01(_offset / _moveDistance) : 0f;
            _t = GetCurveTimeFromValue(positionRate);
        }

        private float GetCurveTimeFromValue(float value)
        {
            if (_animationCurve == null)
                return value;

            value = Mathf.Clamp01(value);

            float low = 0f;
            float high = 1f;

            for (int i = 0; i < 16; i++)
            {
                float mid = (low + high) * 0.5f;
                float evaluated = EvaluateCurve(mid);

                if (evaluated < value)
                    low = mid;
                else
                    high = mid;
            }

            return (low + high) * 0.5f;
        }

        private float EvaluateCurve(float t)
        {
            if (_animationCurve == null)
                return Mathf.Clamp01(t);

            return Mathf.Clamp01(_animationCurve.Evaluate(t));
        }
    }
}