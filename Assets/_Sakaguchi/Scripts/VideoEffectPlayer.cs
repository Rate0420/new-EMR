using UnityEngine;
using System.Collections;
using UnityEngine.Video;


public class VideoEffectPlayer : MonoBehaviour
{
    VideoPlayer videoPlayer;
    Renderer videoRenderer;
    [SerializeField] float endlength = 0.5f; // 終了前にフェードアウトする時間

    public string[] videoPaths;
    public VideoClip[] videoClips;

    Material mat;
    Color color;
    const float PrepareTimeout = 5f;
    const float PlaybackTimeoutPadding = 2f;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoRenderer = GetComponent<Renderer>();

        mat = videoRenderer.material;
        color = mat.color;
    }

    // 外部からコルーチンで呼び出す用
    public IEnumerator PlayVideoCoroutine(int id, float playbackSpeed = 1f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        bool isPrepared = false;
        bool isFinished = false;
        videoPlayer.Stop();
        videoPlayer.clip = null;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClips[id];
        videoPlayer.playbackSpeed = playbackSpeed;

        videoRenderer.enabled = false;

        void OnPrepared(VideoPlayer vp)
        {
            isPrepared = true;
        }

        void OnFinished(VideoPlayer vp)
        {
            isFinished = true;
        }

        // 準備完了イベント
        videoPlayer.prepareCompleted += OnPrepared;

        // 再生終了イベント
        videoPlayer.loopPointReached += OnFinished;

        videoPlayer.Prepare();

        // 準備完了後
        yield return WaitForPrepare(() => isPrepared, id);
        videoPlayer.prepareCompleted -= OnPrepared;
        if (!isPrepared)
        {
            videoPlayer.loopPointReached -= OnFinished;
            yield break;
        }

        // ★ここでリセット
        color.a = 1f;
        mat.color = color;

        videoRenderer.enabled = true;
        videoPlayer.Play();

        // 再生終了待ち
        yield return WaitForVideoEnd(() => isFinished, id);
        videoPlayer.loopPointReached -= OnFinished;

        // フェードアウト
        yield return StartCoroutine(FadeOut(1.0f)); // 1秒でフェード

        videoRenderer.enabled = false;

    }

    public IEnumerator PlayVideoNoFadeCoroutine(int id, float playbackSpeed = 1f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        Debug.Log($"[VideoEffectPlayer] PlayVideoNoFadeCoroutine: {videoPaths[id]}");

        bool isPrepared = false;

        videoPlayer.Stop();
        videoPlayer.clip = null;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClips[id];
        videoPlayer.playbackSpeed = playbackSpeed;

        videoRenderer.enabled = false;

        void OnPrepared(VideoPlayer vp)
        {
            isPrepared = true;
        }

        videoPlayer.prepareCompleted += OnPrepared;

        videoPlayer.Prepare();

        yield return WaitForPrepare(() => isPrepared, id);
        videoPlayer.prepareCompleted -= OnPrepared;
        if (!isPrepared)
        {
            yield break;
        }

        color.a = 1f;
        mat.color = color;

        videoRenderer.enabled = true;
        videoPlayer.Play();

        // こっちは最後まで再生
        yield return WaitForVideoEnd(() => !videoPlayer.isPlaying, id);

        videoRenderer.enabled = false;
    }

    public IEnumerator WaitEarlyEndCoroutine(float earlyEndTime)
    {
        // length取れるまで待つ（重要）
        float prepareElapsed = 0f;
        while (!videoPlayer.isPrepared && prepareElapsed < PrepareTimeout)
        {
            prepareElapsed += Time.deltaTime;
            yield return null;
        }

        if (!videoPlayer.isPrepared)
        {
            Debug.LogWarning($"{nameof(VideoEffectPlayer)}: Video was not prepared within {PrepareTimeout:0.0} seconds.");
            yield break;
        }

        double endTime = videoPlayer.length - earlyEndTime;
        if (endTime < 0) endTime = 0;

        float timeout = GetPlaybackTimeout(endTime);
        float elapsed = 0f;
        while (videoPlayer.time < endTime && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (videoPlayer.time < endTime)
        {
            Debug.LogWarning($"{nameof(VideoEffectPlayer)}: Early end wait timed out at {videoPlayer.time:0.00}/{endTime:0.00} seconds.");
        }
    }

    public IEnumerator PlayVideoFadeInOut(int id, float playbackSpeed = 1f, float fadeintime = 0f, float fadeouttime = 0f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        videoPlayer.Stop();
        videoPlayer.clip = null;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClips[id];
        videoPlayer.playbackSpeed = playbackSpeed;
        videoRenderer.enabled = false;

        bool isPrepared = false;

        void OnPrepared(VideoPlayer vp)
        {
            isPrepared = true;
        }

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();

        yield return WaitForPrepare(() => isPrepared, id);

        videoPlayer.prepareCompleted -= OnPrepared; // ★超重要（イベント解除）
        if (!isPrepared)
        {
            yield break;
        }

        // 最初から透明にする
        color.a = 0f;
        mat.color = color;

        videoRenderer.enabled = true;

        videoPlayer.Play();

        // ★フェードイン（0ならスキップ）
        if (fadeintime > 0f)
            yield return StartCoroutine(FadeIn(fadeintime));
        else
        {
            color.a = 1f;
            mat.color = color;
        }

        // ★length安定待ち（1フレーム）
        yield return null;

        double endTime = videoPlayer.length - fadeouttime;
        if (endTime < 0) endTime = 0;

        float timeout = GetPlaybackTimeout(endTime);
        float elapsed = 0f;
        while (videoPlayer.time < endTime && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (videoPlayer.time < endTime)
        {
            Debug.LogWarning($"{nameof(VideoEffectPlayer)}: Fade video wait timed out at {videoPlayer.time:0.00}/{endTime:0.00} seconds.");
        }

        // ★フェードアウト
        if (fadeouttime > 0f)
            yield return StartCoroutine(FadeOut(fadeouttime));
        else
        {
            color.a = 0f;
            mat.color = color;
        }

        videoPlayer.Stop();
        videoRenderer.enabled = false;
    }

    IEnumerator WaitForPrepare(System.Func<bool> isPreparedFunc, int id)
    {
        float elapsed = 0f;
        while (!isPreparedFunc() && elapsed < PrepareTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!isPreparedFunc())
        {
            Debug.LogWarning($"{nameof(VideoEffectPlayer)}: Prepare timed out for video id {id}.");
        }
    }

    IEnumerator WaitForVideoEnd(System.Func<bool> isEndedFunc, int id)
    {
        float timeout = GetPlaybackTimeout(videoPlayer.length);
        float elapsed = 0f;
        while (!isEndedFunc() && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!isEndedFunc())
        {
            Debug.LogWarning($"{nameof(VideoEffectPlayer)}: Playback timed out for video id {id} at {videoPlayer.time:0.00}/{videoPlayer.length:0.00} seconds.");
            videoPlayer.Stop();
        }
    }

    float GetPlaybackTimeout(double targetTime)
    {
        if (double.IsNaN(targetTime) || double.IsInfinity(targetTime) || targetTime <= 0)
        {
            return 10f;
        }

        float speed = Mathf.Max(0.01f, Mathf.Abs(videoPlayer.playbackSpeed));
        return Mathf.Max(3f, (float)targetTime / speed + PlaybackTimeoutPadding);
    }
    IEnumerator FadeIn(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / time);
            color.a = a;
            mat.color = color;
            yield return null;
        }

        color.a = 1f;
        mat.color = color;
    }

    IEnumerator FadeOut(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / time);
            color.a = a;
            mat.color = color;
            yield return null;
        }

        color.a = 0f;
        mat.color = color;
    }
}
