using UnityEngine;

public class CharaRoutr : MonoBehaviour
{
    private int routeNo;

    public void RouteChara01()
    {
        routeNo = 7;
        PlayerPrefs.SetInt("RouteNo", routeNo);
        TitleFade.Instance.SceneChangeAni();
    }

    public void RouteChara02()
    {
        routeNo = 6;
        PlayerPrefs.SetInt("RouteNo", routeNo);
        TitleFade.Instance.SceneChangeAni();
    }
}
