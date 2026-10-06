using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class TitleFade : MonoBehaviour
{
    public static TitleFade Instance { get; private set; }

    [SerializeField] private RectTransform panel;   // 暗転１
    [SerializeField] private RectTransform image;   // 暗転２(キャラ)

    // 暗転用
    [SerializeField] private Vector2 zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 zoomOutPos = Vector2.zero;
    // 暗転用(キャラ)
    [SerializeField] private Vector2 _zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 _zoomOutPos = Vector2.zero;

    [SerializeField] private float duration;    // 暗転１の暗転時間
    [SerializeField] private float _duration;   // 暗転２の暗転時間
    [SerializeField] private GameObject fadePanel;

    [SerializeField] GameObject[] bOImage;      // 暗転用画像
    [SerializeField] private float wSF;         // シーン切り替えの間隔

    [SerializeField] private string titleSceneName; // タイトルシーン名
    [SerializeField] private string gameSceneName;  // ゲームシーン名
    [SerializeField] private string charaScene;     // キャラ選択シーン
    [SerializeField] private string prologueScene;   // プロローグシーン

    private RectTransform targetPanel;  // 現在暗転させているもの
    private float targerDuration;       // 現在設定されている暗転時間

    private string currentScene;

    [SerializeField] private Image c_Image;

    private bool isChara;

    private bool isChangingScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);   // 既に存在するので削除
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fadePanel.SetActive(false);

        isChara = false;
    }

    public void ImageChange(Sprite sprite)
    {
        c_Image.sprite = sprite;
    }

    /// <summary>
    /// 暗転＋シーン遷移
    /// </summary>
    public void SceneChangeAni()
    {
        if (isChangingScene)
            return;

        isChangingScene = true;

        fadePanel.SetActive(true);
        targetPanel = panel;
        targerDuration = duration;
        CloseAnimation().Forget();
    }

    /// <summary>
    /// 暗転全般
    /// </summary>
    private async UniTask CloseAnimation()
    {
        await ScaleAnimation(zoomInPos, zoomOutPos);
        targetPanel = image;
        targerDuration = _duration;
        await ScaleAnimation(_zoomOutPos, _zoomInPos);
        await UniTask.WaitForSeconds(wSF);

        // シーン遷移 タイトル⇔ゲームシーン
        currentScene = SceneManager.GetActiveScene().name;
        if (!isChara)
        {
            // 一度だけキャラ選択画面に移行
            isChara = true;
            await SceneManager.LoadSceneAsync(charaScene);
        }
        else if (currentScene == charaScene)
        {
            await SceneManager.LoadSceneAsync(prologueScene);
            //BGMManager.Instance.BGMChange(3);
        }
        else if (currentScene == prologueScene)
        {
            await SceneManager.LoadSceneAsync(gameSceneName);
            BGMManager.Instance.BGMChange(3);
        }
        else if (currentScene == titleSceneName)
        {
            await SceneManager.LoadSceneAsync(gameSceneName);
            BGMManager.Instance.BGMChange(3);
        }
        else if (currentScene == gameSceneName)
        {
            await SceneManager.LoadSceneAsync(titleSceneName);
            BGMManager.Instance.BGMChange(0);
        }

        // 暗転解除
        targetPanel = image;
        targerDuration = _duration;

        await ScaleAnimation(_zoomInPos, _zoomOutPos);
        targetPanel = panel;
        targerDuration = duration;
        await ScaleAnimation(zoomOutPos, zoomInPos);
        await UniTask.WaitForSeconds(wSF);
        fadePanel.SetActive(false);
       
        isChangingScene = false;

    }

    private async UniTask ScaleAnimation(Vector3 start, Vector3 end)
    {
        float time = 0;

        while (time < targerDuration)
        {
            time += Time.deltaTime;
            float t = time / targerDuration;

            targetPanel.localScale = Vector3.Lerp(start, end, t);

            await UniTask.Yield();
        }

        targetPanel.localScale = end;
    }
}
