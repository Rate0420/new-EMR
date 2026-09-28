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

    /// <summary>
    /// ロックの種類。
    /// FullScreen: メニュー・シナリオ・ラウンドチェンジ等、画面を完全に占有する処理。
    ///             リールが止まるのを待ってから取得し、取得中はリールの保留消化も止める。
    /// SubMonitor: JPC払い出し・JPCC抽選等、サブモニター側で表示しつつ
    ///             リールの保留消化とは並行して進めたい処理。
    ///             FullScreen系の開始だけをブロックし、リールは止めない。
    /// </summary>
    public enum GameLockKind
    {
        FullScreen,
        SubMonitor
    }

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

        // 各名前がどちらの種類でAcquireされているか
        readonly Dictionary<string, GameLockKind> _kinds = new Dictionary<string, GameLockKind>();

        // FullScreen系のAcquire呼び出し中(WaitUntilで待機中も含む)の人数。
        // 「実際にロックを持っているか」より広く、「Acquireを呼んでからReleaseするまで」をカバーする。
        // これがある間はずっとpauseRequestedをtrueにしておくことで、
        // 「リールが止まった瞬間、Acquire側がまだpauseRequestedを立てていない一瞬の隙」に
        // ReserveManagerが次のスピンを始めてしまう競合を防ぐ。
        // SubMonitor系はここに加算しない(リールを止める対象ではないため)。
        readonly Dictionary<string, int> _pending = new Dictionary<string, int>();

        /// <summary>
        /// ReserveManagerはシーン上のオブジェクトのため、GameStateの初期化時点では
        /// まだ存在しないことがある。ReserveManager自身のStart()等から、準備できた時点で
        /// これを呼んで登録してもらう。
        /// </summary>
        public void RegisterReserveManager(IReserveGate reserveManager)
        {
            _reserveManager = reserveManager;
        }


        /// <summary>誰か(自分以外)がロックを持っているか(種類を問わず)</summary>
        public bool IsLocked => _holders.Count > 0;

        /// <summary>FullScreen系のAcquireが呼ばれている(待機中含む)か</summary>
        public bool HasPending => _pending.Count > 0;

        /// <summary>現在のロック保持者一覧(デバッグ表示用)</summary>
        public IEnumerable<string> Holders => _holders.Keys;

        /// <summary>
        /// 指定した名前(who)以外の誰かがロックを持っているか(種類を問わず)。
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
        /// 指定した名前(who)以外の「FullScreen系」の保持者がいるか。
        /// SubMonitor系のAcquireは、他のSubMonitor系とは競合させず、
        /// FullScreen系(メニュー・シナリオ・ラウンドチェンジ)の開始だけをブロックしたい時に使う。
        /// </summary>
        public bool IsHeldByFullScreenOthers(string who)
        {
            foreach (var kv in _holders)
            {
                if (kv.Key == who || kv.Value <= 0) continue;
                if (_kinds.TryGetValue(kv.Key, out var kind) && kind == GameLockKind.FullScreen) return true;
            }
            return false;
        }

        /// <summary>
        /// ロックを取得する。
        /// kindがFullScreen(既定)の場合: 「自分以外の誰かがロックを持っている間(種類問わず)」
        /// 「リールが回転中の間」は自動的に待ち、取得中はリールの保留消化も止める。
        /// kindがSubMonitorの場合: FullScreen系が動いている間だけ待ち、リールは止めない。
        /// SubMonitor同士は互いにブロックしない(必要なら呼び出し側の仕組みで直列化すること)。
        /// </summary>
        public IEnumerator Acquire(string who, GameLockKind kind = GameLockKind.FullScreen)
        {
            _kinds[who] = kind;

            if (kind == GameLockKind.FullScreen)
            {
                if (!_pending.ContainsKey(who)) _pending[who] = 0;
                _pending[who]++;
                if (_reserveManager != null) _reserveManager.pauseRequested = true;

                yield return new WaitUntil(() =>
                    !IsHeldByOthers(who)
                    && (_reserveManager == null || !_reserveManager.isProcessing || _reserveManager.isBetweenReserves)
                );
            }
            else // SubMonitor
            {
                yield return new WaitUntil(() => !IsHeldByFullScreenOthers(who));
            }

            if (!_holders.ContainsKey(who)) _holders[who] = 0;
            _holders[who]++;

            Debug.Log($"[GameLock] Acquire: {who} ({kind}) (保持者:{string.Join(",", Holders)})");
        }

        /// <summary>
        /// ロックを解放する。FullScreen系が誰も残っていなければ自動的にリールの一時停止も解除される。
        /// 保持していない(Acquireしていない)名前を解放しようとしても何も起きない(安全に無視される)。
        /// </summary>
        public void Release(string who)
        {
            if (_holders.ContainsKey(who))
            {
                _holders[who]--;
                if (_holders[who] <= 0)
                {
                    _holders.Remove(who);
                    _kinds.Remove(who);
                }
            }

            if (_pending.ContainsKey(who))
            {
                _pending[who]--;
                if (_pending[who] <= 0) _pending.Remove(who);
            }

            Debug.Log($"[GameLock] Release: {who} (残り保持者:{string.Join(",", Holders)})");

            // FullScreen系のpendingがすべて無くなった時だけリールの一時停止を解除する。
            // (SubMonitor系はそもそもpauseRequestedを立てないので、ここには影響しない)
            if (!HasPending && _reserveManager != null)
            {
                _reserveManager.pauseRequested = false;
            }
        }
    }
}
