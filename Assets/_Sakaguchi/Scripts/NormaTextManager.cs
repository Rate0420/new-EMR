using UnityEngine;
using TMPro;
using EMR.Core;

public class NormaTextManager : MonoBehaviour
{
    [SerializeField] TextMeshPro normaText; // ノルマを表示するテキスト
    [SerializeField] TextMeshPro roundText; // ラウンドを表示するテキスト

    private void Update()
    {
        roundText.text = "Round:" + GameState.Instance.RoundManager.CurrentRound.ToString("F0");
        normaText.text = "      [進行条件]\n必要メダル:\n<size=8>" + GameState.Instance.RoundService.ConsumedMedals.ToString("F0") + "/" + GameState.Instance.RoundService.RequiredMedalCount.ToString("F0") + "</size>枚" +
            "\n必要ボール:\n<size=8>" + GameState.Instance.RoundService.DroppedBallCount.ToString("F0") + "/" + GameState.Instance.RoundService.RequiredBallCount.ToString("F0") + "</size>個";



        // デバッグ
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameState.Instance.OwnedModel.AddMedal(100);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // ボール
            GameState.Instance.RoundService.AddDroppedBalls(1);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameState.Instance.RoundService.ConsumeMedals(20);
        }
    }
}
