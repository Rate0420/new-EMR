using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class TitleFade : MonoBehaviour
{
    public static TitleFade Instance { get; private set; }

    [SerializeField] private RectTransform panel;   // ˆÃ“]‚P
    [SerializeField] private RectTransform image;   // ˆÃ“]‚Q(ƒLƒƒƒ‰)

    // ˆÃ“]—p
    [SerializeField] private Vector2 zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 zoomOutPos = Vector2.zero;
    // ˆÃ“]—p(ƒLƒƒƒ‰)
    [SerializeField] private Vector2 _zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 _zoomOutPos = Vector2.zero;

    [SerializeField] private float duration;    // ˆÃ“]‚P‚ÌˆÃ“]ŠÔ
    [SerializeField] private float _duration;   // ˆÃ“]‚Q‚ÌˆÃ“]ŠÔ
    [SerializeField] private GameObject fadePanel;

    [SerializeField] GameObject[] bOImage;      // ˆÃ“]—p‰æ‘œ
    [SerializeField] private float wSF;         // ƒV[ƒ“Ø‚è‘Ö‚¦‚ÌŠÔŠu

    [SerializeField] private string titleSceneName; // ƒ^ƒCƒgƒ‹ƒV[ƒ“–¼
    [SerializeField] private string gameSceneName;  // ƒQ[ƒ€ƒV[ƒ“–¼

    private RectTransform targetPanel;  // Œ»İˆÃ“]‚³‚¹‚Ä‚¢‚é‚à‚Ì
    private float targerDuration;       // Œ»İİ’è‚³‚ê‚Ä‚¢‚éˆÃ“]ŠÔ

    private string currentScene;

    [SerializeField] private Image c_Image;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);   // Šù‚É‘¶İ‚·‚é‚Ì‚Åíœ
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fadePanel.SetActive(false);
    }

    public void ImageChange(Sprite sprite)
    {
        c_Image.sprite = sprite;
    }

    /// <summary>
    /// ˆÃ“]{ƒV[ƒ“‘JˆÚ
    /// </summary>
    public void SceneChangeAni()
    {
        fadePanel.SetActive(true);
        targetPanel = panel;
        targerDuration = duration;
        CloseAnimation().Forget();
    }

    /// <summary>
    /// ˆÃ“]‘S”Ê
    /// </summary>
    private async UniTask CloseAnimation()
    {
        SEManagerMenu.Instance.SE_Shrink();
        await ScaleAnimation(zoomInPos, zoomOutPos);
        targetPanel = image;
        targerDuration = _duration;
        await ScaleAnimation(_zoomOutPos, _zoomInPos);
        await UniTask.WaitForSeconds(wSF);

        // ƒV[ƒ“‘JˆÚ ƒ^ƒCƒgƒ‹ÌƒQ[ƒ€ƒV[ƒ“
        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == titleSceneName)
            await SceneManager.LoadSceneAsync(gameSceneName);
        else if (currentScene == gameSceneName)
            await SceneManager.LoadSceneAsync(titleSceneName);

        // ˆÃ“]‰ğœ
        targetPanel = image;
        targerDuration = _duration;

        SEManagerMenu.Instance.SE_Enlarge();
        await ScaleAnimation(_zoomInPos, _zoomOutPos);
        targetPanel = panel;
        targerDuration = duration;
        await ScaleAnimation(zoomOutPos, zoomInPos);
        await UniTask.WaitForSeconds(wSF);
        fadePanel.SetActive(false);
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
