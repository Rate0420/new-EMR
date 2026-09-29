using UnityEngine;

public class VolumeSet : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource, seSource, voiceSource;

    private float setBGMVolume;   // BGMï€ë∂âπó 
    private float setVoiceVolume; // Voiceï€ë∂âπó 

    /// <summary>
    /// ï€ë∂Ç≥ÇÍÇΩâπó ê›íËÇÃÉZÉbÉg
    /// </summary>
    public void VolumeSetScene()
    {
        setBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 5);
        setVoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 5);

        bgmSource.volume = setBGMVolume / 10f;
        voiceSource.volume = setVoiceVolume / 10f;
    }
}
