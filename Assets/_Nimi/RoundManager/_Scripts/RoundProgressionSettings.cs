using System;
using System.Collections.Generic;
using UnityEngine;

namespace EMR.Round
{
    /// <summary>
    /// ラウンド進行に必要な設定データ。
    /// ScriptableObject アセットとして作成して利用する。
    /// </summary>
    [CreateAssetMenu(
        fileName = "RoundProgressionSettings",
        menuName = "Scriptable Objects/Round Progression Settings")]
    public class RoundProgressionSettings : ScriptableObject
    {
        private const int DefaultRoundCount = 8;

        /// <summary>
        /// リストの0番目はラウンド1、1番目はラウンド2に対応する。
        /// </summary>
        [SerializeField] private List<RoundRequirement> _roundRequirements = new();

        public int RoundCount => _roundRequirements.Count;


        /// <summary>
        /// 指定ラウンドの進行条件を取得する。
        /// </summary>
        public RoundRequirement GetRequirement(int roundNumber)
        {
            if (roundNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(roundNumber),
                    "ラウンド番号は1以上にしてください。");
            }

            int index = roundNumber - 1;

            if (index >= _roundRequirements.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(roundNumber),
                    $"ラウンド{roundNumber}の進行条件が設定されていません。");
            }

            return _roundRequirements[index];
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _roundRequirements = new List<RoundRequirement>(DefaultRoundCount);

            for (int i = 0; i < DefaultRoundCount; i++)
            {
                _roundRequirements.Add(new RoundRequirement());
            }
        }
#endif
    }

    [Serializable]
    public class RoundRequirement
    {
        [field: SerializeField, Min(0)]
        public int RequiredBallCount { get; private set; } = 2;

        [field: SerializeField, Min(0)]
        public int RequiredMedalCount { get; private set; } = 75;
    }
}