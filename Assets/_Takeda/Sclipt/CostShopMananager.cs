using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EMR.Core;

public class CostShopMananager : MonoBehaviour
{

    [Header("リロールコスト")]
    public int rerollCost = 50;

    [Header("全アイテム")]
    public ItemData[] allItems;

    [Header("現在表示中")]
    public ItemData[] currentItems = new ItemData[4];

    [Header("アイテムボタン")]
    public ItemButton[] itemButtons;

    [Header("詳細パネル")]
    public GameObject detailPanel;

    public Image detailIcon;
    public TMP_Text detailName;
    public TMP_Text detailDesc;
    public TMP_Text costText;

    [Header("アイテム種類")]
    public TMP_Text itemTypeText;

    [Header("所持メダル表示")]
    public TMP_Text medalText;

    [Header("購入ボタン")]
    public Button buyButton;

    private ItemData currentItem;

    // 無料リロール用変数
    public int freeRerollCount = 0;
    private int rerollCount = 0;
    void Start()
    {
        // 最初は詳細を隠す
        detailPanel.SetActive(false);

        // 購入ボタン無効
        buyButton.interactable = false;

        // 初回ショップ生成
        RerollFree();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameState.Instance.OwnedModel.AddMedal(100);

            UpdateBuyButton();
        }
    }

    public void SelectItem(ItemData item)
    {
        currentItem = item;

        detailPanel.SetActive(true);

        detailIcon.sprite = item.icon;
        detailName.text = item.itemName;
        detailDesc.text = item.description[item.level];
        costText.text = item.cost + "枚";

        if (item.isConsumable)
        {
            itemTypeText.text = "消費アイテム";
        }
        else
        {
            itemTypeText.text = "バフアイテム";
        }

        UpdateBuyButton();
    }
    //------------------------------------
    // 購入
    //------------------------------------
    public void BuyItem()
    {
        Debug.Log(currentItem.name, this);
        if (currentItem == null)
            return;

        if (GameState.Instance.OwnedModel.Count < 0)
        {
            Debug.Log("メダル不足");
            return;
        }

        Debug.Log(currentItem.itemName + " を購入しました");
        GameState.Instance.OwnedModel.RemoveMedal(currentItem.cost[1]);
        currentItem = null;

        detailPanel.SetActive(false);

        buyButton.interactable = false;
    }

    //------------------------------------
    // 購入ボタン更新
    //------------------------------------
    void UpdateBuyButton()
    {
        if (currentItem == null)
        {
            buyButton.interactable = false;
            return;
        }

        buyButton.interactable =
            GameState.Instance.OwnedModel.Count >= currentItem.cost[1];
    }

    //------------------------------------
    // リロール
    //------------------------------------
    public void Reroll()
    {
        // ★無料リロールがある場合
        if (freeRerollCount > 0)
        {
            freeRerollCount--;
            Debug.Log($"無料リロール使用！残り {freeRerollCount}");
        }
        else
        {
            rerollCount++;

            int cost = rerollCost * rerollCount;

            if (GameState.Instance.OwnedModel.Count < cost)
            {
                Debug.Log("メダル不足");
                return;
            }

            GameState.Instance.OwnedModel.RemoveMedal(cost);
        }

        RerollFree();

        currentItem = null;
        detailPanel.SetActive(false);
        buyButton.interactable = false;
    }

    //------------------------------------
    // 重複なしでショップ生成
    //------------------------------------
    void RerollFree()
    {
        List<ItemData> pool = new List<ItemData>(allItems);

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (pool.Count == 0)
                break;

            int randomIndex = Random.Range(0, pool.Count);

            currentItems[i] = pool[randomIndex];

            itemButtons[i].SetItem(currentItems[i]);

            pool.RemoveAt(randomIndex);
        }
    }
    public void RefreshUI()
    {
        UpdateBuyButton();
    }
    // ラウンド開始時に呼ぶ
    public void RoundShopReset()
    {
        rerollCount = 0;
        RerollFree();
    }

    // ID35から呼ぶ
    public void AddFreeReroll(int count)
    {
        freeRerollCount += count;
        Debug.Log($"無料リロール +{count}（現在 {freeRerollCount}回）");
    }
}
