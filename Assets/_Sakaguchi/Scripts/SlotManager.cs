using UnityEngine;
using System.Collections;
using static ReserveManager;

public class SlotManager : MonoBehaviour
{
    public const float baseWinProbability = 0.1f; //当たりの確率
    public const float baseChanceWinProbability = 0.3f; // 確変時の確率
    public float winProbability = 0.1f; //当たりの確率
    public float chanceWinProbability = 0.3f; // 確変時の確率
    [SerializeField] bool Kakuhen = false;
    [SerializeField] ReelManager reelManager; //リールの管理クラス
    [SerializeField] float slotEndDelay = 1.0f; //スロットが止まってから次の抽選までの遅延時間
    [SerializeField] EffectManager effectManager; // 演出管理クラス
    [SerializeField] ReserveManager reserveManager; // 保留管理クラス
    bool isWin; //当たりかどうか
    int selectedNumber = 1; // ルートキャラに対応した数字(後で別のクラスから参照するように変更)
    public bool testflg;

    int totalplayCount = 0;  // 現在の総回転数
    int playCount = 0;       // 最後の当たりからの回転数
    int StageChangeCount = 5; // ステージチェンジさせる回転数
    [SerializeField] BackgroundManager bgManager;

    [SerializeField] WinManager winManager;

    public bool IsBusy { get; private set; } = false;

    /*
     
    ・Weak（ほぼ外れ）
    通常セリフ
    何もなし
    弱リーチ（たまに）

    ・Normal
    リーチ
    キャラクターリーチ（弱）
    設定キャラセリフ

    ・Strong
    設定キャラリーチ
    キャラ群（低頻度）

    ・VeryStrong
    高確キャラリーチ
    高確設定キャラリーチ
    キャラ群確定

    ・Jackpot
    フリーズ

     */

    // 演出の強さを表すランク
    public enum EffectRank
    {
        Weak,
        Normal,
        Strong,
        VeryStrong,
        Jackpot
    }

    EffectRank rank;

    public enum EffectType
    {
        None,
        NormalTalk,
        SetCharacterTalk,
        CharacterCutin,
        SetCharacterCutin,
        CharacterReach,
        SetCharacterReach,
        CharacterGroup,
        HighChanceReach,
        HighChanceSetCharacterReach,
        Freeze
    }

    EffectType effect;

    void Start()
    {
        // 初期化
        winProbability = baseWinProbability;
        chanceWinProbability = baseChanceWinProbability;
    }

    public IEnumerator PlaySlot(ReserveData data)
    {
        IsBusy = true;
        yield return PlaySlotInternal(data);
        IsBusy = false;  // 何があっても必ずここに来る
    }

    int[] GenerateReelResult(int resultNumber,bool reach)
    {
        int[] reels = new int[3];

        if (resultNumber == -1)
        {
            // 外れでも1/4、もしくはキャラクターリーチ以上の演出ならリーチにする
            if (Random.value < 0.1f || effect >= EffectType.CharacterReach)
            {
                reach = true;
            }
        }

        if (resultNumber != -1)
        {
            // 当たり
            reels[0] = resultNumber;
            reels[1] = resultNumber;
            reels[2] = resultNumber;
        }
        else
        {
            // ハズレ（ここで演出パターン作る）
            int pattern = Random.Range(0, 3);

            // バラバラ(リーチではない)
            reels[0] = Random.Range(1, 10);
            reels[1] = Random.Range(1, 10);
            reels[2] = Random.Range(1, 10);
            //　reels[2]がreels[0]と同じにならないようにする
            while (reels[2] == reels[0])
            {
                reels[2] = Random.Range(1, 10);
            }

            if (reach)
            {
                int n = Random.Range(1, 10);
                reels[0] = n;
                reels[2] = n;

                reels[1] = Random.Range(1, 10);
                while (reels[1] == n)
                {
                    reels[1] = Random.Range(1, 10);
                }
            }
        }

        return reels;
    }

    void EffectGenerate()
    {
        if (!isWin)
        {
            float r = Random.value;

            if (r < 0.6f) rank = EffectRank.Weak;
            else if (r < 0.85f) rank = EffectRank.Normal;
            else if (r < 0.97f) rank = EffectRank.Strong;
            else rank = EffectRank.VeryStrong; // ←ハズレでも来る
        }
        else
        {
            float r = Random.value;

            if(r < 0.05f) rank = EffectRank.Weak;
            else if (r < 0.5f) rank = EffectRank.Normal;
            else if (r < 0.8f) rank = EffectRank.Strong;
            else if (r < 0.95f) rank = EffectRank.VeryStrong;
            else rank = EffectRank.Jackpot;
        }
    }

