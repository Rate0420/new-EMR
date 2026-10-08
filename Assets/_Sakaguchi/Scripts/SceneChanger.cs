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

    [SerializeField] private MenuManager menuManager;

    bool isStartmenuprocessing = false;

    public S_StoryData[] MiniEvents;  // ミニイベントのシナリオデータを格納する配列。プロローグ後に攻略キャラを選んだ際に格納する事

    [SerializeField] S_StoryData[] Liselotte_MiniEvents;
    [SerializeField] S_StoryData[] Sayo_MiniEvents;

    // メニュー・シナリオ終了時にpauseRequestedを解除する
    [SerializeField] float returnDelay = 2.0f; // Inspectorで調整可能

    // シナリオ・メニューが開いている間trueになるフラグ
    public bool IsSceneActive { get; private set; } = false;

    public bool IsTransitioning = false;

    private bool isStartingScenario = false;
    private bool isEndingScenario = false;

    public bool CanStartScenario => !IsSceneActive && !IsTransitioning;

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
        switch (PlayerPrefs.GetInt("RouteNo"))
        {
            case 5:
                MiniEvents = Liselotte_MiniEvents;
                break;
        }
        switch (PlayerPrefs.GetInt("RouteNo"))
        {
            case 6:
                MiniEvents = Sayo_MiniEvents;
                break;
        }
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
        if (IsSceneActive || IsTransitioning ||
            isStartingScenario || isEndingScenario)
        {
            Debug.Log("[SceneChanger] StartScenario拒否");
            return;
        }

        StartCoroutine(StartScenarioCoroutine(sceneName));
    }

    private IEnumerator StartScenarioCoroutine(string sceneName)
    {
        if (isStartingScenario)
            yield break;

        isStartingScenario = true;
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

        isStartingScenario = false;
        IsTransitioning = false;
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
        if (isEndingScenario)
        {
            Debug.Log("[SceneChanger] EndScenario二重実行拒否");
            return;
        }

        StartCoroutine(EndScenarioCoroutine(sceneName));
    }

    private IEnumerator EndScenarioCoroutine(string sceneName)
    {
        if (isEndingScenario)
            yield break;

        isEndingScenario = true;
        IsTransitioning = true;

        roundChange.ResetMethod();

        yield return SceneManager.UnloadSceneAsync(sceneName);

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

        isEndingScenario = false;
        IsTransitioning = false;
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
        if (IsSceneActive || IsTransitioning)
        {
            Debug.Log("[SceneChanger] ミニイベント開始待機中：現在シーンがActive");
            return;
        }

        S_StoryData storyData =
            MiniEvents[Random.Range(0, MiniEvents.Length)];

        S_DontDestroyStory.instance.story = storyData;
        statusGet.miniStory++;

        StartScenario("Sakaguchi_TestStoryScene");
    }

    [SerializeField] SlotManager slotManager;


    public void EndMenu()
    {
        menuManager.isAllMF = false;
        StartCoroutine(EndMenuCoroutine());
    }
}
