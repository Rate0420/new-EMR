using EMR.Core;
using EMR.Medal.Refund;
using TMPro;
using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshPro payoutText;
    [SerializeField] VideoEffectPlayer videoPlayer;

    [SerializeField] MedalRefundBehaviour refundBehaviour;
    [SerializeField] MedalRefundBehaviour refundBehaviour2;
    [SerializeField] PriseGenerator priseGenerator;

    public bool isPayout = false;

    // どちらのrefundBehaviourを使うかのフラグ（SetActiveを使わない）
    bool useJPCRefund = false;

    public void SetIsPayout(bool value) => isPayout = value;

    public IEnumerator PlayWin(int resultNumber)
    {
        useJPCRefund = false;
        refundBehaviour.gameObject.SetActive(true);
        refundBehaviour2.gameObject.SetActive(false);
        yield return StartCoroutine(PlayWinInternal(GetPayout(resultNumber)));
        priseGenerator.DisChargeBall();
        priseGenerator.PriseLottely();
    }

    public IEnumerator PlayWinbyJPC(int payoutNum)
    {
        useJPCRefund = true;
        refundBehaviour.gameObject.SetActive(false);
        refundBehaviour2.gameObject.SetActive(true);
        yield return StartCoroutine(PlayWinInternal(payoutNum));
    }

    IEnumerator PlayWinInternal(int payout)
    {
        isPayout = true;

        yield return videoPlayer.PlayVideoNoFadeCoroutine(0, 1.0f);

        winUI.SetActive(true);
        payoutText.text = $"{payout}枚";

        bool isFinished = false;
        System.Action<int> spawnHandler = (remaining) => { payoutText.text = $"{remaining}枚"; };
        System.Action finishHandler = () => { isFinished = true; };

        // useJPCRefundに応じて使うrefundBehaviourを切り替え
        MedalRefundBehaviour target = useJPCRefund ? refundBehaviour2 : refundBehaviour;

        if (target == null)
        {
            Debug.LogError($"{nameof(WinManager)}: refundBehaviour が未設定です");
            winUI.SetActive(false);
            isPayout = false;
            yield break;
        }

        target.OnMedalSpawned += spawnHandler;
        target.OnRefundFinished += finishHandler;

        GameState.Instance.RefundNotifier.RequestRefund(payout);

        float timeout = Mathf.Max(10f, payout * 0.5f + 5f);
        float elapsed = 0f;
        while (!isFinished && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.OnMedalSpawned -= spawnHandler;
        target.OnRefundFinished -= finishHandler;

        if (!isFinished)
        {
            Debug.LogWarning($"{nameof(WinManager)}: Refund did not finish within {timeout:0.0}s");
        }

        winUI.SetActive(false);
        isPayout = false;
    }

    int GetPayout(int number)
    {
        if (number == 7) return 100;
        if (number % 2 == 0) return 30;
        else return 50;
    }
}