using UnityEngine;

public class TitleVoice : MonoBehaviour
{
    [SerializeField] private AudioClip[] startVoice;
    [SerializeField] private AudioSource voiceSource;

    private int index;

    public void StartVoice()
    {
        voiceSource.clip = startVoice[index];
        voiceSource.Play();
    }
}
