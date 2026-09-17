using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackLogButton : MonoBehaviour
{
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト

    [SerializeField] private GameObject backLog;            // 会話履歴オブジェクト
    [SerializeField] private GameObject backLogBtn;         // 会話履歴表示ボタン
    [SerializeField] private GameObject closeBtn;           // 会話履歴閉じるボタン
    [SerializeField] private GameObject textBtn;            // テキストボタン(ストーリー中もBackLogBtnにNavigationできるように)
    [SerializeField] private Button twoBtn;                 // 2個の選択肢ボタン
    [SerializeField] private Button threeBtn;               // 3個の選択肢ボタン

    public bool isBackLog;  // バックログが表示されているか判定
    public bool isClick;    // 閉じるボタンが押されたか判定

    void Start()
    {
        backLog.SetActive(false);   // バックログ非表示
        EventSystem.current.SetSelectedGameObject(textBtn); // テキストボタンを最初に選択
    }

    /// <summary>
    /// バックログボタン押下時の処理
    /// </summary>
    public void OnBackLogBtn()
    {
        // 会話履歴ログ表示、closeボタンを最初に選択
        SetGameObject(true, closeBtn);
    }

    /// <summary>
    /// 閉じるボタン押下時の処理
    /// </summary>
    public void OnCloseBtn()
    {
        isClick = true;

        // 会話履歴ログ非表示、textボタンを最初に選択
        SetGameObject(false, textBtn);

        // 選択肢が出ているとき
        if (chooseManager.isEvent == true)
        {
            EventSystem.current.SetSelectedGameObject(backLogBtn);  // バックログボタンを最初に選択
        }
    }
    /// <summary>
    /// 会話履歴ログの表示・非表示、最初に選択するボタンを設定
    /// </summary>
    /// <param name="log"></param>
    /// <param name="button"></param>
    private void SetGameObject(bool log, GameObject button)
    {
        isBackLog = log;
        backLog.SetActive(log);    // バックログ表示
        EventSystem.current.SetSelectedGameObject(button);    // 閉じるボタンを最初に選択
    }

    /// <summary>
    /// ボタンの遷移先を設定
    /// </summary>
    public void NavigationButton()
    {
        // Buttonコンポーネントを取得
        Selectable backLogSele = backLogBtn.GetComponent<Selectable>();
        // Navigation構造体を取得
        Navigation backLogNav = backLogSele.navigation;

        // バックログボタンからの移動先ボタンを設定
        if (chooseManager.isTwoBtn == true) backLogNav.selectOnLeft = twoBtn;
        else if (chooseManager.isTwoBtn == false) backLogNav.selectOnLeft = threeBtn;

        // 変更をコンポーネントに反映
        backLogSele.navigation = backLogNav;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) OnBackLogBtn();

        if (Input.GetKeyDown(KeyCode.X)) OnCloseBtn();
    }
}
