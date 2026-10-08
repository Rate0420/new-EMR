using NaughtyAttributes;
using System;
using UnityEngine;

namespace EMR.Medal.Hole
{
    /// <summary>
    /// 落ちた物を判定するクラス
    /// </summary>
    public class CollectionHole : MonoBehaviour
    {
        [SerializeField] bool _isJackSpot = false; // ジャックスポットかどうか
        [SerializeField] bool _isCount = false;    // カウントするかどうか
        [SerializeField] ReserveManager reserveManager;
        [SerializeField] BallCounter ballCounter;
        [SerializeField] SlotManager slotManager;

        /// <summary>
        /// 何かが判定されたときに発行されるイベント（引数を汎用的なICollectableに変更）
        /// </summary>
        public event Action<ICollectable> OnCollected;

        /// <summary>
        /// 何かが判定されたときに、そのオブジェクトと当たったワールド座標を渡すイベント
        /// </summary>
        public event Action<ICollectable, Vector3> OnCollectedAt;

        /// <summary>
        /// オブジェクトが落ちた位置
        /// </summary>
        public Vector3 HitPosition { get; private set; }



        public void OnTriggerEnter(Collider other)
        {
            // タグではなく、インターフェースを持っているかで判定
            var collectable = other.GetComponentInParent<ICollectable>();

            if (collectable != null)
            {
                HitPosition = other.ClosestPoint(transform.position);

                // アイテム側の消滅・回収処理を実行
                collectable.Collect();

                if (_isCount)
                {
                    if (collectable == null || ballCounter == null) return;
                    if (collectable.Info.Type == CollectableType.Ball)
                    {
                        ballCounter.BallCountAdd();
                        // SE予定地: ボール落下音
                        SEManager.Instance.SEPlays(SEManager.SEType.GetBall);
                    }

                    // SE予定地:コイン獲得時SE
                    SEManager.Instance.SEPlays(SEManager.SEType.GetCoin);
                    OnCollected?.Invoke(collectable);
                    OnCollectedAt?.Invoke(collectable, HitPosition);
                }

                if (_isJackSpot)
                {
                    // SE予定地:チャッカー
                    reserveManager?.AddReserve();
                    SEManager.Instance.SEPlays(SEManager.SEType.Chukka);
                }

                if (!_isCount && !_isJackSpot)
                {
                    // randomRangeして、slotManagerのsideHoleSaveChance以下だったらカウント
                    float randomValue = UnityEngine.Random.Range(0f, 1f);
                    if (randomValue < slotManager.sideHoleSaveChance)
                    {
                        // SE予定地:コイン獲得時SE
                        SEManager.Instance.SEPlays(SEManager.SEType.GetCoin);
                        OnCollected?.Invoke(collectable);
                        OnCollectedAt?.Invoke(collectable, HitPosition);
                    }
                }
            }
        }
    }
}