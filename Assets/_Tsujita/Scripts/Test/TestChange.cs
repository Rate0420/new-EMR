using UnityEngine;
using UnityEngine.UI;

public class TestChange : MonoBehaviour
{
    [SerializeField] private TitleFade fade;
    [SerializeField] private CharacterData[] characterData;

    [SerializeField] private Image c_Image;

    [SerializeField] private GameObject fadeCanvas;

    public void StartButton()
    {
        Debug.Log("02");
        fadeCanvas.SetActive(true);
        int index = Random.Range(0, characterData.Length);
        Debug.Log(index);
        c_Image.sprite = characterData[index].cutinSprite;
    }
}
