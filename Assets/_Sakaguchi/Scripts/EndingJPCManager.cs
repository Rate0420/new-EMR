using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndingJPCManager : MonoBehaviour
{
    // 最終盤でのエンディングのJPCを管理するスクリプト
    [SerializeField] CroonHole[] endingJPCHoles; // エンディングのJPCの配列
    int likeAbbility = 0; // 好感度の値
    string Routename = ""; // 好感度を取得するキャラクターの名前

    // 既にJPCHoleのisGoodEndHoleがtrueになっている穴がある場合、同じ穴を再度trueにしないようにするためのリスト
    List<int> goodEndHoleIndices = new List<int>();

    // likeAbbilityの値に応じて、JPCHoleの穴の状態を変更する。
    // 10個ある穴のうち、likeAbbilityが10につき1つの穴のisGoodEndHoleをtrueにする。
    // 何番目がtrueになるかは、ランダムでやる

    [SerializeField] GameObject Ball;
    [SerializeField] Transform BallSpawnPos;
    [SerializeField] S_StoryData[] EndingstoryData;
    [SerializeField] S_StoryData[] GoodEndingstoryData;

    void Start()
    {
        switch (PlayerPrefs.GetInt("RouteNo"))
        {
            case 5:
                Routename = "Liselotte";
                break;
            case 6:
                Routename = "Sayo";
                break;
        }

        Debug.Log("BaseLike" + " " + S_AffinityManager.Instance.Get(Routename, "likeability"));
        likeAbbility = S_AffinityManager.Instance.Get(Routename, "likeability");
        Debug.Log("likeAbbility: " + likeAbbility);
        

        // likeAbbilityの値に応じて、JPCHoleの穴の状態を変更する。
        int goodEndHoleCount = likeAbbility / 10; // 好感度が10につき1つの穴をtrueにする
        for (int i = 0; i < goodEndHoleCount; i++)
        {
            int randomIndex;
            do
            {
                Debug.Log("aaaaaaa");
                randomIndex = Random.Range(0, endingJPCHoles.Length);
            } while (goodEndHoleIndices.Contains(randomIndex)); // 既にtrueになっている穴のインデックスは避ける
            Debug.Log("iiiiiiiii");
            endingJPCHoles[randomIndex].isGoodEndHole = true;
            goodEndHoleIndices.Add(randomIndex); // trueにした穴のインデックスをリストに追加
            endingJPCHoles[randomIndex].SetText("Best");
        }

        // trueではない穴のテキストを"Normal"にする
        for (int i = 0; i < endingJPCHoles.Length; i++)
        {
            if (!endingJPCHoles[i].isGoodEndHole)
            {
                endingJPCHoles[i].SetText("Normal");
            }
        }

        Instantiate(Ball, BallSpawnPos.position, BallSpawnPos.rotation);
    }
    public void JPCEnd(int no)
    {
        // no番目の穴にボールが入ったときの処理
        // もしno番目の穴がisGoodEndHoleなら、trueにする
        if (endingJPCHoles[no].isGoodEndHole)
        {
            Debug.Log("Good End Hole!");
            // 好感度が高いエンディングの処理をここに書く
            S_DontDestroyStory.instance.story = GoodEndingstoryData[PlayerPrefs.GetInt("RouteNo")];
            SceneManager.LoadScene("Sakaguchi_TestStoryScene");
        }
        else
        {
            Debug.Log("Normal End Hole.");
            // 通常のエンディングの処理をここに書く
            S_DontDestroyStory.instance.story = EndingstoryData [PlayerPrefs.GetInt("RouteNo")];
            SceneManager.LoadScene("Sakaguchi_TestStoryScene");
        }
    }
}
