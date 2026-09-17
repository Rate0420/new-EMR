using EMR.Medal.Launch;
using EMR.Medal.Refund.l;
using NaughtyAttributes;
using UnityEngine;

namespace EMR.Medal.Refund
{
    /// <summary>
    /// 払い戻しメダルを生成するコンポーネント
    /// </summary>
    public sealed class MedalRefundSpawner : MonoBehaviour
    {
        [Header("生成場所")]
        [SerializeField] Transform _root;

        [Header("生成位置")]
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _rotate;

        [Header("生成範囲")]
        [SerializeField] private Vector3 _pointA;
        [SerializeField] private Vector3 _pointB;

        [Header("生成方向")]
        [SerializeField] private Vector3 _direction = Vector3.up;

        [Header("生成時の力")]
        [SerializeField] private float _minPower = 10f;
        [SerializeField] private float _maxPower = 50f;


        /// <summary>
        /// メダルを生成してランダムな力を与える
        /// </summary>
        /// <param name="prefab">生成するメダルPrefab</param>
        public void SpawnMedal(GameObject prefab, Transform root)
        {
            Quaternion rotation = Quaternion.Euler(_rotate);

            // ランダムな生成位置
            Vector3 localPosition = new(
                Random.Range(_pointA.x, _pointB.x),
                Random.Range(_pointA.y, _pointB.y),
                Random.Range(_pointA.z, _pointB.z));

            Vector3 spawnPosition =
                transform.position +
                _offset +
                rotation * localPosition;

            // メダル生成
            GameObject medal = Instantiate(
                prefab,
                spawnPosition,
                rotation);

            if (root == null) root = _root;

            medal.transform.SetParent(root);

            // Rigidbodyがあれば力を加える
            if (medal.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 forceDirection = (rotation * _direction).normalized;
                float power = Random.Range(_minPower, _maxPower);

                rb.AddForce(forceDirection * power, ForceMode.Impulse);
            }

            // haslanded 処理(当たった際のメダルに特殊効果を付与させない)
            medal.GetComponent<MedalLanded>().hasLanded = true;
        }

        private void OnDrawGizmos()
        {
            Quaternion rotation = Quaternion.Euler(_rotate);
            Vector3 origin = transform.position + _offset;

            Vector3 start = origin + rotation * _pointA;
            Vector3 end = origin + rotation * _pointB;

            // 生成範囲
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(start, 0.15f);
            Gizmos.DrawSphere(end, 0.15f);
            Gizmos.DrawLine(start, end);

            // 生成位置
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.1f);

            // 発射方向
            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, (rotation * _direction).normalized * 2f);
        }
    }
}