using UnityEngine;

public class Test_T2 : MonoBehaviour
{
    public StatusGet statusGet;
    public ItemData itemData;

    // 装備変更テスト用
    public void BuffNo()
    {
        statusGet.AddBuff(itemData.itemType);
    }

}
