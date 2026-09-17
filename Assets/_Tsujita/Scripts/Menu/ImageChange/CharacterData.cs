using UnityEngine;

[CreateAssetMenu(fileName = "CharacterImageDate", menuName = "Game/CharacterImage Data")]
public class CharacterData : ScriptableObject
{
    public CharactorType charactorType;

    public string characterName;    // キャラクターの名前
    public string characterName_JP;

    public Sprite[] sprites;    // メイン画像の立ち絵
    public AudioClip[] voice;   // ボイス
    public string[] menuTexts;  // メニュー用テキスト
    public string[] charaExp;   // キャラ解説テキスト用

    public int likeability;     // 好感度
    public int expStoryNo;      // 解説を追加するタイミング

    public Sprite cutinSprite;  // カットイン用画像

    public AudioClip startVoice;    // ゲーム開始時のボイス
    public AudioClip endVoice;      // ゲーム終了時のボイス
}
