using System;

namespace EMR.Medal
{
    /// <summary>
    /// 現在所持しているメダルの数を管理するクラス
    /// </summary>
    public class MedalsOwnedModel
    {
        /// <summary>
        /// 現在所持しているメダル数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 消費したメダルの累計数
        /// </summary>
        public int ConsumedCount { get; private set; }


        /// <summary>
        /// 所持メダル数が変更されたときに呼び出されるイベント
        /// </summary>
        public event Action<int> OnCountChanged;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="startCount">初期メダル数</param>
        public MedalsOwnedModel(int startCount)
        {
            SetCount(startCount);
        }


        /// <summary>
        /// 現在のメダル数を指定した数にするメソッド
        /// </summary>
        /// <param name="count">設定するメダル数</param>
        /// <exception cref="ArgumentException">メダル数が負の数の場合</exception>
        public void SetCount(int count)
        {
            if (count < 0) throw new ArgumentException("メダル数は負の数にすることはできません", nameof(count));
            Count = count;
            OnCountChanged?.Invoke(Count);
        }


        /// <summary>
        /// 現在のメダル数から+1するメソッド
        /// </summary>
        public void AddMedal()
        {
            AddMedal(1);
        }


        /// <summary>
        /// 現在のメダル数から指定した数だけ増やすメソッド
        /// </summary>
        /// <param name="count">増やす数</param>
        /// <exception cref="ArgumentException">増やす数は負の数にすることはできません</exception>
        public void AddMedal(int count)
        {
            if (count < 0) throw new ArgumentException("増やす数は負の数にすることはできません", nameof(count));

            Count += count;
            OnCountChanged?.Invoke(Count);
        }


        /// <summary>
        /// 現在のメダル数から-1するメソッド
        /// </summary>
        public void RemoveMedal()
        {
            RemoveMedal(1);
        }


        /// <summary>
        /// 現在のメダル数から指定した数だけ減らすメソッド
        /// </summary>
        /// <param name="count">減らす数</param>
        /// <exception cref="ArgumentException">減らす数は負の数にすることはできません</exception>
        public void RemoveMedal(int count)
        {
            if (count < 0) throw new ArgumentException("減らす数は負の数にすることはできません", nameof(count));

            int removed = Math.Min(Count, count);

            Count -= removed;
            ConsumedCount += removed;

            OnCountChanged?.Invoke(Count);
        }
    }
}