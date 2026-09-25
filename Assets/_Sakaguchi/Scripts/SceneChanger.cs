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

    bool isStartmenuprocessing = false;

    [SerializeField] S_StoryData[] MiniEvents;  // ミニイベントのシナリオデータを格納する配列。プロローグ後に攻略キャラを選んだ際に格納する事

    // メニュー・シナリオ終了時にpauseRequestedを解除する
    [SerializeField] float returnDelay = 2.0f; // Inspectorで調整可能

    // シナリオ・メニューが開いている間trueになるフラグ
    public bool IsSceneActive { get; private set; } = false;

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartScenario("Sakaguchi_TestStoryScene");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(EndScenarioCoroutine("Sakaguchi_TestStoryScene"));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {

            if (!isStartmenuprocessing)
            {
                isStartmenuprocessing = true;
                StartCoroutine(StartMenu());
            }
        }
    }

    public void StartScenario(string sceneName)
    {
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
        IsSceneActive = true;
        yield return null;

        yield return GameState.Instance.GameLock.Acquire("Scenario");

        gamePause.ChangePause(true);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        MedalRoot.SetActive(false);
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

    public IEnumerator EndScenarioCoroutine(string sceneName)
    {
        roundChange.ResetMethod(); // ラウンドチェンジ由来のシナリオだった場合、UI(キャンバス)だけ先に片付ける
        yield return SceneManager.UnloadSceneAsync(sceneName);
        MedalRoot.SetActive(true);

        yield return new WaitForSeconds(returnDelay);

        gamePause.ChangePause(false);
        IsSceneActive = false; // ← 終了時にfalse

        // ミニイベント等、"Scenario"ロックを持っていた場合はここで解放する
        // (ラウンドチェンジ由来で"Scenario"を持っていなかった場合は何も起きない)
        GameState.Instance.GameLock.Release("Scenario");

        // "RoundChange"ロックの解放は、後片付けが全部終わった一番最後に行う
        // (ラウンドチェンジ由来でなかった場合は何も起きない)
        roundChange.CompleteRoundChange();
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
        StartCoroutine(StartScenarioCoroutine("Sakaguchi_TestStoryScene"));
    }

    [SerializeField] SlotManager slotManager;


    public void EndMenu()
    {
        StartCoroutine(EndMenuCoroutine());
    }



}
