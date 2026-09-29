using UnityEngine;

public class SEManagerMenu : MonoBehaviour
{
    public static SEManagerMenu Instance { get; private set; }

    [SerializeField] public AudioSource seSource;      // SE用AudioSource
    [SerializeField] private AudioClip[] audioClips;   // SE音源

    public enum SEType
    {
        Decision = 0,       // 決定
        Back = 1,           // キャンセル
        Enlarge = 2,        // 拡大
        Shrink = 3,         // 縮小
        Slide = 4,          // スライド
        Tap = 5,            // タップ
        Buy = 6,            // 購入
        GetBall = 7,        // ボール取得
        GetCoin = 8,        // コイン取得
        Chukka = 9 ,        // チャッカ―反応
        Reach = 10,         // リーチ時
        SlotStop = 11,      // スロット停止
        CoinLaunch = 12,    // コイン発射
        JingleHit = 13,     // 当たり時ジングル
    }

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
    /// SE再生
    /// </summary>
    public void SEPlays(SEType seType)
    {
        int index = (int)seType;

        if (seSource == null || audioClips == null || index >= audioClips.Length || audioClips[index] == null)
        {
            Debug.LogWarning($"SE「{seType}」が設定されていません。");
            return;
        }

        seSource.Stop();
        seSource.PlayOneShot(audioClips[index]);
    }
}
