using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EXPText : MonoBehaviour
{
    [SerializeField] private CharacterData[] cData;
    [SerializeField] private MenuManager menuManager;

    [SerializeField] private TextMeshProUGUI[] nameText;  // 名前
    [SerializeField] private TextMeshProUGUI expText; // 解説
    [SerializeField] private Image cImage;              // 画像
    [SerializeField] private GameObject expPanel;       // 解説パネル

    private LAAni laAni;  // 拡大縮小用アニメーション

    public int nowStoryNo;  // 現在のストーリー番号（仮）

    private void Awake()
    {
        laAni = expPanel.GetComponent<LAAni>();
    }

    // 名前・解説・画像の反映
    public void ShowCharacter(int index)
    {
        laAni.PanelZoomIn();

        if (index < 0 || index >= cData.Length)
            return;

        CharacterData data = cData[index];

        nameText[0].text = data.characterName;
        nameText[1].text = data.characterName_JP;
        // ルートキャラかつ、特定の進行度まで進んだら解説2を開放
        if (nowStoryNo >= data.expStoryNo && data.charactorType == menuManager.currentRoute)
            expText.text = data.charaExp[0] + "\n" + data.charaExp[1];
        else
            expText.text = data.charaExp[0];
        cImage.sprite = data.sprites[0];
    }

    public void PanelFalse()
    {
        laAni.PanelZoomOut();
    }
}
