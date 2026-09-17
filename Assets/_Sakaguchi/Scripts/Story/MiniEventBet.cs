using EMR.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class MiniEventBet : MonoBehaviour
{
    // ミニイベント用
    public int effectInt = 0;  // 何個プラスかマイナスなのか

    int Medals;
    int textMedals;
    
    [SerializeField] TextMeshProUGUI medalText;
    [SerializeField] TextMeshProUGUI consumptionText;
    [SerializeField] TextMeshProUGUI EffectText;

    public int consumptionMedal = 15;

    void Awake()
    {
        Medals = GameState.Instance.OwnedModel.Count;
        Debug.Log("メダル:"+Medals);
        textMedals = Medals;

        medalText.text = "所持メダル：" + textMedals.ToString();
        consumptionText.text = "消費メダル" + (consumptionMedal * effectInt).ToString();
        EffectText.text = Effect();
    }

    public void BetUP()
    {
        Debug.Log("ベットアップ");
        if (textMedals - consumptionMedal <= 0) return;
        Debug.Log("ベットアップ2");
        textMedals -= consumptionMedal;
        effectInt++;

        medalText.text = "所持メダル：" + textMedals.ToString();
        consumptionText.text = "消費メダル"+(consumptionMedal*effectInt).ToString();
        EffectText.text = Effect();
        
    }

    public void BetDown()
    {
        if (effectInt >= 1)
        {
            effectInt--;
            textMedals += consumptionMedal;

            medalText.text = "所持メダル：" + textMedals.ToString();
            consumptionText.text = "消費メダル" + (consumptionMedal * effectInt).ToString();
            EffectText.text = Effect();
        }
    }

    string Effect()
    {
        switch (effectInt)
        { 
            case 0: return "効果なし";
            case 1: return "微増";
            case 2: return "微増";
            case 3: return "増加";
            case 4: return "増加";
        }
        if (effectInt >= 5)
        {
            return "超増加";
        }
        return "";
    }

    public int Calc(int a)
    {
        return a += effectInt;
    }
}
