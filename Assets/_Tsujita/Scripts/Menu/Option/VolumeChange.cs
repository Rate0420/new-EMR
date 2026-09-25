using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChange : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;

    private float setBGMVolume;   // BGM保存音量
    private float setSEVolume;    // SE保存音量
    private float setVoiceVolume; // Voice保存音量

    // 各種スライダー
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Slider voiceSlider;

    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private TextMeshProUGUI bgmText, seText, voiceText;

    private void Start()
    {
        VolumeSetStart();
        bgmSlider.value = setBGMVolume;
        seSlider.value = setSEVolume;
        voiceSlider.value = setVoiceVolume;

        bgmSlider.onValueChanged.AddListener(BGMSetVolume);
        seSlider.onValueChanged.AddListener(SESetVolume);
        voiceSlider.onValueChanged.AddListener(VoiceSetVolume);

        BGMSetVolume(bgmSlider.value);
        SESetVolume(seSlider.value);
        VoiceSetVolume(voiceSlider.value);
    }

    /// <summary>
    /// 保存された音量設定のセット
    /// </summary>
    private void VolumeSetStart()
    {
        setBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 5);
        setSEVolume = PlayerPrefs.GetFloat("SEVolume", 5);
        setVoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 5);

        BGMManager.Instance.SetVolume(setBGMVolume);
        SEManagerMenu.Instance.seSource.volume = setSEVolume / 10f;
        voiceSource.volume = setVoiceVolume / 10f;

        bgmText.text = setBGMVolume.ToString();
        seText.text = setSEVolume.ToString();
        voiceText.text = setVoiceVolume.ToString();
    }

    // BGMの変更
    public void BGMSetVolume(float value)
    {
        setBGMVolume = value;
        BGMManager.Instance.SetVolume(value);
        bgmText.text = setBGMVolume.ToString();
    }
    // SEの変更
    public void SESetVolume(float value)
    {
        SEManagerMenu.Instance.seSource.volume = value / 10;
        setSEVolume = seSlider.value;
        seText.text = setSEVolume.ToString();
    }
    // Voiceの変更
    public void VoiceSetVolume(float value)
    {
        voiceSource.volume = value / 10f;
        setVoiceVolume = voiceSlider.value;
        voiceText.text = setVoiceVolume.ToString();
    }

    // 値の保存
    public void VolumeSave()
    {
        PlayerPrefs.SetFloat("BGMVolume", setBGMVolume);
        PlayerPrefs.SetFloat("SEVolume", setSEVolume);
        PlayerPrefs.SetFloat("VoiceVolume", setVoiceVolume);

        // 音量変更パネルの非表示
        menuManager.OnMeunButtons(0);
    }
}
