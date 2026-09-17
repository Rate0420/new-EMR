using System;
using UnityEngine;

namespace EMR.Utility
{
    /// <summary>
    /// 確率付きデータ
    /// </summary>
    [Serializable]
    public class ProbabilityItem<T>
    {
        [SerializeField] private T _value;
        [SerializeField][Range(0f, 100f)] private float _probability;

        public T Value => _value;
        public float Probability => _probability;
    }
}