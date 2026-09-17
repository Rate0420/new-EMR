using System.Collections;
using TMPro;
using UnityEngine;

public class WriteText : MonoBehaviour
{
    [SerializeField] private StoryData storyData;           // スクリプタブルオブジェクト
    [SerializeField] private SetStoryUI setStoryUI;         // SetStoryUIスクリプト
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト
    [SerializeField] private BackLogButton backLogButton;   // BackLogButtonスクリプト
    [SerializeField] private CreateBackLog createBackLog;   // CreateBackLogスクリプト

    [SerializeField] private GameObject image;              // Aボタンの画像
    [SerializeField] private GameObject fastUI;             // 早送りの画像、テキスト

    [SerializeField] private TextMeshProUGUI messageText;   // メッセージテキスト
    [SerializeField] private Animator anim;                 // アニメーター
    [SerializeField] private string[] scene;                // シーン名

    private const float NORMAL_SPEED = 0.1f;                // 通常時の文字送りの速さ
    private const float FAST_SPEED = 0.04f;                 // 早送り時の文字送りの速さ

    private float textSpeed = NORMAL_SPEED;                 // 文字送りの速さ

    private bool isSceneChange; // シーン遷移の判別
    private bool isFast;        // 早送りかどうかの判別
    private bool isDrawing;     // メッセージを書いているどうかの判別(連打防止)

    public int index = 0;

    private void Start()
    {
        storyData = DontDestroyStory.instance.story;
        anim = fastUI.GetComponent<Animator>();     // 早送りの画像、テキストからアニメーターを取得
        scene = storyData.sceneName;
        setStoryUI.ChangeNameAndSprite();
        DrawText();
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// 文字を画面に反映させる
    /// </summary>
    public void HandleInput()
    {
        // 選択肢と会話履歴ログが非表示のときのみ文字表示
        if (chooseManager.isEvent) return;
        if (backLogButton.isBackLog) return;
        //
        // とりあえず||使ってキーマウに対応させる
        //
        // 左クリックorSpaceキー
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // 直前に会話履歴を表示していたら実行しない
            if (backLogButton.isClick)
            {
                backLogButton.isClick = false;
                return;
            }

            DrawText();
        }
        // 早送り右クリックor左シフトキー
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            ChangeFastMode(!isFast);
        }
    }

    /// <summary>
    /// 文字を書き出す
    /// </summary>
    private void DrawText()
    {
        if (isDrawing)  // 文字書き出し中
        {
            CompleteCurrentMessage();
            return;
        }
        if (index < storyData.text.Length)  // 全ての文字を出し終わったとき
        {
            StartNextMessage();
            return;
        }
        ChangeScene();
    }
    /// <summary>
    /// 文字をすべて表示
    /// </summary>
    private void CompleteCurrentMessage()
    {
        

        // 文字を出している最中に連打された場合は、文字をすべて表示する
        StopAllCoroutines();                        // コルーチンを停止
        messageText.text = storyData.text[index];   // すべての文字を表示
        createBackLog.CreateLog(index);             // 会話履歴のログ作成
        image.SetActive(true);                      // Aボタンの画像を表示する
        index++;

        isDrawing = false;
    }
    /// <summary>
    /// 次のインデックスの文字を表示
    /// </summary>
    private void StartNextMessage()
    {
        image.SetActive(false);     // Aボタンの画像を非表示にする

        StopAllCoroutines();        // 連打対策
        messageText.text = "";      // 初期化
        StartCoroutine(CorDrawText(storyData.text[index])); // 文字送り
    }
    /// <summary>
    /// シーン遷移
    /// </summary>
    private void ChangeScene()
    {
        if (isSceneChange) return;
        anim.SetBool("ScaleBool", false);   // アニメーション停止

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            FadeSceneChanger.ChangeScene(scene[0]);
            isSceneChange = true;
        }
    }

    /// <summary>
    /// 1文字ずつ表示するためのコルーチン
    /// </summary>
    private IEnumerator CorDrawText(string message)
    {
        if (isDrawing) yield break;

        isDrawing = true;
        setStoryUI.ChangeNameAndSprite();
        float time = 0f;
        while (true)
        {
            // バックログ表示中は一時停止
            if (backLogButton.isBackLog)
            {
                yield return null;
                continue;
            }

            yield return null;
            time += Time.deltaTime;
            int length = Mathf.FloorToInt(time / textSpeed);
            if (length > message.Length) break;
            messageText.text = message.Substring(0, length);
        }

        messageText.text = message;

        createBackLog.CreateLog(index);     // 会話履歴のログ作成

        yield return null;

        index++;

        image.SetActive(true);  // Aボタンの画像を表示する
        isDrawing = false;
    }

    /// <summary>
    /// 早送り
    /// </summary>
    /// <param name="fast"></param>
    private void ChangeFastMode(bool fast)
    {
        isFast = fast;
        textSpeed = fast ? FAST_SPEED : NORMAL_SPEED;   // trueなら早送り、falseなら通常速度
        anim.SetBool("ScaleBool", fast);    // アニメーション
    }
}