    EffectType SelectEffect(EffectRank rank)
    {
        float r = Random.value;

        switch (rank)
        {
            case EffectRank.Weak:
                if (r < 0.4f) return EffectType.None;
                if (r < 0.7f) return EffectType.NormalTalk;
                return EffectType.SetCharacterTalk;

            case EffectRank.Normal:
                if (r < 0.4f) return EffectType.CharacterCutin;
                if (r < 0.7f) return EffectType.SetCharacterCutin;
                return EffectType.CharacterReach;

            case EffectRank.Strong:
                if (r < 0.5f) return EffectType.SetCharacterReach;
                if (r < 0.8f) return EffectType.CharacterGroup;
                return EffectType.HighChanceReach;

            case EffectRank.VeryStrong:
                if (r < 0.5f) return EffectType.HighChanceReach;
                return EffectType.HighChanceSetCharacterReach;

            case EffectRank.Jackpot:
                return EffectType.Freeze;
        }

        return EffectType.None;
    }

    int GetRandomOtherNumber()
    {
        // 選択した数字以外の数字をランダムに返す（例えば1の場合は2,3,4,5,6,8,9の中から）
        int[] otherNumbers = new int[] { 1, 2, 3, 4, 5, 6, 8, 9 };
        // selectedNumberを除外
        otherNumbers = System.Array.FindAll(otherNumbers, n => n != selectedNumber);
        int index = Random.Range(0, otherNumbers.Length);
        return otherNumbers[index];
    }

    public ReserveData GenerateReserveData()
    {
        ReserveData data = new ReserveData();

        int resultNumber;
        float r = Random.value;

        if (Kakuhen)
        {
            if (r < chanceWinProbability)
            {
                resultNumber = GenerateResultNumber(); // 当たり
                data.isReach = true; // ★確定でリーチ
            }
            else
            {
                resultNumber = -1;

                // ★ここでリーチを決める（先に！）
                data.isReach = Random.value < 0.25f;
            }
        }
        else
        {
            if (r < winProbability)
            {
                resultNumber = GenerateResultNumber(); // 当たり
                data.isReach = true; // ★確定でリーチ
            }
            else
            {
                resultNumber = -1;

                // ★ここでリーチを決める（先に！）
                data.isReach = Random.value < 0.25f;
            }
        }

        data.resultNumber = resultNumber;

        // ↓ここで演出
        isWin = resultNumber != -1;
        EffectGenerate();
        data.rank = rank;
        data.effect = SelectEffect(rank);

        // ★強演出なら強制リーチ
        if (data.effect >= EffectType.CharacterReach)
        {
            data.isReach = true;
        }

        data.visual = DecideReserveVisual(data);

        return data;
    }
    ReserveVisualType DecideReserveVisual(ReserveData data)
    {
        // 面倒だがswitchでランク別にどの保留色になるかを決める
        switch(data.rank)
        {
            case EffectRank.Weak:
                // weakは7割ノーマル、3割青。
                return Random.value < 0.7f ? ReserveVisualType.Normal : ReserveVisualType.Blue;
            case EffectRank.Normal:
                // 2割ノーマル、6割青、2割緑
                return Random.value < 0.2f ? ReserveVisualType.Normal : (Random.value < 0.75f ? ReserveVisualType.Blue : ReserveVisualType.Green);
            case EffectRank.Strong:
                //　5割緑、5割赤
                return Random.value < 0.5f ? ReserveVisualType.Green : ReserveVisualType.Red;
            case EffectRank.VeryStrong:
                return ReserveVisualType.Red;
            case EffectRank.Jackpot:
                return ReserveVisualType.Gold;
            default:
                return ReserveVisualType.Normal;
        }
    }

    bool DecidePreEffect(ReserveData data)
    {
        if (data.rank >= EffectRank.Strong)
        {
            return Random.value < 0.7f;
        }

        if (data.rank == EffectRank.Normal)
        {
            return Random.value < 0.3f;
        }

        return false;
    }

