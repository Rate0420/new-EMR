using System;
using UnityEngine;

namespace EMR.Round
{
    /// <summary>
    /// ラウンド内でのメダル消費数を管理し、
    /// 必要数に達した場合にラウンドを進行させるサービス。
    /// </summary>
    public class RoundProgressService
    {
        /// <summary>
        /// ラウンド管理
        /// </summary>
        private readonly RoundManager _roundManager;

        /// <summary>
        /// ラウンド進行に必要なボール数
        /// </summary>
        public int RequiredBallCount { get; private set; }

        /// <summary>
        /// ラウンド進行に必要な所持メダル数
        /// </summary>
        public int RequiredMedalCount { get; private set; }

        /// <summary>
        /// 現在のラウンドで落下したボール数
        /// </summary>
        public int DroppedBallCount { get; private set; }



        /// <summary>
        /// 現在ラウンドで獲得したメダル数
        /// </summary>
        public int CollectedMedals { get; private set; }

        /// <summary>
        /// ラウンド進行までに必要な残りボール数
        /// </summary>
        public int RemainingBalls =>
            Math.Max(0, RequiredBallCount - DroppedBallCount);

        /// <summary>
        /// ラウンド進行までに必要な残りメダル数
        /// </summary>
        public int RemainingMedals =>
            Math.Max(0, RequiredMedalCount - CollectedMedals);

        /// <summary>
        /// ボールを取得したとき
        /// </summary>
        public event Action<int> OnBallsDropped;

        /// <summary>
        /// メダルを獲得したとき
        /// </summary>
        public event Action<int> OnMedalsCollected;

        /// <summary>
        /// ラウンドが進んだ時
        /// </summary>
        public event Action<int> OnRoundAdvanced;



        public RoundProgressService(RoundManager manager)
        {
            _roundManager = manager;
            // CurrentRoundが実際に更新された瞬間(SetRound内)に合わせて通知するため購読する
            _roundManager.OnRoundChanged += HandleRoundChanged;
        }

        
        private void HandleRoundChanged(int newRound)
        {
            OnRoundAdvanced?.Invoke(newRound);
        }

        /// <summary>
        /// ラウンド進行条件を更新する
        /// </summary>
        public void SetRequirement(RoundRequirement requirement)
        {
            RequiredBallCount = requirement.RequiredBallCount;
            RequiredMedalCount = requirement.RequiredMedalCount;
            //Debug.Log($"NextRound {_roundManager.CurrentRound + 1} -> Ball:{RequiredBallCount}, Medal:{RequiredMedalCount}");
        }

        /// <summary>
        /// メダルを追加する
        /// </summary>
        public void AddMedals(int amount)
        {
            if (amount <= 0) return;

            CollectedMedals += amount;

            OnMedalsCollected?.Invoke(amount);

            CheckRoundAdvance();
        }

        /// <summary>
        /// メダル所持数を指定した数にする
        /// </summary>
        /// <param name="amount"></param>
        public void SetConsumedMedals(int amount)
        {
            if (amount < 0) return;
            CollectedMedals = amount;
            OnMedalsCollected?.Invoke(amount);
            CheckRoundAdvance();
        }


        /// <summary>
        /// ボールを取得した数を加算する。
        /// </summary>
        public void AddDroppedBalls(int amount)
        {
            if (amount <= 0)
                return;

            DroppedBallCount += amount;

            CheckRoundAdvance();
        }

        /// <summary>
        /// ラウンド進行条件を満たしているか確認する。
        /// </summary>
        public void CheckRoundAdvance()
        {
            if (CollectedMedals < RequiredMedalCount) return;

            if (DroppedBallCount < RequiredBallCount) return;

            _roundManager.NextRound();

            //// 次ラウンド用にリセット
            //ConsumedMedals = 0;
            //DroppedBallCount = 0;

            // OnRoundAdvancedはここでは呼ばない。
            // NextRound()の時点ではCurrentRoundはまだ更新されていない(演出後にSetRoundで更新される)ため、
            // 実際にCurrentRoundが変わったタイミング(RoundManager.OnRoundChanged)で自動的に発火する。
        }

        /// <summary>
        /// ラウンド進行条件をリセットする。
        /// </summary>
        public void ResetProgress()
        {
            CollectedMedals = 0;
            DroppedBallCount = 0;
        }
    }
}