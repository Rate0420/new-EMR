using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using EMR.Core;
using EMR.Utility;


namespace EMR.Medal.Refund
{
    /// <summary>
    /// <see cref="GameState.OnRefundRequested"/> を受け取り、E
    /// 実際に所持メダルへ枚数を加算するコンポ�Eネント、E
    /// </summary>
    public class MedalRefundBehaviour : MonoBehaviour
    {
        [SerializeField] Transform _root;
        [SerializeField] MedalRefundSpawner[] _spawner;
        [SerializeField] ProbabilitySelector<GameObject> _enemySelector;

        // 払い戻し�E通知
        private MedalRefundNotifier _refundNotifier;

        public System.Action OnRefundFinished;  // 終亁E��知

        public System.Action<int> OnMedalSpawned; // 残り枚数


        private void Start()
        {
            _refundNotifier = GameState.Instance.RefundNotifier;
            _refundNotifier.OnRefundRequested += HandleRefundRequested;
        }

        private void OnDisable()
        {
            if (_refundNotifier != null)
            {
                // イベントを解除
                _refundNotifier.OnRefundRequested -= HandleRefundRequested;
            }
        }

        private void HandleRefundRequested(int refundAmount)
        {
            ProcessRefundAsync(refundAmount).Forget();
        }

        /// <summary>
        /// 払い戻し要求を受け取り、メダルを排出する
        /// </summary>
        /// <param name="amount">払い戻すメダルの枚数</param>
        private async UniTask ProcessRefundAsync(int refundAmount)
        {
            Debug.Log($"Refund: {refundAmount}");

            try
            {
                while (_refundNotifier.RefundAmount > 0)
                {
                    MedalRefundSpawner spawner = _spawner[Random.Range(0, _spawner.Length)];
                    GameObject prefab = _enemySelector.GetRandom();

                    spawner.SpawnMedal(prefab, _root);

                    _refundNotifier.RemoveRefund(1);

                    // Notify remaining payout count.
                    OnMedalSpawned?.Invoke(_refundNotifier.RefundAmount);

                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f));

                    await UniTask.WaitUntil(() => !GameState.Instance.GamePause.isPaused);
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                OnRefundFinished?.Invoke();
            }
        }
    }
}
