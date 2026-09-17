using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// StoryData のエントリに従い、名前・立ち絵・背景・サウンドを更新する。
/// 立ち絵は anchoredPosition で XY オフセットを反映する。
/// </summary>
public class S_SetStoryUI : MonoBehaviour
{
    [SerializeField] private S_StoryData storyData;

    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image char1Img;
    [SerializeField] private Image char2Img;
    [SerializeField] private Image bgImg;

    // 立ち絵の基準座標（インスペクタで初期位置を設定しておく）
    [Header("立ち絵の基準位置")]
    [SerializeField] private Vector2 char1BasePos = Vector2.zero;
    [SerializeField] private Vector2 char2BasePos = Vector2.zero;

    private RectTransform char1Rect;
    private RectTransform char2Rect;

    // ----------------------------------------------------------------

    private void Awake()
    {
        if (char1Img != null) char1Rect = char1Img.GetComponent<RectTransform>();
        if (char2Img != null) char2Rect = char2Img.GetComponent<RectTransform>();

        storyData = S_DontDestroyStory.instance.story;
    }


    // ----------------------------------------------------------------

    /// <summary> 指定エントリのUIをすべて更新する </summary>
    public void Apply(int index)
    {
        if (index < 0 || index >= storyData.Length) return;
        var entry = storyData.Get(index);

        ApplyName(entry);
        ApplyChar1(entry);
        ApplyChar2(entry);
        ApplyBG(entry);
        ApplySound(entry, index);
    }

    // ---- 各更新処理 ------------------------------------------------

    private void ApplyName(S_StoryData.StoryEntry entry)
    {
        if (nameText == null) return;
        nameText.text = (entry.charName == "none") ? "" : entry.charName ?? "";
    }

    private void ApplyChar1(S_StoryData.StoryEntry entry)
    {
        if (char1Img == null) return;

        if (entry.char1Sprite != null)
        {
            char1Img.sprite = entry.char1Sprite;
            char1Img.color = Color.white;
        }
        else
        {
            char1Img.color = new Color(1, 1, 1, 0);
        }

        // XY オフセット適用
        if (char1Rect != null)
            char1Rect.anchoredPosition = char1BasePos + new Vector2(entry.char1OffsetX, entry.char1OffsetY);
    }

    private void ApplyChar2(S_StoryData.StoryEntry entry)
    {
        if (char2Img == null) return;

        if (entry.char2Sprite != null)
        {
            char2Img.sprite = entry.char2Sprite;
            char2Img.color = Color.white;
        }
        else
        {
            char2Img.color = new Color(1, 1, 1, 0);
        }

        // XY オフセット適用
        if (char2Rect != null)
            char2Rect.anchoredPosition = char2BasePos + new Vector2(entry.char2OffsetX, entry.char2OffsetY);
    }

    private void ApplyBG(S_StoryData.StoryEntry entry)
    {
        if (bgImg == null || entry.bgSprite == null) return;
        bgImg.sprite = entry.bgSprite;
    }

    private void ApplySound(S_StoryData.StoryEntry entry, int index)
    {
        // SoundManager が実装されたらコメントを外す
        // if (entry.bgm != null)  SoundManager.Instance.PlayBGM_Story(entry.bgm);
        // else                    SoundManager.Instance.StopBGM();
        // if (entry.se != null)   SoundManager.Instance.PlaySE_Story(entry.se);
        // if (entry.voice != null) SoundManager.Instance.PlayVoice_Story(entry.voice);
       //  Debug.Log($"[SetStoryUI] ApplySound index={index}  bgm={entry.bgm?.name}  se={entry.se?.name}  voice={entry.voice?.name}");
    }
}
