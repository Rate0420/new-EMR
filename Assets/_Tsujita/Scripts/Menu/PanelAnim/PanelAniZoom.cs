using UnityEngine;
using Cysharp.Threading.Tasks;

public class PanelAniZoom : MonoBehaviour
{
    // メニューのパネル切り替えアニメーション
    [SerializeField] private CharacterImage characterImage;

    [SerializeField] private GameObject menuPanel;  // メニューパネル

    [SerializeField] private RectTransform panel;   // 暗転１
    [SerializeField] private RectTransform image;   // 暗転２(キャラ)

    // 暗転用
    [SerializeField] private Vector2 zoomInPos = new Vector2(0 , 0);
    [SerializeField] private Vector2 zoomOutPos = Vector2.zero;
    // 暗転用(キャラ)
    [SerializeField] private Vector2 _zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 _zoomOutPos = Vector2.zero;

    [SerializeField] private float duration;    // 暗転１の暗転時間
    [SerializeField] private float _duration;   // 暗転２の暗転時間

    [SerializeField] GameObject[] bOImage;      // 暗転用画像

    [SerializeField] private float wSF; // シーン切り替えの間隔

    private RectTransform targetPanel;  // 現在暗転させているもの
    private float targerDuration;       // 現在設定されている暗転時間

    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject shopPanel;

    public bool isGameScene = false;

    /// <summary>
    /// メニューパネルの表示切替
    /// </summary>
    public void MenuPanelChange()
    {
        bOImage[0].SetActive(true);
        bOImage[1].SetActive(true);
        targetPanel = panel;
        targerDuration = duration;
        CloseAnimation().Forget();
    }

    /// <summary>
    /// 暗転→暗転解除
    /// </summary>
    private async UniTask CloseAnimation()
    {
        SEManagerMenu.Instance.SE_Shrink();
        await ScaleAnimation(zoomInPos, zoomOutPos);

        // 暗転開始
        targetPanel = image;
        targerDuration = _duration;
        await ScaleAnimation(_zoomOutPos, _zoomInPos);

        if(isGameScene)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            isGameScene = false;
        }
        // ショップボタンを押したか
        else if (menuManager.isShopFlg)
        {
            if (shopPanel.activeSelf)
            {
                shopPanel.SetActive(false);
                menuManager.isShopFlg = false;
            }
            else
                shopPanel.SetActive(true);
        }
        else
            menuPanel.SetActive(!menuPanel.activeSelf);

        await UniTask.WaitForSeconds(wSF);

        // ショップ以外の場合、キャラ画像の変更
        if (!shopPanel.activeSelf)
            characterImage.MainImageChange();

        // 暗転解除
        SEManagerMenu.Instance.SE_Enlarge();
        await ScaleAnimation(_zoomInPos, _zoomOutPos);
        targetPanel = panel;
        targerDuration = duration;
        await ScaleAnimation(zoomOutPos, zoomInPos);
        bOImage[0].SetActive(false);
        bOImage[1].SetActive(false);
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
