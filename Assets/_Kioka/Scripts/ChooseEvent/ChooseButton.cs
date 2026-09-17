using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChooseButton : MonoBehaviour
{
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト

    /// <summary>
    /// 選択肢のボタンを押したときの処理
    /// </summary>
    public void OnClickButton(int chooseIndex)
    {
        chooseManager.OnChooseButton(chooseIndex);
    }
}
