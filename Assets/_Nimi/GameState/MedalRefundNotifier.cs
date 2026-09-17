using System.Collections.Generic;
using UnityEngine;
using System;
using EMR.Utility;

namespace EMR.Core
{
    /// <summary>
    /// メダルの払い戻しを外部へ通知するシングルトンコンポーネント。
    /// 払い戻し枚数のバリデーションを行い、<see cref="OnRefundRequested"/> イベントを発火する。
    /// 実際のメダル加算処理は <see cref="MedalRefundBehaviour"/> など、
    /// このイベントを購読するクラスに委ねる。
    /// </summary>
    public class MedalRefundNotifier
    {
        /// <summary>
        /// 払い戻し枚数
        /// </summary>
        public int RefundAmount { get; private set; }

        /// <summary>
        /// 払い戻し数に変化があった時に呼ばれるイベント
        /// </summary>
        public event Action<int> OnChangeRefundAmounted;

        /// <summary>
        /// 払い戻しが要求されたときに発火するイベント。
        /// 引数は払い戻し枚数（0以上の整数）。
        /// </summary>
        public event Action<int> OnRefundRequested;

        /// <summary>
        /// 全てのメダルを払い戻せた時に発火するイベント
        /// </summary>
        public event Action OnRefundConfirmed;


        private ProbabilitySelector<GameObject> _selector;

        /// <summary>
        /// 払い戻しを行うオブジェクトと確率のリスト
        /// </summary>
        public ProbabilitySelector<GameObject> Selector => _selector;


        /// <summary>
        /// 払い戻しを要求する。
        /// </summary>
        /// <param name="amount">払い戻すメダルの枚数。</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="amount"/> が負の値の場合にスローされる。
        /// </exception>
        public void RequestRefund(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "払い戻し枚数は0以上の値を指定してください。");
            }

            RefundAmount += amount;
            OnChangeRefundAmounted?.Invoke(RefundAmount);
            OnRefundRequested?.Invoke(RefundAmount);
        }

        /// <summary>
        /// 払い戻し枚数を無くす
        /// </summary>
        /// <param name="amount">無くす枚数</param>
        public void RemoveRefund(int amount)
        {
            RefundAmount--;

            if (RefundAmount < 0)
            {
                RefundAmount = 0;
                OnRefundConfirmed?.Invoke();
                return;
            }
            else
            {
                OnChangeRefundAmounted?.Invoke(RefundAmount);
            }
        }
    }
}
