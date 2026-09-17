using System;
using UnityEngine;

namespace EMR.Utility
{
    /// <summary>
    /// 確率に応じてデータを選択するクラス
    /// </summary>
    [Serializable]
    public class ProbabilitySelector<T>
    {
        [SerializeField]
        private ProbabilityItem<T>[] _items;

        /// <summary>
        /// 配列全体の確率合計
        /// </summary>
        public float TotalProbability
        {
            get
            {
                float total = 0;

                foreach (var item in _items)
                {
                    total += item.Probability;
                }

                return total;
            }
        }

        /// <summary>
        /// 確率に応じて1つ取得
        /// </summary>
        public T GetRandom()
        {
            if (_items == null || _items.Length == 0)
            {
                throw new InvalidOperationException("ProbabilityItemが設定されていません。");
            }

            float random = UnityEngine.Random.Range(0f, TotalProbability);

            float current = 0;

            foreach (var item in _items)
            {
                current += item.Probability;

                if (random <= current)
                {
                    return item.Value;
                }
            }

            return _items[^1].Value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 合計が100か確認
        /// </summary>
        public bool IsValid()
        {
            return Mathf.Approximately(TotalProbability, 100f);
        }
#endif
    }
}