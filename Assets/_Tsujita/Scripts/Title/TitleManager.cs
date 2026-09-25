using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TitleVoice voice;  // ボイスの再生
    [SerializeField] private CharacterData[] characterData; // キャラクターデータ
    [SerializeField] private VolumeSet volumeSet;

    [SerializeField] private AudioSource bgmAudio;      // BGM
    [SerializeField] private AudioSource startVoice;    // キャラボイス

    private void Start()
    {
        SetUpStart();
        BGMManager.Instance.BGMChange(0);
        volumeSet.VolumeSetScene();
    }

    /// <summary>
    /// タイトルシーンに遷移したら呼び出す
    /// </summary>
    public void SetUpStart()
    {
    }

    /// <summary>
    /// スタートボタンを押したら呼び出す
    /// </summary>
    public void StartButton()
    {
        // タイトルからの遷移はルートキャラ以外も出現
        int index = Random.Range(0, characterData.Length);
        Debug.Log(characterData[index].characterName_JP);

        // カットイン画像の変更
        TitleFade.Instance.ImageChange(characterData[index].cutinSprite);
        // ボイスの再生
        startVoice.clip = characterData[index].startVoice;
        startVoice.PlayOneShot(startVoice.clip);

        TitleFade.Instance.SceneChangeAni();
    }
}
