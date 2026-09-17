using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public SequencePlayer cutinPlayer;
    public TalkManager talkManager;
    [SerializeField] string[] cutinPath;
    public int selectedNumber = 0; // ルートキャラに対応した数字(後で別のクラスから参照するように変更)
    [SerializeField] string selectedCharacter;
    public float reachtime;
    [SerializeField] VideoEffectPlayer videoEffectPlayer;
    public VideoEffectPlayer preEffectPlayer;

    [SerializeField] GameObject[] ReachChara;        // Arisugawaオブジェクト
    [SerializeField] Animator[] CharacterAnimator;  // VRMのAnimator
    [SerializeField] ReachEffectTrigger[] reachEffectTrigger; // アタッチしたTrigger
    [SerializeField] ReelManager reelManager;

    int reachCharacterNum;

    SlotManager.EffectType currentEffect;

    // ReelManagerから参照できるようにプロパティ化
    public SlotManager.EffectType CurrentEffect => currentEffect;

    public void SetCurrentEffect(SlotManager.EffectType effect)
    {
        currentEffect = effect;
    }

    enum selectedCharacterEnum
    {
        陽向,
        加流,
        小夜,
        澪音,
        リーゼロッテ,
        リリス,
        冥衣,
        ヴェルミリオン
    }

    // PlayEffectのcase追記
    public IEnumerator PlayEffect(SlotManager.EffectType effect)
    {
        currentEffect = effect; // ← effectをフィールドに保存
        Debug.Log($"[演出] 効果: {effect}");
        switch (effect)
        {
            case SlotManager.EffectType.NormalTalk:
                yield return StartCoroutine(PlayTalk("キャラセリフ", GetRandomOtherNumber()));
                break;
            case SlotManager.EffectType.SetCharacterTalk:
                yield return StartCoroutine(PlayTalk("設定キャラセリフ", selectedNumber));
                break;
            case SlotManager.EffectType.CharacterCutin:
                yield return StartCoroutine(PlayCutin("キャラカットイン", GetRandomOtherNumber()));
                break;
            case SlotManager.EffectType.SetCharacterCutin:
                yield return StartCoroutine(PlayCutin("設定キャラカットイン", selectedNumber));
                break;
            case SlotManager.EffectType.CharacterGroup:
                yield return StartCoroutine(PlayCharacteroGroup());
                break;
            case SlotManager.EffectType.Freeze:
                yield return StartCoroutine(PlayFreez());
                break;
            // CharacterReach系はPlayEffectでは何もしない
            // （リール停止後にReelManagerからPlayReachEffectが呼ばれる）
            case SlotManager.EffectType.CharacterReach:
            case SlotManager.EffectType.SetCharacterReach:
            case SlotManager.EffectType.HighChanceReach:
            case SlotManager.EffectType.HighChanceSetCharacterReach:
                reachCharacterNum = Random.Range(0, ReachChara.Length);
                yield return null;
                break;
            default:
                yield return null;
                break;
        }
    }

    // リーチ時にReelManagerから呼ばれる
    public IEnumerator PlayReachEffect(int winIndex, bool isWin)
    {
        // 仮停止完了を待つ
        yield return new WaitUntil(() => reelManager.centerReel.IsTempStopped);
        Debug.Log("[EffectManager] 仮停止確認、キャラ演出開始");

        yield return StartCoroutine(PlayCharacterReach(winIndex, isWin));
    }

    public IEnumerator PlayCharacterReach(int winIndex, bool isWin)
    {

        // ① キャラクター表示・アニメーション開始
        reachEffectTrigger[reachCharacterNum].Setup(reelManager, winIndex, isWin);
        ReachChara[reachCharacterNum].SetActive(true);
        string animName = isWin ? "Win" : "Lose";
        CharacterAnimator[reachCharacterNum].Play(animName);
        Debug.Log($"[EffectManager] キャラアニメ再生: {animName}");

        // ② アニメーションイベント「OnReelRestart」を待つ
        // （アニメーションの攻撃タイミングで中リール再回転させる）
        reelManager.NotifyReachEffectEndReset();
        reelManager.NotifyReelRestartReset(); // ← 追加

        yield return new WaitUntil(() => reelManager.IsReelRestartRequested);
        Debug.Log("[EffectManager] 中リール再回転開始");

        // ③ 中リール再回転
        reelManager.centerReel.StartSpin();

        // ④ アニメーションイベント「OnReelStop」を待つ
        yield return new WaitUntil(() => reelManager.IsReelStopRequested);
        Debug.Log("[EffectManager] 中リール停止開始");

        // ⑤ 結果に応じて停止
        reelManager.centerReel.StopSpin(isWin ? winIndex : reelManager.CurrentLoseIndex);
        yield return new WaitUntil(() => !reelManager.centerReel.IsSpinning);
        Debug.Log("[EffectManager] 中リール停止完了");

        // ⑥ アニメーション終了待ち
        yield return new WaitUntil(() => reelManager.IsReachEffectEnded);

        ReachChara[reachCharacterNum].SetActive(false);
    }

    public IEnumerator PlayFreez()
    {
        Debug.Log($"[演出] フリーズ");
        yield return StartCoroutine(videoEffectPlayer.PlayVideoCoroutine(16, 0.5f));
    }

    public IEnumerator PlayCharacteroGroup()
    {
        Debug.Log($"[演出] キャラ群予告");
        // 再生は裏で走らせる（最後まで流れる）
        StartCoroutine(videoEffectPlayer.PlayVideoNoFadeCoroutine(17, 1f));
        yield return StartCoroutine(videoEffectPlayer.WaitEarlyEndCoroutine(3f));
    }

    // ReelManagerからyield returnできるように変更
    public IEnumerator PlayReach()
    {
        StartCoroutine(videoEffectPlayer.PlayVideoNoFadeCoroutine(18, 1f));
        yield return StartCoroutine(videoEffectPlayer.WaitEarlyEndCoroutine(1f));
    }

    IEnumerator PlayTalk(string text, int num)
    {
        Debug.Log($"[演出] 会話: {text}");

        // selectedNumberをenumに変換してキャラ名を取得
        selectedCharacterEnum characterEnum = (selectedCharacterEnum)num;
        selectedCharacter = characterEnum.ToString();
        int talknum = talkManager.searchTalkBoxNum(selectedCharacter);
        Debug.Log("キャラクター" + selectedCharacter);
        yield return new WaitForSeconds(talkManager.SetTalkBox(talknum));
    }

    IEnumerator PlayCutin(string name, int num)
    {
        Debug.Log($"[演出] カットイン: {name}");
        yield return new WaitForSeconds(cutinPlayer.Play(cutinPath[num]));
        cutinPlayer.Stop();
    }

    int GetRandomOtherNumber()
    {
        while (true)
        {
            int num = Random.Range(0, cutinPath.Length);
            if (num != selectedNumber) return num;
        }
    }

    public void PlayPreEffect()
    {
        int r = Random.Range(0, preEffectPlayer.videoPaths.Length);
        switch (r)
        {
            case 0:
                Debug.Log($"[演出] 先読み: 白ほうき星");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
            case 1:
                Debug.Log($"[演出] 先読み: 雪結晶");
                StartCoroutine(preEffectPlayer.PlayVideoFadeInOut(r, 1f, 1, 1));
                break;
            case 2:
                Debug.Log($"[演出] 先読み: レンズフレア");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
            case 3:
                Debug.Log($"[演出] 先読み: 光の風的な奴");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
        }


    }

}