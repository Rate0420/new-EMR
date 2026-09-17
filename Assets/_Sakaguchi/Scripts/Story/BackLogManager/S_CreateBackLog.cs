using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class S_CreateBackLog : MonoBehaviour
{
    [SerializeField] private GameObject content;    // プレハブを生成する親オブジェクト
    [SerializeField] private GameObject logPre;     // ログのプレハブ

    private S_StoryData storyData;

    private HashSet<int> createdIndexes = new HashSet<int>();

    private void Awake()
    {
        storyData = S_DontDestroyStory.instance.story;
    }

    /// <summary>
    /// 指定インデックスのログを生成する。WriteText の CorDrawText から呼ぶ。
    /// 同じインデックスは1回だけ生成する。
    /// </summary>
    public void CreateLog(int index)
    {
        if (createdIndexes.Contains(index)) return;
        createdIndexes.Add(index);

        var entry = storyData.Get(index);

        GameObject obj = Instantiate(logPre, content.transform);

        var messageText = obj.transform.Find("MassageText")?.GetComponent<TextMeshProUGUI>();
        var nameText = obj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();

        if (messageText != null) messageText.text = entry.text;
        if (nameText != null) nameText.text = (entry.charName == "none") ? "" : entry.charName ?? "";
    }

    /// <summary> ログを全消去する（シーン再読み込み時などに使用）</summary>
    public void ClearLogs()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);
        createdIndexes.Clear();
    }
}
