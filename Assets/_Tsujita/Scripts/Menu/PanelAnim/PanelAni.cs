using UnityEngine;
using System.Collections;

public class PanelAni : MonoBehaviour
{
    [SerializeField] private RectTransform panel;   // 対象のパネル
    [SerializeField] private float moveTime;        // 移動時間

    [SerializeField] private Vector2 showPos = Vector2.zero;        // 表示位置
    [SerializeField] protected Vector2 hidePos = new Vector2(0, 0); // 開始位置

    private Coroutine coroutine;

    /// <summary>
    /// 表示
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(MovePanel(hidePos, showPos));
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Close()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(ClosePanel());
    }

    private IEnumerator ClosePanel()
    {
        yield return MovePanel(hidePos);
        gameObject.SetActive(false);
    }

    private IEnumerator MovePanel(Vector2 start, Vector2 end)
    {
        panel.anchoredPosition = start;

        float time = 0;

        while (time < moveTime)
        {
            time += Time.deltaTime;

            panel.anchoredPosition =
                Vector2.Lerp(
                    start,
                    end,
                    time / moveTime);

            yield return null;
        }

        panel.anchoredPosition = end;
    }

    private IEnumerator MovePanel(Vector2 end)
    {
        Vector2 start = panel.anchoredPosition;

        float time = 0;

        while (time < moveTime)
        {
            time += Time.deltaTime;

            panel.anchoredPosition =
                Vector2.Lerp(
                    start,
                    end,
                    time / moveTime);

            yield return null;
        }

        panel.anchoredPosition = end;
    }
}
