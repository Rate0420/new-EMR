using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float fadeTime;

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
        }
    }

    private Coroutine bgmCoroutine;

    /// <summary>
    /// BGMの変更
    /// </summary>
    public void BGMChange(int bgmNo)
    {
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
        }

        bgmCoroutine = StartCoroutine(ChangeBGM(bgmNo));
    }

    /// <summary>
    /// BGMをフェードさせながら変更
    /// </summary>
    private IEnumerator ChangeBGM(int bgmNo)
    {
        if (bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        bgmSource.clip = bgmClips[bgmNo];
        bgmSource.Play();

        yield return StartCoroutine(FadeIn());
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    private IEnumerator FadeOut()
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        bgmSource.volume = 0;
        bgmSource.Stop();
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    private IEnumerator FadeIn()
    {
        float targetVolume = 1.0f;

        bgmSource.volume = 0;

        while (bgmSource.volume < targetVolume)
        {
            bgmSource.volume += targetVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
}
