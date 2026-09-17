using UnityEngine;

public class Test_T : MonoBehaviour
{
    // デバッグ用
    // アイテムレベルのリセット
    [SerializeField] ItemData[] itemData;
    private void Start()
    {
        for(int i = 0; i < itemData.Length; i++)
        {
            itemData[i].level = 0;
        }
    }
}
