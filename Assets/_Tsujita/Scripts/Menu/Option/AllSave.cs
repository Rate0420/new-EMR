using UnityEngine;
using Cysharp.Threading.Tasks;

public class AllSave : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject saveText;

    [SerializeField] private float fdx;

    public bool isTitle;    // タイトル画面にいるか

    /// <summary>
    /// セーブ
    /// </summary>
    public void AllSeve()
    {
        // 未実装
        // ここでセーブの呼び出し
        if (isTitle)
            isTitle = false;
        else
            SaveButton().Forget();
    }

    private async UniTask SaveButton()
    {
        saveText.SetActive(true);
        await UniTask.WaitForSeconds(fdx);
        saveText.SetActive(false);
        menuManager.OnMeunButtons(0);
        await UniTask.Yield();
    }
}
