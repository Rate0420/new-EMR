using System;
using UnityEngine;

namespace EMR.Medal
{
    public class CollectableItem : MonoBehaviour, ICollectable
    {
        [SerializeField] CollectableData _info;

        /// <summary>
        /// メダルの情報を取得します。
        /// </summary>
        public CollectableData Info => _info;

        /// <summary>
        /// メダルのカウント数を取得します。
        /// </summary>
        public int Count => _info.Count;

        /// <summary>
        /// 獲得したときに呼ばれるイベント
        /// </summary>
        public event Action OnCollect;

        /// <summary>
        /// メダルを獲得したときのメソッド
        /// </summary>
        public void Collect()
        {
            OnCollect?.Invoke();
            // 演出とか追加する予定s
            Destroy(gameObject);
        }
    }
}