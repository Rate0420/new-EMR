using UnityEngine;

public class SendChooseData : MonoBehaviour
{
    public string route;   // 選択肢のルート
    public string choose;   // 選択肢の内容

    /// <summary>
    /// 選択情報を渡す
    /// </summary>
    public void SendData(string sendRoute, string sendChoose)
    {
        Debug.Log($"選択データ: {sendRoute}  キャラ好感度： {sendChoose}");
        
        route = sendRoute;     // ルートを入れる

        choose = sendChoose;   // 選択肢の内容を入れる
    }
}
