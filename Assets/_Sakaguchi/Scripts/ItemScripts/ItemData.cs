using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    // アイテムの種類
    public ItemType itemType;

    // アイテム名
    public string itemName;

    // 詳細画面などに表示する説明文
    [TextArea(3, 5)]
    public string description;

    // 購入に必要なメダル数
    public int[] cost;

    // 詳細画面に表示するアイコン(Sprite)
    public Sprite icon;

    // 購入時にInventoryへ追加し、ゲーム中に効果を発揮する
    public ItemEffect effect;

    // 消費アイテムかどうか
    public bool isConsumable;

    // アイテムレベル
    public int level;
}