using UnityEngine;
using EMR.Core;
using EMR.Medal;

public class RoundMedalBonusController : MonoBehaviour
{
    private void Start()
    {
        GameState.Instance.RoundManager.OnRoundChanged += OnRoundChanged;
    }

    private void OnDestroy()
    {
        if (GameState.Instance != null &&
            GameState.Instance.RoundManager != null)
        {
            GameState.Instance.RoundManager.OnRoundChanged -= OnRoundChanged;
        }
    }

    private void OnRoundChanged(int currentRound)
    {
        if (MedalBuffData.RoundMedalBonus <= 0f)
            return;

        int possessionMedals = GameState.Instance.OwnedModel.Count;

        float bonusRate =
            MedalBuffData.RoundMedalBonus * currentRound;

        int bonusMedals =
            Mathf.FloorToInt(possessionMedals * bonusRate);

        // メダルが増えたときだけログを出す
        if (bonusMedals > 0)
        {
            GameState.Instance.OwnedModel.AddMedal(bonusMedals);

            Debug.Log(

                $"高そうな腕時計発動！ " +
                $"ラウンド:{currentRound} " +
                $"付与率:{bonusRate:P0} " +
                $"メダルが{bonusMedals}枚増えました！"
            );
        }
    }
}