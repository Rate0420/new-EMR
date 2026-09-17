using System;
using UnityEngine;
using UnityEngine.Events;

namespace EMR.Round
{
    /// <summary>
    /// 現在のラウンド番号を管理するクラス。
    /// ラウンド変更時にはイベントを通知する。
    /// </summary>
    public class RoundManager
    {
        public bool RoundChangeWait = false;

        public UnityEvent onNextRoundCalled = new UnityEvent();

        /// <summary>
        /// 現在のラウンド数。
        /// </summary>
        public int CurrentRound { get; private set; }

        /// <summary>
        /// ラウンド数が変更されたときに呼ばれるイベント。
        /// </summary>
        public event Action<int> OnRoundChanged;


        public RoundManager(int startRound = 1)
        {
            SetRound(startRound);
        }

        /// <summary>
        /// 次のラウンドへ進める。
        /// </summary>
        public void NextRound()
        {
            if (RoundChangeWait) return;
            onNextRoundCalled?.Invoke();
            RoundChangeWait = true;
        }

        /// <summary>
        /// 前のラウンドへ戻す。
        /// </summary>
        public void PrevRound() => SetRound(CurrentRound - 1);

        /// <summary>
        /// 指定したラウンド数に設定する。
        /// </summary>
        public void SetRound(int value)
        {
            int newRound = Math.Max(0, value);

            if (CurrentRound == newRound)
                return;

            CurrentRound = newRound;
            OnRoundChanged?.Invoke(CurrentRound);

            Debug.Log($"ラウンド進行 {CurrentRound}");
            // ここではRoundChangeWaitを解除しない。
            // （ラウンドチェンジ演出・ストーリーがまだ続いている場合があるため、
            //   　二重にラウンド進行処理が走ってしまうのを防ぐ）
        }

        /// <summary>
        /// ラウンドチェンジ処理(演出・ストーリー含む)が完全に終わったときに呼ぶ。
        /// これを呼ぶまでは NextRound() は再度発火しない。
        /// </summary>
        public void FinishRoundChange()
        {
            RoundChangeWait = false;
        }
    }
}