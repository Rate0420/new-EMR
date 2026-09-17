//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//public class TestButton : MonoBehaviour
//{
//    private ItemData itemData;

//    private BuffShopManager shopManager;

//    private Button button;

//    private Transform iconRoot;

//    [SerializeField] private TMP_Text nameText;
//    [SerializeField] private TMP_Text costText;

//    GameObject currentIcon;

//    void Awake()
//    {
//        button = GetComponent<Button>();

//        if (button != null)
//        {
//            button.onClick.AddListener(Select);
//        }
//    }

//    private void OnDisable()
//    {
//        button.onClick.RemoveListener(Select);
//    }

//    public void SetItem(ItemData item)
//    {
//        itemData = item;

//        nameText.text = item.itemName;
//        costText.text = item.cost + "枚";

//        // 前のアイコン削除
//        if (currentIcon != null)
//            Destroy(currentIcon);

//        // 新しいアイコン生成
//        if (item.buttonIconPrefab != null)
//        {
//            currentIcon = Instantiate(item.buttonIconPrefab, iconRoot, false);

//            RectTransform rect = currentIcon.GetComponent<RectTransform>();
//            if (rect != null)
//            {
//                rect.anchoredPosition = Vector2.zero;
//                rect.localScale = Vector3.one;
//            }
//        }
//    }

//    private void Select()
//    {
//        if (itemData != null)
//            shopManager.SelectItem(itemData);
//    }
//}
