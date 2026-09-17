using UnityEngine;

public class SEManagerMenu : MonoBehaviour
{
    public static SEManagerMenu Instance { get; private set; }

    [SerializeField] public AudioSource seSource;      // SE用AudioSource
    [SerializeField] private AudioClip[] audioClips;    // SE音源

    private int clipNo; // 再生する音源番号

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 決定
    /// </summary>
    public void SE_Decision()
    {
        clipNo = 0;
        SEPlays();
    }

    /// <summary>
    /// キャンセル
    /// </summary>
    public void SE_Back() 
    {
        clipNo = 1;
        SEPlays();
    }

    /// <summary>
    /// 拡大
    /// </summary>
    public void SE_Enlarge()
    {
        clipNo = 2;
        SEPlays();
    }

    /// <summary>
    /// 縮小
    /// </summary>
    public void SE_Shrink()
    {
        clipNo = 3;
        SEPlays();
    }

    /// <summary>
    /// スライド
    /// </summary>
    public void SE_Slide()
    {
        clipNo = 4;
        SEPlays();
    }

    /// <summary>
    /// タップ
    /// </summary>
    public void SE_Tap()
    {
        clipNo = 5;
        SEPlays();
    }

    /// <summary>
    /// 購入音
    /// </summary>
    private void SE_Buy()
    {
        clipNo = 6;
        SEPlays();
    }


    /// <summary>
    /// SE再生
    /// </summary>
    private void SEPlays()
    {
        seSource.Stop();
        seSource.clip = audioClips[clipNo];
        seSource.PlayOneShot(seSource.clip);
    }
}
