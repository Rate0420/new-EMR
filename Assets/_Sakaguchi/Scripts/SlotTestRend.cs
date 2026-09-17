using UnityEngine;
using TMPro;
using System.Collections;

public class SlotTestRend : MonoBehaviour
{
    public TextMeshProUGUI resultText; // 結果表示用のテキスト
    [SerializeField] float EffectTime = 1.0f;
    float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
    }


    public void SlotRend(int[] slotnum,SlotManager.EffectRank effectRank,SlotManager.EffectType effectType)
    {
        // ここでslotnumをもとにスロットの絵柄を表示する処理を書く　今回はテストなのでTMPに表示するだけ
        // まず演出を知らせ、左、右、中の順でslotnum[0]、slotnum[2]、slotnum[1]に対応させる。それぞれEffectTime秒待つ
        StartCoroutine(SlotRendCoroutine(slotnum,effectRank,effectType));
    }

    IEnumerator SlotRendCoroutine(int[] slotnum, SlotManager.EffectRank effectRank, SlotManager.EffectType effectType)
    {
        resultText.text = $"演出: _______ : ___________";
        yield return new WaitForSeconds(EffectTime);
        resultText.text = $"演出: _______ : ___________ \n{slotnum[0]}____";
        yield return new WaitForSeconds(EffectTime);
        resultText.text = $"演出: _______ : ___________ \n{slotnum[0]}___{slotnum[2]}";
        yield return new WaitForSeconds(EffectTime);
        resultText.text = $"演出: {effectRank} : ___________ \n{slotnum[0]}___{slotnum[2]}";
        yield return new WaitForSeconds(EffectTime);
        resultText.text = $"演出: {effectRank} : {effectType} \n{slotnum[0]}___{slotnum[2]}";
        yield return new WaitForSeconds(EffectTime);
        resultText.text = $"演出: {effectRank} : {effectType} \n{slotnum[0]}_{slotnum[1]}_{slotnum[2]}";

    }
}
