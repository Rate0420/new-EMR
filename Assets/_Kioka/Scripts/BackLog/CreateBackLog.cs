using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CreateBackLog : MonoBehaviour
{
    [SerializeField] private StoryData storyData;   // StoryDataスクリプタブルオブジェクト

    [SerializeField] private GameObject content;    // プレハブを生成する親オブジェクト
    [SerializeField] private GameObject logPre;     // ログのプレハブ

    private TextMeshProUGUI nameText;               // 名前テキスト
    private TextMeshProUGUI messageText;            // メッセージテキスト

    private string[] name;  // 名前配列
    private string[] text;  // メッセージ配列

    private HashSet<int> createLogs = new HashSet<int>();   // 使われたindexを入れる

    void Start()
    {
        // スクリプタブルオブジェクトから取得
        name = storyData.name;
        text = storyData.text;
    }

    /// <summary>
    /// 会話履歴ログを生成
    /// </summary>
    /// <param name="index"></param>
    public void CreateLog(int index)
    {
        // 既にログ作成済みのインデックスなら処理しない
        if (createLogs.Contains(index)) return;
        // 初回のみ処理できるようインデックスを処理済みにする
        createLogs.Add(index);

        // ログ生成
        GameObject obj = Instantiate(logPre, content.transform);    

        // プレハブからテキスト取得
        messageText = obj.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
        nameText = obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        // テキスト更新
        messageText.text = storyData.text[index];
        nameText.text = storyData.name[index];
    }
}
