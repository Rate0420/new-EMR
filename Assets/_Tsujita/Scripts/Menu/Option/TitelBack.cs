using UnityEngine;

public class TitelBack : MonoBehaviour
{
    [SerializeField] private AllSave allSave;
    [SerializeField] private StatusGet statusGet;
    [SerializeField] private AudioSource voiceSource;

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    public void TitleBackButton(int buttonNo)
    {
        TitleFade.Instance.ImageChange(statusGet.characterData.cutinSprite);
        voiceSource.clip = statusGet.characterData.endVoice;

        switch (buttonNo)
        {
            case 1: // セーブして終了
                voiceSource.PlayOneShot(voiceSource.clip);
                allSave.isTitle = true;
                allSave.AllSeve();
                TitleFade.Instance.SceneChangeAni();
                break;
            case 2: // セーブしないで終了
                voiceSource.PlayOneShot(voiceSource.clip);
                TitleFade.Instance.SceneChangeAni();
                break;
        }
    }
}
