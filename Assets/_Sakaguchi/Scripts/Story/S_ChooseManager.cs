using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// S_StoryData の ChoiceEntry に基づき選択肢UIを表示し、
/// 結果を WriteText と S_AffinityManager に渡す。
/// </summary>
public class S_ChooseManager : MonoBehaviour
{
    [SerializeField] private S_StoryData storyData;
    [SerializeField] private S_WriteText writeText;
    // S_AffinityManager はシングルトンのため参照はInstance経由。
    // インスペクタアサイン不要。

    [Header("2択UI")]
    [SerializeField] private CanvasGroup twoChoiceGroup;
    [SerializeField] private List<Button> twoChoiceBtns;

    [Header("3択UI")]
    [SerializeField] private CanvasGroup threeChoiceGroup;
    [SerializeField] private List<Button> threeChoiceBtns;

    [Header("ミニイベント用リソースUI")]
    [Tooltip("ミニイベント時に選択肢と一緒に表示する所持金などのUIオブジェクト")]
    [SerializeField] private GameObject resourceUI;

    public bool IsShowingChoices { get; private set; } = false;

    /// <summary> 現在表示中の選択肢数（BackLogManager のNavigation設定に使用）</summary>
    public int CurrentChoiceCount { get; private set; } = 0;

    private int currentEntryIndex = -1;

    // 選択肢表示中にフォーカスを維持するための参照
    private GameObject currentFirstBtn = null;

    // ----------------------------------------------------------------

    private void Awake()
    {
        // Start より早い Awake で初期化することで、WriteText の Start から
        // ShowChoices が呼ばれる前に storyData が確実にセットされる
        storyData = S_DontDestroyStory.instance.story;
        HideAll();
    }

    private void Update()
    {
        // 選択肢表示中、マウス移動などで選択が外れた場合に再セットする。
        // UnityのStandaloneInputModuleはマウス入力があると選択をnullにリセットする
        // 仕様があるため、毎フレーム監視して選択が外れたら復元する。
        if (IsShowingChoices && currentFirstBtn != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(currentFirstBtn);
        }

        if (storyData.isMiniEvent && IsShowingChoices && resourceUI != null)
        {
            if (Input.GetKeyDown(KeyCode.H))
                resourceUI.SetActive(!resourceUI.activeSelf);
        }
    }

    // ----------------------------------------------------------------

    /// <summary> 指定エントリの選択肢を表示する（WriteText から呼ばれる）</summary>
    public void ShowChoices(int entryIndex)
    {
        var entry = storyData.Get(entryIndex);
        if (!entry.HasChoices) return;

        currentEntryIndex = entryIndex;
        IsShowingChoices = true;

        // ミニイベントなら選択肢と同時にリソースUIを表示する
        if (storyData.isMiniEvent && resourceUI != null)
            resourceUI.SetActive(true);

        int count = entry.choices.Count;
        CurrentChoiceCount = count;
        if (count == 2)
        {
            SetupButtons(twoChoiceBtns, entry.choices);
            Show(twoChoiceGroup, twoChoiceBtns[0].gameObject);
        }
        else if (count == 3)
        {
            SetupButtons(threeChoiceBtns, entry.choices);
            Show(threeChoiceGroup, threeChoiceBtns[0].gameObject);
        }
        else
        {
            Debug.LogWarning($"[ChooseManager] 選択肢数 {count} は未対応です（2か3にしてください）");
        }
    }

    /// <summary> ボタン押下時の処理（引数は0始まりのIndex）</summary>
    public void OnChoiceButtonPressed(int choiceIndex)
    {
        // 先に退避・リセットして JumpTo 内の ShowChoices 呼び出しと競合させない
        int resolvedEntry = currentEntryIndex;
        currentEntryIndex = -1;

        if (resolvedEntry < 0) return;

        var choices = storyData.Get(resolvedEntry).choices;
        if (choiceIndex < 0 || choiceIndex >= choices.Count) return;

        var choice = choices[choiceIndex];

        // キャラ別・ステータス別に変化量をまとめて適用
        S_AffinityManager.Instance?.ApplyDeltas(choice.statusDeltas);

        HideAll();
        IsShowingChoices = false;

        int nextIndex = choice.jumpToIndex >= 0 ? choice.jumpToIndex : resolvedEntry + 1;
        writeText.JumpTo(nextIndex);
    }

    // ----------------------------------------------------------------

    private void SetupButtons(List<Button> buttons, List<S_StoryData.ChoiceEntry> choices)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i >= choices.Count) continue;

            var label = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = choices[i].label;

            var newBtn = ReplaceButton(buttons[i]);
            buttons[i] = newBtn;

            // Navigation を Automatic にしてキーボード/ゲームパッドでの
            // ボタン間フォーカス移動を有効にする
            var nav = newBtn.navigation;
            nav.mode = Navigation.Mode.Automatic;
            newBtn.navigation = nav;

            int captured = i;
            newBtn.onClick.AddListener(() => OnChoiceButtonPressed(captured));
        }
    }

    /// <summary>
    /// Buttonコンポーネントを作り直してonClickリスナーを完全リセットする。
    /// RemoveAllListeners() はインスペクタ登録(Persistent)を消せないための対策。
    /// </summary>
    private Button ReplaceButton(Button original)
    {
        var go = original.gameObject;
        var colors = original.colors;
        var navigation = original.navigation;
        var transition = original.transition;
        var targetGraphic = original.targetGraphic;

        DestroyImmediate(original);  // Destroy は遅延削除のため AddComponent と競合する
        var newBtn = go.AddComponent<Button>();

        newBtn.colors = colors;
        newBtn.navigation = navigation;
        newBtn.transition = transition;
        newBtn.targetGraphic = targetGraphic;

        return newBtn;
    }

    private void Show(CanvasGroup group, GameObject firstSelected)
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
        currentFirstBtn = firstSelected;
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void HideAll()
    {
        currentFirstBtn = null;
        CurrentChoiceCount = 0;
        if (resourceUI != null) resourceUI.SetActive(false);
        Hide(twoChoiceGroup);
        Hide(threeChoiceGroup);
    }

    private void Hide(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}
