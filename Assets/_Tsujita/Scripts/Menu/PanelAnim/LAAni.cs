using UnityEngine;
using System.Collections;

public class LAAni : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    [SerializeField] private Vector2 zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 zoomOutPos = Vector2.zero;

    [SerializeField] private float duration;

    /// <summary>
    /// パネルの拡大
    /// </summary>
    public void PanelZoomIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleAnimation(zoomOutPos, zoomInPos));
    }

    /// <summary>
    /// メニューパネルの表示切替
    /// </summary>
    public void PanelZoomOut()
    {
        StartCoroutine(CloseAnimation());
    }

    /// <summary>
    /// 暗転→暗転解除
    /// </summary>
    private IEnumerator CloseAnimation()
    {
        yield return ScaleAnimation(zoomInPos, zoomOutPos);
    }

    private IEnumerator ScaleAnimation(Vector3 start, Vector3 end)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            panel.localScale = Vector3.Lerp(start, end, t);

            yield return null;
        }

        panel.localScale = end;
    }
}