    public IEnumerator PlaySlotInternal(ReserveData data)
    {
        IsBusy = true;  // ← 先頭に追加

        // テストフラグが立っている場合は、常に会話演出にする
        if (testflg)
        {
            Coroutine preEffectCoroutine = null;
            if (reserveManager.HasPreTarget())
            {
                effectManager.PlayPreEffect();
            }

            data.effect = EffectType.NormalTalk;
            data.rank = EffectRank.Weak;
            Debug.Log("ランク:"+data.rank +"演出:"+data.effect); 
            int[] reel = GenerateReelResult(data.resultNumber,data.isReach); // 3を当たりとして生成
            reelManager.StartReels();
            Coroutine efc = StartCoroutine(effectManager.PlayEffect(data.effect));
            // efcが終わるまで待つ
            yield return efc;
            reelManager.StartStopReels(reel);

            yield return new WaitUntil(() =>
                !reelManager.leftReel.IsSpinning &&
                !reelManager.centerReel.IsSpinning &&
                !reelManager.rightReel.IsSpinning
            );

            // ★ここが重要（当たり処理）
            if (data.resultNumber != -1 && data.resultNumber != 7)
            {
                // 保留停止
                reserveManager.isPaused = true;

                // 当たり演出
                yield return StartCoroutine(winManager.PlayWin(data.resultNumber));

                // 再開
                reserveManager.isPaused = false;
            }

            // とりあえずresultnumが７の場合も処理する
            if (data.resultNumber == 7)
            {
                // 保留停止
                reserveManager.isPaused = true;

                // 当たり演出
                yield return StartCoroutine(winManager.PlayWin(data.resultNumber));

                // 再開
                reserveManager.isPaused = false;
            }

            yield return new WaitForSeconds(slotEndDelay);

            if (preEffectCoroutine != null)
            {
                StopCoroutine(preEffectCoroutine);
            }
            IsBusy = false;
        }

        else
        {
            Coroutine preEffectCoroutine = null;

            // 先読み対象があるか確認
            if ( reserveManager.HasPreTarget())
            {
                Debug.Log("先読み演出");
                effectManager.PlayPreEffect();
            }

            Coroutine effectCoroutine = null;
            int[] reels;
            if (data.effect == EffectType.Freeze)
            {
                data.resultNumber = 7;
                reels = GenerateReelResult(7, true);
            }
            else
            {
                reels = GenerateReelResult(data.resultNumber, data.isReach);
            }
            
            Debug.Log("ランク:" + data.rank + "演出:" + data.effect);
            reelManager.StartReels();

            // 演出をyield returnで待ってから停止開始
            if (data.effect != EffectType.None)
            {
                yield return StartCoroutine(effectManager.PlayEffect(data.effect));
            }

            // 演出が終わってから停止開始
            reelManager.StartStopReels(reels);
            Debug.Log("[SlotManager] StartStopReels呼び出し");

            yield return new WaitUntil(() => reelManager.IsAllStopped);

            // 演出がまだ終わっていない場合は待つ
            if (effectCoroutine != null)
            {
                yield return effectCoroutine;
            }

            yield return new WaitForSeconds(slotEndDelay);

            // 当たり処理...

            // 当たってた場合ここで当たりの演出をやる

            // dataの当たりに応じて確変か確変じゃないかを決める
            // 7以外の奇数(1,3,5,9)だった場合、確変フラグをオンにする

            // 当たりの場合
            if (data.resultNumber != -1)
            {
                if(data.resultNumber == 7)
                {
                    // JPC処理
                }
                // 7以外の奇数(1,3,5,9)だった場合、確変フラグをオンにする
                if (data.resultNumber % 2 == 1)
                {
                    Kakuhen = true;
                    // 確変ステージに移行させる
                    bgManager.ChangeStage(StageType.Special);
                    playCount = 0;
                }
                else
                {
                    // 現在確変中だったら通常ステージに戻す
                    if (Kakuhen)
                    {
                        bgManager.ChangeStage((StageType)Random.Range(0, (int)StageType.Special));
                    }
                    Kakuhen = false;
                    playCount = 0;
                }

                if (data.resultNumber != -1 && data.resultNumber != 7)
                {
                    // 保留停止
                    reserveManager.isPaused = true;

                    // 当たり演出
                    yield return StartCoroutine(winManager.PlayWin(data.resultNumber));

                    // 再開
                    reserveManager.isPaused = false;
                }

                // とりあえずresultnumが７の場合も処理する
                if (data.resultNumber == 7)
                {
                    // 保留停止
                    reserveManager.isPaused = true;

                    // 当たり演出
                    yield return StartCoroutine(winManager.PlayWin(data.resultNumber));

                    // 再開
                    reserveManager.isPaused = false;
                }
            }

            if (preEffectCoroutine != null)
            {
                StopCoroutine(preEffectCoroutine);
            }

            playCount++;
            totalplayCount++;

            if (playCount != 0 && !Kakuhen)
            {
                Debug.Log($"現在の回転数: {playCount}");
                if (playCount % StageChangeCount == 0)
                {
                    // enumの最初から最後を除いたものの中からランダムにステージを選ぶ
                    bgManager.ChangeStage((StageType)Random.Range(0, (int)StageType.Special));
                }
            }
            IsBusy = false;
        }
    }

    int GenerateResultNumber()
    {
        int resultnum;
        float r = Random.value;
        //ルートキャラの数字 = 40% 
        //非ルートキャラ = 8 %×7人 = 56 
        //7(JPCC) = 4 %
        if (r < 0.4f) resultnum = selectedNumber;
        else if (r < 0.96f) resultnum = GetRandomOtherNumber();
        else resultnum = 7;

        return resultnum;
    }
}

