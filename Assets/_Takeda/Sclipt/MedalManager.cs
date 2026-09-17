using UnityEngine;
using TMPro;
using EMR.Core;

public class MedalManager : MonoBehaviour
{
    public TMP_Text medalText;

    private void Start()
    {
        GameState.Instance.OwnedModel.OnCountChanged += UpdateUI;
        UpdateUI(GameState.Instance.OwnedModel.Count);
    }

    private void OnDisable()
    {
        GameState.Instance.OwnedModel.OnCountChanged -= UpdateUI;
    }


    void UpdateUI(int medal)
    {
        medalText.text = medal + "–‡";
    }
}