using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float fadeTime = 1.0f;     // フェード時間

    private Coroutine bgmCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        float volume = PlayerPrefs.GetFloat("BGMVolume", 5);
        bgmSource.volume = volume / 10f;
    }

    /// <summary>
    /// BGMを変更
    /// </summary>
    public void BGMChange(int bgmNo)
    {
        if (bgmNo < 0 || bgmNo >= bgmClips.Length)
        {
            Debug.LogError("BGMが登録されていません : " + bgmNo);
            return;
        }

        if (bgmSource.clip == bgmClips[bgmNo] &&　bgmSource.isPlaying)
        {
            return;
        }

        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
        }

        bgmCoroutine = StartCoroutine(ChangeBGM(bgmNo));
    }

    /// <summary>
    /// BGMの変更処理
    /// </summary>
    private IEnumerator ChangeBGM(int bgmNo)
    {
        // フェードアウト
        if (bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        bgmSource.clip = bgmClips[bgmNo];
        bgmSource.Play();

        // フェードイン
        yield return StartCoroutine(FadeIn());

        bgmCoroutine = null;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    private IEnumerator FadeOut()
    {
        float startVolume = bgmSource.volume;

        if (fadeTime <= 0)
        {
            bgmSource.volume = 0;
        }
        else
        {
            float time = 0;

            while (time < fadeTime)
            {
                time += Time.deltaTime;

                bgmSource.volume =
                    Mathf.Lerp(startVolume, 0, time / fadeTime);

                yield return null;
            }
        }

        bgmSource.volume = 0;
        bgmSource.Stop();
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    private IEnumerator FadeIn()
    {
        float targetVolume = PlayerPrefs.GetFloat("BGMVolume", 5) / 10f;

        bgmSource.volume = 0;

        if (fadeTime <= 0)
        {
            bgmSource.volume = targetVolume;
            yield break;
        }

        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            bgmSource.volume =
                Mathf.Lerp(0, targetVolume, time / fadeTime);

            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    /// <summary>
    /// BGMの音量変更
    /// </summary>
    public void SetVolume(float value)
    {
        bgmSource.volume = value / 10f;
    }
}