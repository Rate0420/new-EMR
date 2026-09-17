using UnityEngine;
using System.Collections;
using EMR.Core;

public class ReelController : MonoBehaviour
{
    [SerializeField] RectTransform content;
    [SerializeField] float symbolHeight = 1280f;
    [SerializeField] float speed = 5f;
    [SerializeField] int symbolCount = 9;
    [SerializeField] float stopDecelerationMultiplier = 1f;

    // メニュー中などにリールの見た目を確実に止めるための保険
    // (ReserveManagerと同じくGameState.Instance.GamePauseを直接見る)
    GamePause gamePause;

    void Start()
    {
        gamePause = GameState.Instance.GamePause;
    }

    public bool IsSpinning => isSpinning;

    // 仮停止・ガコガコ中かどうか外から確認できるように
    public bool IsTempStopped { get; private set; } = false;

    [SerializeField] float currentIndex = 0f;
    bool isSpinning = false;
    bool isStopping = false;
    int targetIndex = 0;
    float currentSpeed;

    // ガコガコ用
    Coroutine gacogacoCoroutine;

    void ApplyPosition()
    {
        float corrected = (symbolCount - (currentIndex % symbolCount)) % symbolCount;
        content.anchoredPosition = new Vector2(
            content.anchoredPosition.x,
            -corrected * symbolHeight
        );
    }

    bool isTempStopping = false; // ← 追加

    public void StopSpin(int index, float decelMultiplier = 1f)
    {
        int converted = (symbolCount + 1) - index;
        targetIndex = converted % symbolCount;
        isStopping = true;
        isTempStopping = false; // ← 通常停止はfalse
        mustFullRotation = false;
        rotationCount = 0f;
        stopDecelerationMultiplier = decelMultiplier;

        float remaining = currentIndex - targetIndex;
        if (remaining <= 0) remaining += symbolCount;
        if (remaining < 1.5f) mustFullRotation = true;

        Debug.Log($"[StopSpin] index:{index} targetIndex:{targetIndex} currentIndex:{currentIndex:F2} remaining:{remaining:F2} mustFull:{mustFullRotation}");
    }

    // 1..symbolCountの範囲に正規化する(0や範囲外にならないようラップする)
    int WrapSymbolNumber(int n)
    {
        return ((n - 1) % symbolCount + symbolCount) % symbolCount + 1;
    }

    // pendingIndex: 本来の最終結果の図柄番号
    // avoidNumber: 既に停止済みの左右リールの図柄番号(渡さない場合は-1)
    //   仮停止では「本来の結果からひとつだけズラした図柄」を見せてタメを作るが、
    //   そのズラした図柄がavoidNumberと同じだと、仮停止の時点で揃って見えてしまう。
    //   その場合は反対側にズラすことで回避する。
    public void TempStop(int pendingIndex, int avoidNumber = -1)
    {
        int tensionNumber = WrapSymbolNumber(pendingIndex - 1);
        if (avoidNumber != -1 && tensionNumber == avoidNumber)
        {
            tensionNumber = WrapSymbolNumber(pendingIndex + 1);
        }

        int converted = (symbolCount + 1) - tensionNumber;
        targetIndex = converted % symbolCount;
        isStopping = true;
        isTempStopping = true; // ← TempStop由来はtrue
        mustFullRotation = false;
        rotationCount = 0f;
        IsTempStopped = false;
        stopDecelerationMultiplier = 1f;

        float remaining = currentIndex - targetIndex;
        if (remaining <= 0) remaining += symbolCount;
        if (remaining < 1.5f) mustFullRotation = true;

        Debug.Log($"[TempStop] pendingIndex:{pendingIndex} avoidNumber:{avoidNumber} tensionNumber:{tensionNumber} targetIndex:{targetIndex} currentIndex:{currentIndex:F2} remaining:{remaining:F2} mustFull:{mustFullRotation}");
    }

    // フィールドに追加
    bool mustFullRotation = false;
    float rotationCount = 0f; // 何周したか

    // StartSpinにリセット追加
    public void StartSpin()
    {
        isSpinning = true;
        isStopping = false;
        IsTempStopped = false;
        mustFullRotation = false;
        rotationCount = 0f;
        currentSpeed = speed;
        // ...
    }

