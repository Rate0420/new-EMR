using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EMR.Core;

public class SceneChanger : MonoBehaviour
{
    private GamePause gamePause;

    [SerializeField] GameObject MedalRoot;
    [SerializeField] PanelAniZoom panelAniZoom;
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject BlackOutImage;
    [SerializeField] RoundChange roundChange;
    [SerializeField] StatusGet statusGet;
    public bool IsTransitioning = false;

    [SerializeField] private MenuManager menuManager;

    bool isStartmenuprocessing = false;

    [SerializeField] S_StoryData[] MiniEvents;  // ミニイベントのシナリオデータを格納する配列。プロローグ後に攻略キャラを選んだ際に格納する事

    // メニュー・シナリオ終了時にpauseRequestedを解除する
    [SerializeField] float returnDelay = 2.0f; // Inspectorで調整可能

    // シナリオ・メニューが開いている間trueになるフラグ
    public bool IsSceneActive { get; private set; } = false;

    private bool isEndingScenario = false;

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
    }

    private void Update()
    {

    }

    public void OnMenuButton()
    {
        if (!isStartmenuprocessing)
        {
            isStartmenuprocessing = true;
            menuManager.isAllMF = true;
            StartCoroutine(StartMenu());
        }
    }

    public void StartScenario(string sceneName)
    {
        if (IsTransitioning)
            return;

        StartCoroutine(StartScenarioCoroutine(sceneName));
    }


    IEnumerator StartMenu()
    {
        IsSceneActive = true;

        // リールが止まっていて、他の誰もロックを持っていない状態になるまで自動的に待つ
        // (JPC払い出し・シナリオ・ラウンドチェンジ・ミニイベント中などは待たされる)
        yield return GameState.Instance.GameLock.Acquire("Menu");

        gamePause.ChangePause(true);
        panelAniZoom.isGameScene = true;
        menuCanvas.SetActive(true);
        BlackOutImage.SetActive(true);
        panelAniZoom.MenuPanelChange();

        yield return new WaitForSeconds(1);
        MedalRoot.SetActive(false);
        isStartmenuprocessing = false;
    }

    IEnumerator StartScenarioCoroutine(string sceneName)
    {
        IsTransitioning = true;
        IsSceneActive = true;

        yield return null;

        yield return GameState.Instance.GameLock.Acquire("Scenario");

        gamePause.ChangePause(true);

        yield return SceneManager.LoadSceneAsync(
            sceneName,
            LoadSceneMode.Additive
        );

        MedalRoot.SetActive(false);

        IsTransitioning = false;
    }
    public IEnumerator StartShopCoroutine()
    {
        IsSceneActive = true;

        yield return GameState.Instance.GameLock.Acquire("Shop");
        // TODO: ショップを閉じる処理側で GameState.Instance.GameLock.Release("Shop") を呼ぶこと
        //       (該当スクリプトが手元に無かったため、ここでは組み込んでいません)

        gamePause.ChangePause(true);
        MedalRoot.SetActive(false);
    }

    public void EndScenario(string sceneName)
    {
        if (IsTransitioning)
            return;

        StartCoroutine(EndScenarioCoroutine(sceneName));
    }

    public IEnumerator EndScenarioCoroutine(string sceneName)
    {
        if (isEndingScenario)
            yield break;

        isEndingScenario = true;
        IsTransitioning = true;

        roundChange.ResetMethod();

        yield return SceneManager.UnloadSceneAsync(sceneName);

        // 確変時なら番号４、通常時なら番号３
        if (!slotManager.Kakuhen)
            BGMManager.Instance.BGMChange(3);
        else
            BGMManager.Instance.BGMChange(4);

        MedalRoot.SetActive(true);

        yield return new WaitForSeconds(returnDelay);

        gamePause.ChangePause(false);
        IsSceneActive = false;

        GameState.Instance.GameLock.Release("Scenario");

        roundChange.CompleteRoundChange();

        IsTransitioning = false;
        isEndingScenario = false;
    }
    IEnumerator EndMenuCoroutine()
    {
        MedalRoot.SetActive(true);

        yield return new WaitForSeconds(returnDelay);

        gamePause.ChangePause(false);
        IsSceneActive = false; // ← 終了時にfalse

        GameState.Instance.GameLock.Release("Menu");
    }

    public void StartMiniEvent()
    {
        S_StoryData storyData = MiniEvents[Random.Range(0, MiniEvents.Length)];
        S_DontDestroyStory.instance.story = storyData;
        statusGet.miniStory = statusGet.miniStory ++;
        StartCoroutine(StartScenarioCoroutine("Sakaguchi_TestStoryScene"));
    }

    [SerializeField] SlotManager slotManager;


    public void EndMenu()
    {
        menuManager.isAllMF = false;
        StartCoroutine(EndMenuCoroutine());
    }
}
