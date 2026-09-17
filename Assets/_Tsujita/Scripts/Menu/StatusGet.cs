using EMR.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusGet : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private ItemDataBase itemDatabase;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private CharacterData[] datas;

    [Header("各キャラの好感度")]
    [SerializeField] private TextMeshProUGUI[] likeabilityTexts;     // 好感度一覧

    [Header("常時表示のステータス")]
    [SerializeField] private TextMeshProUGUI statusLikeability;     // ルートキャラ用好感度
    [SerializeField] private TextMeshProUGUI nowStoryText;  // ストーリー進行度
    [SerializeField] private TextMeshProUGUI miniStoryText; // ミニイベ進行度
    [SerializeField] private TextMeshProUGUI nowMedalText;  // 所持メダル
    [SerializeField] private Image cutinSprite; // 

    [Header("バフ関連")]
    public ItemData[] buffStats = new ItemData[3];    // セットされているバフ
    [SerializeField] private TextMeshProUGUI[] buffStatsTexts;          // バフ用テキスト
    [SerializeField] private TextMeshProUGUI[] buffNameTexts;           // バフ名テキスト
    [SerializeField] private TextMeshProUGUI[] buffLevelTexts;          // バフレベル用テキスト
    [SerializeField] private GameObject[] deleteButtons;                // バフ削除ボタン
    [SerializeField] private GameObject buffPanel;                      // 確認パネル
    [SerializeField] private int maxBuffLevel;

    public CharacterData characterData;
    private int nowStory;   // ストーリー進行度
    private int miniStory;  // ミニイベ進行度
    private int nowNo = -1; // バフ削除用

    public bool isBuff;     // バフスロットに空きがあるか

    /// <summary>
    /// 現在のステータスを反映
    /// </summary>
    public void SetStatus()
    {
        // ステータスタブへの反映
        // 各キャラの好感度
        for (int i = 0; i < datas.Length; i++)
        {
            likeabilityTexts[i].text = datas[i].likeability.ToString() + " / 100";
        }

        // 現在ルートのキャラ取得
        characterData =
            characterDatabase.GetCharacter(
                MenuManager.Instance.currentRoute
            );

        cutinSprite.sprite = characterData.cutinSprite;

        // ステータス取得　未実装
        nowStory = 1;
        miniStory = 1;

        // 画面左のステータス画面に反映
        nowStoryText.text = nowStory.ToString() + "/7";
        miniStoryText.text = miniStory.ToString() + "/8";
        statusLikeability.text = characterData.likeability.ToString();
        GameState.Instance.OwnedModel.OnCountChanged += UpdateUI;
        UpdateUI(GameState.Instance.OwnedModel.Count);
        UpdateBuffUI();
    }

    private void UpdateUI(int medal)
    {
        nowMedalText.text = medal + "枚";
    }

    /// <summary>
    /// バフUI更新
    /// </summary>
    private void UpdateBuffUI()
    {
        for (int i = 0; i < buffStats.Length; i++)
        {
            if (buffStats[i] != null)
            {
                buffNameTexts[i].text = buffStats[i].itemName;
                buffStatsTexts[i].text = buffStats[i].description;
                buffLevelTexts[i].text = "Lv." + buffStats[i].level.ToString();
                deleteButtons[i].SetActive(true);
            }
            else
            {
                buffNameTexts[i].text = "未設定";
                buffStatsTexts[i].text = ""; 
                buffLevelTexts[i].text = "";
                deleteButtons[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// バフ追加
    /// </summary>
    public bool AddBuff(ItemType itemType)
    {
        ItemData item = itemDatabase.GetCharacter(itemType);

        if (item == null)
        {
            Debug.LogWarning("アイテムが見つかりません");
            return false;
        }

        // 同じバフを所持しているか確認
        for (int i = 0; i < buffStats.Length; i++)
        {
            if (buffStats[i] != null && buffStats[i] == item)
            {
                if (buffStats[i].level == maxBuffLevel)
                {
                    Debug.Log("アイテムレベルが最大です。");
                    return false;
                }
                else
                {
                    buffStats[i].level++;

                    UpdateBuffUI();
                    Debug.Log($"{item.itemName} のレベルが {buffStats[i].level} になりました");
                    ItemTriggerEvents.OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // 空いているスロットを探す
        for (int i = 0; i < buffStats.Length; i++)
        {
            if (buffStats[i] == null)
            {
                buffStats[i] = item;
                buffStats[i].level = 1;
                UpdateBuffUI();
                ItemTriggerEvents.OnInventoryChanged?.Invoke();
                Debug.Log($"{item.itemName} のレベルが {buffStats[i].level} になりました");
                return true;
            }
        }

        isBuff = false;
        Debug.Log("バフ枠がいっぱいです");
        return false;
    }

    /// <summary>
    /// 指定スロットのバフ削除
    /// </summary>
    public void BuffDelete(int slotNo)
    {
        nowNo = slotNo;
        buffPanel.SetActive(true);
    }

    /// <summary>
    /// 所持しているバフの削除
    /// </summary>
    public void DeletChoice(int buttonNo)
    {
        switch(buttonNo)
        {
            case 0:
                if (nowNo < 0 || nowNo >= buffStats.Length)
                    return;

                buffStats[nowNo].level = 0;
                buffStats[nowNo] = null;
                nowNo = -1;
                UpdateBuffUI();
                buffPanel.SetActive(false);
                ItemTriggerEvents.OnInventoryChanged?.Invoke();
                break;
            case 1:
                nowNo = -1;
                buffPanel.SetActive(false);
                break;
        }
    }
}
