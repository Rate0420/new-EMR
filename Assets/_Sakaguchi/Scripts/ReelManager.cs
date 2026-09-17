using UnityEngine;
using System.Collections;

public class ReelManager : MonoBehaviour
{
    public ReelController leftReel;
    public ReelController centerReel;
    public ReelController rightReel;
    [SerializeField] float reachtime;
    [SerializeField] EffectManager effectManager;

    Coroutine stopCoroutine;
    bool isRunning = false;
    const float ReelStopTimeout = 8f;




    bool reachEffectEnded = false;
    public bool IsReachEffectEnded => reachEffectEnded;

    // 中リール再回転・停止のアニメーションイベント用フラグ
    bool reelRestartRequested = false;
    bool reelStopRequested = false;
    public int CurrentLoseIndex { get; private set; } = 0;

    public bool IsReelRestartRequested => reelRestartRequested;
    public bool IsReelStopRequested => reelStopRequested;

    public void NotifyReelRestartReset() => reelRestartRequested = false;
    public void NotifyReelStopReset() => reelStopRequested = false;

    // アニメーションイベントから呼ぶ
    public void NotifyReelRestart() => reelRestartRequested = true;
    public void NotifyReelStop() => reelStopRequested = true;

    // PlayCharacterReach呼び出し前にloseIndexをセット
    // StopReelsCoroutineのisCharacterReachブロックで呼ぶ
    public void SetLoseIndex(int index) => CurrentLoseIndex = index;

    public void StartReels()
    {
        Debug.Log("[ReelManager] StartReels呼び出し");
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }
        isRunning = true;
        IsAllStopped = false; // ← 追加：前回の停止済みフラグが残らないようにする
        leftReel.StartSpin();
        centerReel.StartSpin();
        rightReel.StartSpin();
    }

    public void StartStopReels(int[] result)
    {
        Debug.Log("[ReelManager] StartStopReels呼び出し");
        if (!isRunning) return;
        stopCoroutine = StartCoroutine(StopReelsCoroutine(result));
    }

    // リーチ演出用：仮停止 → ガコガコ → 確定
    public IEnumerator PlayReachEffect(int reachIndex, int winIndex, bool isWin)
    {
        // ① 中リールを1つ手前で仮停止
        centerReel.TempStop(winIndex);

        // 仮停止完了まで待つ
        yield return new WaitUntil(() => centerReel.IsTempStopped);

        // ② キャラ演出はEffectManagerで呼ぶ（呼び出し元のCoroutineで管理）
        // ここでは図柄操作のみ

        // ③ ガコガコ
        yield return centerReel.GacoGaco(winIndex, count: 3, gacoSpeed: 0.08f);

        // ④ 当たり or 外れ確定
        if (isWin)
        {
            centerReel.ConfirmWin(winIndex);
        }
        else
        {
            centerReel.ConfirmLose(winIndex);
        }
    }

    IEnumerator WaitReelStopped(ReelController reel, int resultNumber, string reelName)
    {
        float elapsed = 0f;
        while (reel.IsSpinning && elapsed < ReelStopTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (reel.IsSpinning)
        {
            Debug.LogWarning($"{nameof(ReelManager)}: {reelName} reel did not stop within {ReelStopTimeout:0.0} seconds. Forcing stop to {resultNumber}.");
            reel.ForceStop(resultNumber);
        }
    }

    public void NotifyReachEffectEnd()
    {
        reachEffectEnded = true;
    }

    public void NotifyReachEffectEndReset()
    {
        reachEffectEnded = false;
    }

    public IEnumerator PlayGacoGaco(int winIndex)
    {
        yield return centerReel.GacoGaco(winIndex, count: 3, gacoSpeed: 0.08f);
    }

    public bool IsAllStopped { get; private set; } = false;

    IEnumerator StopReelsCoroutine(int[] result)
    {
        IsAllStopped = false;
        bool isReach = result[0] == result[2];
        bool isWin = isReach && (result[1] == result[0]);
        bool isCharacterReach = isReach && (
            effectManager.CurrentEffect == SlotManager.EffectType.CharacterReach ||
            effectManager.CurrentEffect == SlotManager.EffectType.SetCharacterReach ||
            effectManager.CurrentEffect == SlotManager.EffectType.HighChanceReach ||
            effectManager.CurrentEffect == SlotManager.EffectType.HighChanceSetCharacterReach
        );

        Debug.Log($"[ReelManager] StopReels開始 result:{result[0]},{result[1]},{result[2]} isReach:{isReach} isWin:{isWin} isCharacterReach:{isCharacterReach}");

        yield return new WaitForSeconds(1.0f);

        // 左停止
        leftReel.StopSpin(result[0]);
        yield return WaitReelStopped(leftReel, result[0], "left");
        Debug.Log("[ReelManager] 左停止完了");
        yield return new WaitForSeconds(0.3f);

        // 右停止
        rightReel.StopSpin(result[2]);
        yield return WaitReelStopped(rightReel, result[2], "right");
        Debug.Log("[ReelManager] 右停止完了");
        yield return new WaitForSeconds(0.3f);

        if (isCharacterReach)
        {
            // ① 通常リーチ演出（動画）
            yield return StartCoroutine(effectManager.PlayReach());
            Debug.Log("[ReelManager] PlayReach完了");

            // ② 仮停止
            centerReel.TempStop(result[1], result[0]);
            Debug.Log($"[ReelManager] TempStop呼び出し result[1]:{result[1]}");
            yield return new WaitUntil(() => centerReel.IsTempStopped);
            Debug.Log("[ReelManager] 仮停止完了");

            // ③ キャラ演出開始（アニメーション再生）＋中リール再回転→停止を内部で処理
            // 外れ図柄をセット（リーチ外れ時の停止index）
            SetLoseIndex(result[1]);
            yield return StartCoroutine(effectManager.PlayCharacterReach(result[1], isWin));
            Debug.Log("[ReelManager] キャラリーチ演出完了");
        }
        else if (isReach)
        {
            // 通常リーチ（動画のみ）
            yield return StartCoroutine(effectManager.PlayReach());
            centerReel.StopSpin(result[1], 1f);
            yield return WaitReelStopped(centerReel, result[1], "center");
            Debug.Log("[ReelManager] 通常リーチ中停止完了");
        }
        else
        {
            centerReel.StopSpin(result[1], 1f);
            yield return WaitReelStopped(centerReel, result[1], "center");
            Debug.Log("[ReelManager] 中停止完了");
        }

        Debug.Log("[ReelManager] IsAllStopped=true");
        IsAllStopped = true;
        isRunning = false;
    }
}
