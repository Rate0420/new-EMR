using EMR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMR.Core
{
    /// <summary>
    /// 「今、ゲームを完全に止めて何か1つの処理(メニュー・シナリオ・JPC払い出し・
    /// ラウンドチェンジ・ミニイベント/玉入れキュー 等)だけを行いたい」場合に、
    /// 全員が共通で使う排他ロック。
    ///
    /// これまでは SceneChanger.IsSceneActive / JPCPayoutManager.IsJPCProcessing /
    /// BallEventQueue.IsQueueProcessing / RoundChange.IsRoundChangeProcessing のように
    /// システムごとにバラバラのフラグを持ち、新しいシステムを追加するたびに
    /// 「既存の全員が新しいフラグを見るように書き足す」作業が必要で、
    /// 書き忘れるとシステム同士が同時に走ってバグる、という問題が繰り返し起きていた。
    ///
    /// このクラスを使えば、新しいシステムを追加しても Acquire/Release を呼ぶだけでよく、
    /// 既存のシステム側のコードを一切触らなくても自動的に排他される。
    ///
    /// 使い方:
    ///   yield return GameState.Instance.GameLock.Acquire("Menu");
    ///   // ここに来た時点で、リールは止まっていて、他の誰もロックを持っていない
    ///   ... 実際の処理 ...
    ///   GameState.Instance.GameLock.Release("Menu");
    ///
    /// 名前(who)は Debug.Log やインスペクタでの確認用。
    /// 同じ名前で二重に Acquire しても壊れないよう、内部では保持者数をカウントしている。
    /// </summary>
    /// 

    public interface IReserveGate
    {
        bool isProcessing { get; }
        bool isBetweenReserves { get; }
        bool pauseRequested { get; set; }
    }

    public class GameLockManager
    {
        IReserveGate _reserveManager;

        // 名前ごとの保持カウント(同じ名前が入れ子でAcquireしても崩れないように)
        readonly Dictionary<string, int> _holders = new Dictionary<string, int>();

        /// <summary>
        /// ReserveManagerはシーン上のオブジェクトのため、GameStateの初期化時点では
        /// まだ存在しないことがある。ReserveManager自身のStart()等から、準備できた時点で
        /// これを呼んで登録してもらう。
        /// </summary>
        public void RegisterReserveManager(IReserveGate reserveManager)
        {
            _reserveManager = reserveManager;
        }


        /// <summary>誰か(自分以外)がロックを持っているか</summary>
        public bool IsLocked => _holders.Count > 0;

        /// <summary>現在のロック保持者一覧(デバッグ表示用)</summary>
        public IEnumerable<string> Holders => _holders.Keys;

        /// <summary>
        /// 指定した名前(who)がロックを持っているか。
        /// 「自分以外の誰かが処理中か」を調べたい時に "!IsHeldByOthers(自分の名前)" のように使う。
        /// </summary>
        public bool IsHeldByOthers(string who)
        {
            foreach (var kv in _holders)
            {
                if (kv.Key != who && kv.Value > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// ロックを取得する。
        /// 「自分以外の誰かがロックを持っている間」「リールが回転中の間」は自動的に待つ。
        /// 取得した瞬間からリールも一時停止状態になる(pauseRequested = true)。
        /// </summary>
        public IEnumerator Acquire(string who)
        {
            yield return new WaitUntil(() =>
                !IsHeldByOthers(who)
                && (_reserveManager == null || !_reserveManager.isProcessing || _reserveManager.isBetweenReserves)
            );

            if (!_holders.ContainsKey(who)) _holders[who] = 0;
            _holders[who]++;

            if (_reserveManager != null) _reserveManager.pauseRequested = true;
            Debug.Log($"[GameLock] Acquire: {who} (保持者:{string.Join(",", Holders)})");
        }

        /// <summary>
        /// ロックを解放する。誰も持っていなければ自動的にリールの一時停止も解除される。
        /// 保持していない名前を解放しようとしても何も起きない(安全に無視される)。
        /// </summary>
        public void Release(string who)
        {
            if (_holders.ContainsKey(who))
            {
                _holders[who]--;
                if (_holders[who] <= 0) _holders.Remove(who);
            }

            Debug.Log($"[GameLock] Release: {who} (残り保持者:{string.Join(",", Holders)})");

            if (!IsLocked && _reserveManager != null)
            {
                _reserveManager.pauseRequested = false;
            }
        }
    }
}
