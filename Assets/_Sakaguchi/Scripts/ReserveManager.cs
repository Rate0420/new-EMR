using EMR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReserveManager : MonoBehaviour, IReserveGate
{
    [SerializeField] int maxReserve = 5;
    Queue<ReserveData> reserves = new Queue<ReserveData>();
    ReserveData currentReserve;

    [SerializeField] SlotManager slotManager;
    [SerializeField] TextMeshProUGUI reserveCountText;

    [SerializeField] EffectManager effectManager;
    [SerializeField] GameObject[] ReserveObject;  // 0:現在消化中の保留、1~5:保留スロット

    [SerializeField] private Sprite[] miniCharas;   // ミニキャラ画像 
    [SerializeField] private SpriteRenderer[] miniCharaRenderers;

    private GamePause gamePause;

    public bool isProcessing { get; set; } = false;
    public bool pauseRequested { get; set; } = false;

    public bool isPaused = false;

    // 保留と保留の間（次の消化前の待機中）かどうか
    public bool isBetweenReserves { get; private set; } = false;
    // 外部から「今すぐ止めていい」と伝えるフラグ

    IEnumerator ProcessReserve()
    {
        isProcessing = true;

        while (reserves.Count > 0)
        {
            isBetweenReserves = true;

            // ポーズ要求があれば、解除されるまでここで待機
            yield return new WaitUntil(() => !isPaused && !pauseRequested);

            isBetweenReserves = false;

            DecidePreTargets();
            currentReserve = reserves.Dequeue();
            UpdateReserveVisuals();

            yield return slotManager.PlaySlot(currentReserve);

            currentReserve = null;
            UpdateReserveVisuals();
        }

        isBetweenReserves = true;
        isProcessing = false;
    }

    public void UpgradeReserve(int index)
    {
        var array = reserves.ToArray();

        if (index >= array.Length) return;

        array[index].visual = ReserveVisualType.Red;

        reserves = new Queue<ReserveData>(array);
    }

    public enum ReserveVisualType
    {
        Normal,
        Blue,
        Green,
        Red,
        Gold
    }

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
        gamePause.OnPausedChange += ChangePause;
        GameState.Instance.GameLock.RegisterReserveManager(this); // ← 追加
    }

    private void Update()
    {
        // デバッグ用：スペースキーで保留追加
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddReserve();
        }
    }

    // 外部から呼ぶ
    public void AddReserve()
    {
        if (reserves.Count >= maxReserve) return;

        ReserveData data = slotManager.GenerateReserveData();
        reserves.Enqueue(data);

        if (miniCharas != null && miniCharas.Length > 0)
        {
            int randomIndex = Random.Range(0, miniCharas.Length);
            data.miniChara = miniCharas[randomIndex];
        }

        UpdateReserveVisuals();

        if (!isProcessing)
        {
            StartCoroutine(ProcessReserve());
        }

        // 保留オブジェクトの色を更新

    }


    public void UpdateReserveVisuals()
    {
        var array = reserves.ToArray();

        // 0番は「現在消化中」
        var img0 = ReserveObject[0].GetComponent<Renderer>().material;

        if (currentReserve != null)
        {
            //img0.color = GetColor(currentReserve.visual);

            switch (currentReserve.visual)
            { 
                case ReserveVisualType.Normal:
                    img0.DisableKeyword("_EMISSION");
                    img0.color = Color.white;
                    break;

                case ReserveVisualType.Blue:
                    img0.EnableKeyword("_EMISSION");
                    img0.color = Color.blue;
                    img0.SetColor("_EmissionColor", Color.blue * 2.5f);
                    break;

                case ReserveVisualType.Green:
                    img0.EnableKeyword("_EMISSION");
                    img0.color = Color.green;
                    img0.SetColor("_EmissionColor", Color.green * 2.5f);
                    break;
                case ReserveVisualType.Red:
                    img0.EnableKeyword("_EMISSION");
                    img0.color = Color.red;
                    img0.SetColor("_EmissionColor", Color.red * 2.5f);
                    break;
                case ReserveVisualType.Gold:
                    img0.EnableKeyword("_EMISSION");
                    img0.color = Color.yellow;
                    img0.SetColor("_EmissionColor", Color.yellow * 2.5f);
                    break;
            }
        }
        else
        {
            img0.color = Color.gray;
        }

        // 画像の更新
        SetMiniChara(0, currentReserve);

        // 1以降は「待機中保留」
        for (int i = 0; i < array.Length; i++)
        {
            var img = ReserveObject[i + 1].GetComponent<Renderer>().material;

            img.color = GetColor(array[i].visual);

            // 画像の更新
            SetMiniChara(i + 1, array[i]);
        }

        // 空きスロット
        for (int i = array.Length + 1; i < ReserveObject.Length; i++)
        {
            var img = ReserveObject[i].GetComponent<Renderer>().material;
            img.color = Color.gray;
        }
    }

    Color GetColor(ReserveVisualType type)
    {
        switch (type)
        {
            case ReserveVisualType.Normal: return Color.white;
            case ReserveVisualType.Blue: return Color.blue;
            case ReserveVisualType.Green: return Color.green;
            case ReserveVisualType.Red: return Color.red;
            case ReserveVisualType.Gold: return Color.yellow;
        }
        return Color.white;
    }

    void DecidePreTargets()
    {
        var array = reserves.ToArray();

        // 条件に合う候補を集める
        List<int> candidates = new List<int>();

        for (int i = 0; i < array.Length; i++)
        {
            var data = array[i];

            bool isHit = data.resultNumber != -1;
            bool isStrong = data.effect >= SlotManager.EffectType.CharacterReach;

            if (isHit || isStrong)
            {
                candidates.Add(i);
            }
        }

        // 候補なしなら終了
        if (candidates.Count == 0) return;

        // 候補の中を全て1/4で抽選し、先読みフラグを立てる
        foreach (var i in candidates)
        {
            if (Random.value < 0.9f)
            {
                array[i].isPreTarget = true;
            }
            else
            {
                array[i].isPreTarget = false;
            }
        }

        // Queueに戻す
        reserves = new Queue<ReserveData>(array);

    }

    public bool HasPreTarget()
    {
        foreach (var r in reserves)
        {
            Debug.Log($"保留: {r.resultNumber}, {r.effect}, 先読み: {r.isPreTarget}");
            if (r.isPreTarget) return true;
        }
        return false;
    }

    public void ChangePause()
    {
        isPaused = gamePause.isPaused;
    }

    private void SetMiniChara(int index, ReserveData data)
    {
        if (miniCharaRenderers == null ||
            index < 0 ||
            index >= miniCharaRenderers.Length)
        {
            return;
        }

        SpriteRenderer spriteRenderer = miniCharaRenderers[index];

        if (spriteRenderer == null) return;

        if (data != null && data.miniChara != null)
        {
            spriteRenderer.sprite = data.miniChara;
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.sprite = null;
            spriteRenderer.enabled = false;
        }
    }
}

