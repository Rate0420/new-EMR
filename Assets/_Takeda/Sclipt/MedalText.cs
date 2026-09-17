using UnityEngine;
using TMPro;
using EMR.Core;

public class MedalText : MonoBehaviour
{
   [SerializeField] private TMP_Text medalText;

    private void Start()
    {
        GameState.Instance.OwnedModel.OnCountChanged += UpdateUI;
        UpdateUI(GameState.Instance.OwnedModel.Count);
    }

    private void OnEnable()
    {
        GameState.Instance.OwnedModel.OnCountChanged += UpdateUI;
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