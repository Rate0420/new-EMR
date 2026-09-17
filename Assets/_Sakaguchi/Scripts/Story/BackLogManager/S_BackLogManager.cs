using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_BackLogManager : MonoBehaviour
{
    [SerializeField] private S_ChooseManager chooseManager;
    [SerializeField] private S_WriteText writeText;

    [SerializeField] private GameObject backLog;
    [SerializeField] private GameObject backLogBtn;
    [SerializeField] private GameObject closeBtn;
    [SerializeField] private GameObject textBtn;    // ストーリー中の通常フォーカス先

    // 2択・3択の先頭ボタン（NavigationのselectOnLeftに設定する）
    // S_ChooseManager の twoChoiceBtns/threeChoiceBtns の [0] に相当
    [SerializeField] private Button twoFirstBtn;
    [SerializeField] private Button threeFirstBtn;

    public bool isBackLog { get; private set; } = false;

    // ----------------------------------------------------------------

    private void Start()
    {
        backLog.SetActive(false);
        EventSystem.current.SetSelectedGameObject(textBtn);

        // Navigation は変化しないので Start で1回だけ設定する
        UpdateBackLogNavigation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) OnBackLogBtn(0);
        if (Input.GetKeyDown(KeyCode.X)) OnBackLogBtn(1);

        // 選択肢の表示状態が変わったときだけNavigation更新
        // （毎フレームGetComponentするのを避けるためフラグ監視）
        if (chooseManager.IsShowingChoices != _wasShowingChoices)
        {
            _wasShowingChoices = chooseManager.IsShowingChoices;
            UpdateBackLogNavigation();
        }
    }

    private bool _wasShowingChoices = false;

    // ----------------------------------------------------------------

    public void OnBackLogBtn(int btnNum)
    {
        // バックログを開く
        if (btnNum == 0)
        {
            isBackLog = true;
            backLog.SetActive(true);
            EventSystem.current.SetSelectedGameObject(closeBtn);
            // 開いた瞬間のクリック/キー入力がWriteTextに届かないよう
            // WriteText側のinputBlockedを1フレームONにする
            writeText.BlockInputOneFrame();
        }
        // バックログを閉じる
        else if (btnNum == 1)
        {
            isBackLog = false;
            backLog.SetActive(false);

            // 選択肢表示中ならバックログボタンへ、通常時はテキストボタンへ
            if (chooseManager.IsShowingChoices)
                EventSystem.current.SetSelectedGameObject(backLogBtn);
            else
                EventSystem.current.SetSelectedGameObject(textBtn);
        }
    }

    // ----------------------------------------------------------------

    /// <summary>
    /// バックログボタンのselectOnLeftを現在の選択肢状態に合わせて更新する。
    /// 選択肢が出ているときは先頭の選択肢ボタンへ、出ていないときは null。
    /// </summary>
    private void UpdateBackLogNavigation()
    {
        var sele = backLogBtn.GetComponent<Selectable>();
        var nav = sele.navigation;
        nav.mode = Navigation.Mode.Explicit;

        if (chooseManager.IsShowingChoices)
        {
            // 現在何択表示中かを ChooseManager のプロパティで取得
            nav.selectOnLeft = chooseManager.CurrentChoiceCount == 2 ? twoFirstBtn : threeFirstBtn;
        }
        else
        {
            nav.selectOnLeft = null;
        }

        sele.navigation = nav;
    }
}