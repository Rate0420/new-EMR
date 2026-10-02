using UnityEngine;

public class SST : MonoBehaviour
{

    [SerializeField] StatusGet status;
    int testNo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testNo = 1;
        PlayerPrefs.SetInt("RouteNo", testNo);
        MenuManager.Instance.currentRoute = CharactorType.RoufonEnergeiaVermilion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RouteChara01()
    {
        testNo = 7;
        PlayerPrefs.SetInt("RouteNo", testNo);

    }

    public void RouteChara02()
    {
        testNo = 6;
        MenuManager.Instance.currentRoute = CharactorType.MikumoSayo;
    }
}
