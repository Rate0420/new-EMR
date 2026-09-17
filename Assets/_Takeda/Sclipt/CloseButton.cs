using System.Collections;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public GameObject shopPanel;
    public SceneChanger changer;

    void Update()
    {
        // Shiftキーで表示・非表示切り替え
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //shopPanel.SetActive(!shopPanel.activeSelf);

            if(shopPanel.activeSelf) CloseShop();
            else OpenShop();
        }
    }

    // ボタンで閉じる
    public void CloseShop()
    {
        changer.EndMenu();
        shopPanel.SetActive(false);
    }

    IEnumerator OpenShopCoroutine()
    {
        yield return changer.StartShopCoroutine();
        shopPanel.SetActive(true);
    }

    // ボタンで開く
    public void OpenShop()
    {
        //shopPanel.SetActive(true);
        StartCoroutine(OpenShopCoroutine());
    }
}