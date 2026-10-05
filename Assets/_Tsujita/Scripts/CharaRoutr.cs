using UnityEngine;

public class CharaRoutr : MonoBehaviour
{
    private int routeNo;

    public void RouteChara01()// ƒŠ[ƒ[ƒƒbƒe
    {
        routeNo = 5;
        PlayerPrefs.SetInt("RouteNo", routeNo);
        TitleFade.Instance.SceneChangeAni();
    }

    public void RouteChara02()// ¬–é
    {
        routeNo = 6;
        PlayerPrefs.SetInt("RouteNo", routeNo);
        TitleFade.Instance.SceneChangeAni();
    }
}
