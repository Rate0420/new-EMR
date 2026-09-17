using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    [SerializeField] private ItemData[] itemData;

    public ItemData GetCharacter(ItemType itype)
    {
        foreach (ItemData data in itemData)
        {
            if (data.itemType == itype)
                return data;
        }
        return null;
    }
}
