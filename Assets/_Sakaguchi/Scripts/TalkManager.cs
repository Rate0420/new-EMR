using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TalkManager : MonoBehaviour
{
    // 会話内容、会話キャラ名、顔画像を一つのクラスにまとめる
    [System.Serializable]
    public class TalkBoxData
    {
        public string TextBody;
        public string TextName;
        public Sprite TalkFace; 
        // そのうち音声も追加
        public float TalkLength; // 音声の長さ
    }

    // talkboxdataを二つで管理するクラス
    [System.Serializable]
    public class TalkBoxPair
    {
        public TalkBoxData Talk1;
        public TalkBoxData Talk2;
    }

    [SerializeField] TalkBoxPair[] TalkBoxPairs;

    [SerializeField] GameObject TalkBox1;
    [SerializeField] GameObject TalkBox2;

    [SerializeField] TextMeshPro TextBody1;
    [SerializeField] TextMeshPro TextBody2;
    [SerializeField] TextMeshPro TextName1;
    [SerializeField] TextMeshPro TextName2;
    [SerializeField] GameObject TalkFace1;
    [SerializeField] GameObject TalkFace2;

    public float SetTalkBox(int num)
    {
        TextName1.text = TalkBoxPairs[num].Talk1.TextName;
        TextBody1.text = TalkBoxPairs[num].Talk1.TextBody;
        // faceはquadなのでmaterialのmainTextureにセットする
        TalkFace1.GetComponent<Renderer>().material.mainTexture = TalkBoxPairs[num].Talk1.TalkFace.texture;
        if (TalkBoxPairs[num].Talk2.TextBody != "")
        {
            TextName2.text = TalkBoxPairs[num].Talk2.TextName;
            TextBody2.text = TalkBoxPairs[num].Talk2.TextBody;
            TalkFace2.GetComponent<Renderer>().material.mainTexture = TalkBoxPairs[num].Talk2.TalkFace.texture;
        }
        StartCoroutine(ShowTalkBox(num));
        return TalkBoxPairs[num].Talk1.TalkLength + TalkBoxPairs[num].Talk2.TalkLength;
    }

    IEnumerator ShowTalkBox(int num)
    {
        TalkBox1.SetActive(true);
        yield return new WaitForSeconds(TalkBoxPairs[num].Talk1.TalkLength);
        if (TalkBoxPairs[num].Talk2.TextBody != "")
        {
            TalkBox2.SetActive(true);
            yield return new WaitForSeconds(TalkBoxPairs[num].Talk2.TalkLength);
        }
        TalkBox1.SetActive(false);
        TalkBox2.SetActive(false);
    }

    public int searchTalkBoxNum(string text)
    {
        // textの内容とtalkNameが一致する物が含まれるTalkBoxPairsを探す
        // まず該当条件のTalkBoxPairsをリストに入れる

        List<TalkBoxPair> candidates = new List<TalkBoxPair>();
        foreach (TalkBoxPair pair in TalkBoxPairs)
        {
            if (pair.Talk1.TextName.Contains(text) || pair.Talk2.TextName.Contains(text))
            {
                candidates.Add(pair);
            }
        }

        // リストの中からランダムに一つ選び、そのindexを返す
        int rand = Random.Range(0, candidates.Count);
        return System.Array.IndexOf(TalkBoxPairs, candidates[rand]);
    }


}
