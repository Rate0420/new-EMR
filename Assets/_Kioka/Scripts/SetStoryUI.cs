using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetStoryUI : MonoBehaviour
{
    [SerializeField] private StoryData storyData;             // スクリプタブルオブジェクト
    [SerializeField] private WriteText writeText;             // writeTextスクリプト

    [SerializeField] private TextMeshProUGUI nameText;  // キャラの名前テキスト
    [SerializeField] private Image char1Img;            // キャラ1画像
    [SerializeField] private Image char2Img;            // キャラ2画像
    [SerializeField] private Image bgImg;               // 背景画像

    private void Start()
    {
        storyData = DontDestroyStory.instance.story;
    }

    /// <summary>
    /// 表示されているメッセージテキストに応じてキャラの名前と画像を変更する
    /// </summary>
    public void ChangeNameAndSprite()
    {
        SetCharName(writeText.index);
        SetCharSprite(writeText.index, writeText.index);
        SetBGImg(writeText.index);
        SetSound(writeText.index);
    }

    /// <summary>
    /// キャラの名前をテキストに追加
    /// </summary>
    /// <param name="name"></param>
    void SetCharName(int name)
    {
        if (name < 0 || name >= storyData.name.Length) return;
        if (storyData.name[name] != null)
        nameText.text = storyData.name[name];
        if (storyData.name[name] == "none") nameText.text = "";  // 名前が「none」の場合は名前テキストを空にする
    }

    /// <summary>
    /// キャラのスプライトを設定
    /// </summary>
    /// <param name="char1"></param>
    /// <param name="char2"></param>
    void SetCharSprite(int char1, int char2)
    {
        if(char1 < 0 || char1 >= storyData.char1Sprite.Length) return;
        if (storyData.char1Sprite[char1] != null) 
        char1Img.sprite = storyData.char1Sprite[char1];
        if(storyData.char1Sprite[char1] == null) char1Img.color = new Color(1, 1, 1, 0);  // スプライトが設定されていない場合は画像を透明にする
        else char1Img.color = new Color(1, 1, 1, 1);  // スプライトが設定されている場合は画像を表示する
        if (char2 < 0 || char2 >= storyData.char2Sprite.Length) return;
        if (storyData.char2Sprite[char2] != null) 
        char2Img.sprite = storyData.char2Sprite[char2];
        if (storyData.char2Sprite[char2] == null) char2Img.color = new Color(1, 1, 1, 0);  // スプライトが設定されていない場合は画像を透明にする
        else char2Img.color = new Color(1, 1, 1, 1);  // スプライトが設定されている場合は画像を表示する
    }

    /// <summary>
    /// 背景のスプライトを設定
    /// </summary>
    /// <param name="number"></param>
    void SetBGImg(int number)
    {
        if (number < 0 || number >= storyData.bgSprite.Length) return;
        if (storyData.bgSprite[number] == null) return;  // スプライトが設定されていない場合は処理しない
        bgImg.sprite = storyData.bgSprite[number];
    }

    void SetSound(int number)
    {
        if(number < 0 || number >= storyData.bgm.Length) return;
        if (storyData.bgm[number] != null) 
            //SoundManager.Instance.PlayBGM_Story(storyData.bgm[number]);
        //else SoundManager.Instance.StopBGM();
        if (number < 0 || number >= storyData.se.Length) return;
        if (storyData.se[number] != null)
            //SoundManager.Instance.PlaySE_Story(storyData.se[number]);
        if (number < 0 || number >= storyData.voice.Length) return;
        Debug.Log("SetSound: " + number);
        if (storyData.voice[number] != null)
        {
            Debug.Log("PlayVoice: " + storyData.voice[number].name);
            //SoundManager.Instance.PlayVoice_Story(storyData.voice[number]);
        }
    }
}
