using UnityEngine;

public class SuperLuckyWater_process : MonoBehaviour
{
    // 次回のスロットを確変当たりにするか
    private bool forceChanceWin = false;


    /// <summary>
    /// スーパーラッキーウォーターを使用
    /// </summary>
    public void Activate()
    {
        forceChanceWin = true;

        Debug.Log("【スーパーラッキーウォーター】次回のスロットを確変当たりにします");
    }


    /// <summary>
    /// スーパーラッキーウォーターが有効か確認
    /// </summary>
    public bool IsActive()
    {
        return forceChanceWin;
    }


    /// <summary>
    /// 効果を消費
    /// </summary>
    public void Consume()
    {
        forceChanceWin = false;

        Debug.Log("【スーパーラッキーウォーター】効果を消費しました");
    }
}