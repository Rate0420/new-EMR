using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public enum StageType
{
    NormalCity,
    NormalPark,
    NormalClassroom,
    NormalRoom,
    Special     // 確変用
}

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Renderer videoRenderer;

    [SerializeField] VideoClip normalCityVideos;
    [SerializeField] VideoClip normalParkVideos;
    [SerializeField] VideoClip normalClassroomVideos;
    [SerializeField] VideoClip normalRoomVideos;
    [SerializeField] VideoClip specialVideos;
    StageType currentStage = StageType.NormalCity;

    Coroutine loopCoroutine;

    Material mat;
    Color color;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoRenderer = GetComponent<Renderer>();

        mat = videoRenderer.material;
        color = mat.color;
    }

    public void ChangeStage(StageType newStage)
    {
        if (currentStage == newStage) return;

        currentStage = newStage;

        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        loopCoroutine = StartCoroutine(PlayStageLoop(newStage));
    }

    IEnumerator PlayStageLoop(StageType newStage)
    {
        yield return StartCoroutine(FadeOut(1f));
        VideoClip path = GetVideoList();

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = path;
        videoPlayer.isLooping = true;

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        // ① 先に黒にする
       

        videoPlayer.Play();

        yield return null; // ←これ神

        yield return FadeIn(1f);

        // ステージ変わるまで待機
        while (currentStage == newStage)
        {
            yield return null;
        }
    }

    VideoClip GetVideoList()
    {
        switch (currentStage)
        {
            case StageType.NormalCity:
                return normalCityVideos;
            case StageType.NormalPark:
                return normalParkVideos;
            case StageType.NormalClassroom:
                return normalClassroomVideos;
            case StageType.NormalRoom:
                return normalRoomVideos;
            case StageType.Special:
                return specialVideos;
        }
        return normalCityVideos; // デフォルト
    }

    IEnumerator PlayLoopVideo(string path)
    {
        yield return StartCoroutine(FadeOut(0.5f));

        videoPlayer.url = path;
        videoPlayer.isLooping = false;

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();

        yield return StartCoroutine(FadeIn(0.5f));

        // ★ここが抜けてた
        yield return new WaitUntil(() => videoPlayer.isPlaying);

        // 再生終了待ち
        yield return new WaitUntil(() => !videoPlayer.isPlaying);
    }

    IEnumerator FadeIn(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / time);

            color = new Color(a, a, a, 1f);
            mat.color = color;

            yield return null;
        }

        mat.color = Color.white;
    }

    IEnumerator FadeOut(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / time);

            color = new Color(a, a, a, 1f);
            mat.color = color;

            yield return null;
        }

        mat.color = Color.black;
    }
}