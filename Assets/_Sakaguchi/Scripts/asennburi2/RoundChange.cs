using UnityEngine;
using System.Collections;
using EMR.Core;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RoundChange : MonoBehaviour
{
    private GamePause gamePause;

    [SerializeField] int[] RoundChangeCost;

    [SerializeField] GameObject MedalRoot;
    [SerializeField] PanelAniZoom panelAniZoom;
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject BlackOutImage;

    [SerializeField] GameObject RoundChangeCanvas;          // ラウンドチェンジ用のキャンバス

    float textApplyTime = 1.0f;

    [SerializeField] TextMeshProUGUI posessionMedalText;        // 所持メダル数表示用のTextMeshProUGUI
    [SerializeField] TextMeshProUGUI posessionMedalLabel;       // 所持メダル数ラベル用のTextMeshProUGUI
    [SerializeField] TextMeshProUGUI repaymentMedalText;        // 必要メダル数表示用のTextMeshProUGUI
    [SerializeField] TextMeshProUGUI repaymentMedalLabel;       // 必要メダル数ラベル用のTextMeshProUGUI
    [SerializeField] TextMeshProUGUI newPosessionMedalText;     // 新所持メダル数表示用のTextMeshProUGUI
    [SerializeField] TextMeshProUGUI newPosessionMedalLabel;    // 新所持メダル数ラベル用のTextMeshProUGUI

    [SerializeField] TextMeshProUGUI resultText; // ラウンドチェンジ結果表示用のTextMeshProUGUI



    // 外部から参照できるようにプロパティ化
    bool isProcessing = false;
    public bool IsRoundChangeProcessing => isProcessing;

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
        GameState.Instance.RoundManager.onNextRoundCalled.AddListener(StartRoundChange);
    }

    public void StartRoundChange()
    {
        if (isProcessing) return; // ← 念のための保険：既に処理中なら二重に走らせない
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
        StartCoroutine(ProcessRoundChange());
    }

    IEnumerator ProcessRoundChange()
    {
        Debug.Log("bbbbbbbbbbbbbbbbbbbbbb");
        isProcessing = true;

        // リールが止まっていて、他の誰もロックを持っていない状態になるまで自動的に待つ
        // (メニュー・JPC払い出し・ミニイベント中などは待たされる)
        yield return GameState.Instance.GameLock.Acquire("RoundChange");

        gamePause.ChangePause(true);
        yield return new WaitForSeconds(1);
        MedalRoot.SetActive(false);

        // ここでラウンドチェンジ処理を実行

        // まずはラウンドごとのメダル徴収

        // 足りなかったらゲームオーバー

        yield return StartCoroutine(RoundTextApp());

        GameState.Instance.RoundManager.SetRound(GameState.Instance.RoundManager.CurrentRound + 1);

        // 左クリックを待ち、押されたらシーン遷移
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        // ラウンド数を更新し、S_DontDestroyStoryのシーンを更新してロードする
        // ※ NextRound()はここでは呼ばない(呼ぶとonNextRoundCalledが再発火し、
        //    このProcessRoundChange自体がもう一度二重に走ってしまう原因になっていた)
        // 変更先はS_DontDestroyStory.instance.characterStoryのcurrentRoundに応じたstoryにする
        S_DontDestroyStory.instance.story = S_DontDestroyStory.instance.characterStory.storyParts[GameState.Instance.RoundManager.CurrentRound];

        yield return SceneManager.LoadSceneAsync("Sakaguchi_TestStoryScene", LoadSceneMode.Additive);


    }

    IEnumerator RoundTextApp()
    {
        RoundChangeCanvas.SetActive(true);

        GameState.Instance.RoundService.ResetProgress();

        yield return new WaitForSeconds(textApplyTime);
        posessionMedalLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(textApplyTime);
        posessionMedalText.text = GameState.Instance.OwnedModel.Count.ToString("F0") + "枚";
        yield return new WaitForSeconds(textApplyTime);
        repaymentMedalLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(textApplyTime);
        repaymentMedalText.text = RoundChangeCost[GameState.Instance.RoundManager.CurrentRound].ToString("F0") + "枚";
        yield return new WaitForSeconds(textApplyTime);
        newPosessionMedalLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(textApplyTime);
        newPosessionMedalText.text = (GameState.Instance.OwnedModel.Count - RoundChangeCost[GameState.Instance.RoundManager.CurrentRound]).ToString("F0") + "枚";

        GameState.Instance.OwnedModel.RemoveMedal(RoundChangeCost[GameState.Instance.RoundManager.CurrentRound]);

        yield return new WaitForSeconds(textApplyTime);

        if (GameState.Instance.OwnedModel.Count <= 0)
        {
            resultText.text = "借金が返せない...";
        }
        else
        {
            resultText.text = "返済期限を乗り越えた！";
        }
    }

    public void ResetMethod()
    {
        isProcessing = false;
        posessionMedalLabel.gameObject.SetActive(false);
        posessionMedalText.text = "";
        repaymentMedalLabel.gameObject.SetActive(false);
        repaymentMedalText.text = "";
        newPosessionMedalLabel.gameObject.SetActive(false);
        newPosessionMedalText.text = "";
        resultText.text = "";

        RoundChangeCanvas.gameObject.SetActive(false);

        // ラウンドチェンジ処理(演出・ストーリー含め)が完全に終わったので、
        // ここで初めて次のNextRound()を受け付けられるようにする
        GameState.Instance.RoundManager.FinishRoundChange();

        // "RoundChange"ロックを解放する(通常のミニイベント終了時など、
        // このロックを持っていない場合は何も起きない)
        GameState.Instance.GameLock.Release("RoundChange");
    }
}