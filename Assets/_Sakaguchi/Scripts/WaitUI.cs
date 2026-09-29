using EMR.Core;
using UnityEngine;

public class WaitUI : MonoBehaviour
{
    [SerializeField] GameObject menuWaitText;
    [SerializeField] GameObject miniEventWaitText;
    [SerializeField] GameObject roundChangeWaitText;
    [SerializeField] GameObject jpccWaitText;

    // Update is called once per frame
    void Update()
    {
        menuWaitText.gameObject.SetActive(GameState.Instance.GameLock.IsMenuWaiting);
        miniEventWaitText.gameObject.SetActive(GameState.Instance.GameLock.IsMiniEventWaiting);
        roundChangeWaitText.gameObject.SetActive(GameState.Instance.GameLock.IsRoundChangeWaiting);
        jpccWaitText.gameObject.SetActive(GameState.Instance.GameLock.IsJPCCWaiting);
    }
}
