using UnityEngine;

public class S_ChooseButton : MonoBehaviour
{
    [SerializeField] private S_ChooseManager chooseManager;   // ChooseManagerスクリプト

    /// <summary>
    /// 選択肢のボタンを押したときの処理
    /// </summary>
    public void OnClickButton(int chooseIndex)
    {
        chooseManager.OnChoiceButtonPressed(chooseIndex);
    }
}
