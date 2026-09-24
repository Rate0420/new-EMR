using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using EMR.Core;
using EMR.Utility;


namespace EMR.Medal.Refund
{
    /// <summary>
    /// MedalRefundNotifier.OnRefundRequested を受け取り、
    /// 実際に払い戻しメダルを生成するコンポーネント。
    /// </summary>
    public class MedalRefundBehaviour : MonoBehaviour
    {
        [SerializeField] Transform _root;
        [SerializeField] MedalRefundSpawner[] _spawner;
        [SerializeField] ProbabilitySelector<GameObject> _enemySelector;

        // 払い戻しの通知元
        private MedalRefundNotifier _refundNotifier;

        public System.Action OnRefundFinished;  // 終了通知

        public System.Action<int> OnMedalSpawned; // 残り枚数


        private void Start()
        {
            _refundNotifier = GameState.Instance.RefundNotifier;
            Subscribe();
        }

        private void OnEnable()
        {
            // SetActive(false)→(true)で再度有効化された時に、購読が切れたままにならないようにする。
            // Start()より先にOnEnableが呼ばれるケース(最初の有効化時)では
            // _refundNotifierがまだnullなので、その場合はStart()側の呼び出しで購読される。
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_refundNotifier == null) return;
            // 二重登録を避けるため、一度外してから登録する
            _refundNotifier.OnRefundRequested -= HandleRefundRequested;
            _refundNotifier.OnRefundRequested += HandleRefundRequested;
        }

        private void Unsubscribe()
        {
            if (_refundNotifier != null)
            {
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
        /// <param name="refundAmount">払い戻すメダルの枚数</param>
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