    public IEnumerator GacoGaco(int winIndex, int count = 3, float gacoSpeed = 0.08f)
    {
        Debug.Log($"[GacoGaco] 開始 IsTempStopped:{IsTempStopped} currentIndex:{currentIndex:F2} winIndex:{winIndex}");
        if (!IsTempStopped) yield break;

        float tempPos = currentIndex;
        float winPos = (symbolCount + 1) - winIndex;

        // winPosへの移動は「回転方向（マイナス方向）」に合わせる
        // tempPos → winPos が回転方向と逆なら1周分ずらす
        if (winPos >= tempPos) winPos -= symbolCount;

        Debug.Log($"[GacoGaco] tempPos:{tempPos:F2} winPos:{winPos:F2}");

        for (int i = 0; i < count; i++)
        {
            // 当たり図柄方向へ（マイナス方向）
            float elapsed = 0f;
            while (elapsed < gacoSpeed)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / gacoSpeed);
                currentIndex = Mathf.Lerp(tempPos, winPos, t);
                if (currentIndex < 0) currentIndex += symbolCount;
                ApplyPosition();
                yield return null;
            }

            // 戻る
            elapsed = 0f;
            while (elapsed < gacoSpeed)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / gacoSpeed);
                currentIndex = Mathf.Lerp(winPos, tempPos, t);
                if (currentIndex < 0) currentIndex += symbolCount;
                ApplyPosition();
                yield return null;
            }
        }

        currentIndex = tempPos;
        ApplyPosition();
    }
    // ③ 当たり図柄に確定
    public void ConfirmWin(int winIndex)
    {
        if (gacogacoCoroutine != null)
        {
            StopCoroutine(gacogacoCoroutine);
            gacogacoCoroutine = null;
        }

        int converted = (symbolCount + 1) - winIndex;
        currentIndex = converted;
        IsTempStopped = false;
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;
        ApplyPosition();
    }

    // ④ 外れ図柄に確定（仮停止位置のまま止める）
    public void ConfirmLose(int loseIndex)
    {
        if (gacogacoCoroutine != null)
        {
            StopCoroutine(gacogacoCoroutine);
            gacogacoCoroutine = null;
        }

        int converted = (symbolCount + 1) - loseIndex;
        currentIndex = converted % symbolCount;
        IsTempStopped = false;
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;
        ApplyPosition();
    }

    public void ForceStop(int index)
    {
        targetIndex = (symbolCount + 1) - index;
        currentIndex = targetIndex;
        isSpinning = false;
        isStopping = false;
        IsTempStopped = false;
        currentSpeed = 0f;
        ApplyPosition();
    }

    void Update()
    {
        // メニュー・シナリオ中などゲームが一時停止している間は、
        // ガード漏れがあっても見た目が勝手に動かないようここで確実に止める
        if (gamePause != null && gamePause.isPaused) return;

        if (!isSpinning)
        {
            ApplyPosition();
            return;
        }

        // IsTempStopped中はUpdateで動かさない（TempStop専用）
        if (IsTempStopped) return;

        if (!isStopping)
        {
            currentIndex -= currentSpeed * Time.deltaTime;
            if (currentIndex < 0) currentIndex += symbolCount;
        }
        else
        {
            float remaining = currentIndex - targetIndex;
            if (remaining <= 0) remaining += symbolCount;

            if (mustFullRotation)
            {
                rotationCount += currentSpeed * Time.deltaTime;
                if (rotationCount >= symbolCount)
                {
                    mustFullRotation = false;
                    rotationCount = 0f;
                }
            }

            float decelDistance = 2f * stopDecelerationMultiplier;
            if (!mustFullRotation && remaining < decelDistance)
            {
                currentSpeed = Mathf.Lerp(0.5f, speed, remaining / decelDistance);
            }

            float step = currentSpeed * Time.deltaTime;

            if (!mustFullRotation && remaining <= step)
            {
                currentIndex = targetIndex;
                currentSpeed = 0f;

                if (isTempStopping) // TempStop由来かどうかで分岐
                {
                    isStopping = false;
                    isTempStopping = false;
                    IsTempStopped = true; // TempStop完了
                }
                else
                {
                    isStopping = false;
                    isSpinning = false; // 通常StopSpin完了
                }
            }
            else
            {
                currentIndex -= step;
                if (currentIndex < 0) currentIndex += symbolCount;
            }
        }

        ApplyPosition();
    }
}