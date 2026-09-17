using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public CharactorType currentRoute;

    [SerializeField] private CharacterDatabase database;
    [SerializeField] private CharacterImage characterImage;
    [SerializeField] private StatusGet statusGet;

    [Header("メインのボタンとパネル")]
    [SerializeField] private GameObject[] menuButtons;  // ボタン
    [SerializeField] private GameObject[] menuPanels;   // パネル
    [SerializeField] private SceneChanger changer;

    [Header("シーン切り替え関連")]
    [SerializeField] private Image backButton;  // 戻るボタン
    [SerializeField] private GameObject buckButton2;
    [SerializeField] private GameObject sceneChangeImage;
    [SerializeField] private GameObject sceneCharaImage;

    private PanelAni[] panelAnis;       // パネルアニメーション用
    private PanelAniZoom zoomPanelAni;  // 拡大縮小用アニメーション

    private const int IsLikeabilityPanelIndex = 5;  // 好感度パネル番号
    private const int IsShopPanelIndex = 3;         // ショップパネル番号
    private const int IsSavePanelIndex = 6;         // 好感度パネル番号
    private const int IsVolumePanelIndex = 7;       // 音量変更パネル番号
    private const int IsTitleBuckIndex = 8;         // タイトルへパネル番号

    // ボタンの色を変更
    private Color DefaultColor =        // ボタンの初期色
        new Color32(255, 255, 255, 255);
    private Color SelectColor =         // ボタン選択中色
        new Color32(255, 45, 235, 255);

    // フラグ関連
    public bool isPanelFlg;
    public bool isMenuFlg;  // メニューパネル表示フラグ
    public bool isLBOpen;   // 追加パネル表示用フラグ
    public bool isShopFlg;

    public int nowPanelNo = -1;     // 現在開いているパネル番号
    public int currentPanelNo = -1; // 2重パネル用

    private void Awake()
    {
        Instance = this;
        panelAnis = new PanelAni[menuPanels.Length];

        for (int i = 0; i < menuPanels.Length; i++)
            panelAnis[i] =
                menuPanels[i].GetComponent<PanelAni>();

        zoomPanelAni = sceneChangeImage.GetComponent<PanelAniZoom>();
        sceneChangeImage.SetActive(false);
        isPanelFlg = false;
        isShopFlg = false;
    }

    private void Start()
    {
        MenuStart();
    }

    /// <summary>
    /// メニューパネルを開くと実行
    /// </summary>
    public void MenuStart()
    {
        buckButton2.SetActive(false);
        isMenuFlg = false;
        isLBOpen = false;
        characterImage.Route();
        statusGet.SetStatus();
        characterImage.MainImageChange();
    }

    /// <summary>
    /// 初期ボタンのパネル切り替え
    /// </summary>
    public void OnMeunButtons(int buttonNo)
    {
        // メニューパネル非表示
        if (buttonNo == 0)
            PanelReset();
        else if (buttonNo == IsShopPanelIndex)
        {
            sceneChangeImage.SetActive(true);
            isShopFlg = true;
            zoomPanelAni.MenuPanelChange();
        }
        else
        {
            // 1:ステータスパネル 2:キャラ解説パネル 3:セーブパネル
            // 4:音量設定パネル 5:好感度一覧
            PanelSet(buttonNo);
            BuckButtonChange();
        }
    }

    /// <summary>
    /// パネルの非表示
    /// </summary>
    private void PanelReset()
    {
        if (isLBOpen)    // 好感度パネル
        {
            SEManagerMenu.Instance.SE_Slide();
            panelAnis[currentPanelNo].Close();
            isLBOpen = false;
            currentPanelNo = -1;
        }
        else if (isShopFlg)
        {
            zoomPanelAni.MenuPanelChange();
        }
        else if (isMenuFlg) // パネル全般の切り替え
        {
            SEManagerMenu.Instance.SE_Slide();
            panelAnis[nowPanelNo].Close();
            nowPanelNo = -1;

            backButton.color = DefaultColor;
            buckButton2.SetActive(false);
            isMenuFlg = false;
        }
        else    // メニューパネルの非表示
        {
            sceneChangeImage.SetActive(true);
            zoomPanelAni.MenuPanelChange();
            isPanelFlg = false;

            changer.EndMenu();
        }
    }

    /// <summary>
    /// 初期ボタンのパネル関連
    /// </summary>
    /// <param name="buttonNo"></param>
    private void PanelSet(int buttonNo)
    {
        SEManagerMenu.Instance.SE_Slide();
        if (!isMenuFlg)
        {
            nowPanelNo = buttonNo;
            panelAnis[buttonNo].Open();
            isMenuFlg = true;
        }
        // 好感度・セーブ・音量設定パネル
        else if (buttonNo == IsLikeabilityPanelIndex || 
                 buttonNo == IsSavePanelIndex || 
                 buttonNo == IsVolumePanelIndex　|| 
                 buttonNo == IsTitleBuckIndex)
        {
            currentPanelNo = buttonNo;
            panelAnis[buttonNo].Open();
            isLBOpen = true;
        }
    }

    /// <summary>
    /// 戻るボタンの色変更
    /// </summary>
    private void BuckButtonChange()
    {
        backButton.color = SelectColor;
        buckButton2.SetActive(true);
    }
}
