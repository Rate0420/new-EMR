using UnityEngine;

public class BuffChange : MonoBehaviour
{
    [SerializeField] private StatusGet statusGet;
    private ItemData itemData;  // ボタンが持っているアイテムの取得用

    /// <summary>
    /// バフの取得
    /// </summary>
    public void BuffNo()
    {
        statusGet.AddBuff(itemData.itemType);
    }
}
