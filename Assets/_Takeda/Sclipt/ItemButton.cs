using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class ItemButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    
    private Button button;
    private ItemData item;

    public event Action<ItemData> OnButtonClicked;

    // âºíuÇ´
    public int round = 3;

    private void Awake()
    {
        button = GetComponent<Button>();
        round = 3;
    }

    private void OnEnable()
    {
        button.onClick.AddListener(Select);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Select);
    }

    // èâä˙âªÇ≈åƒÇŒÇÍÇÈÇÊ
    public void SetItem(ItemData item)
    {
        this.item = item;
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        costText.text = $"{item.cost[round]}ñá"; ;
    }

    private void Select()
    {
        OnButtonClicked?.Invoke(item);
    }
}