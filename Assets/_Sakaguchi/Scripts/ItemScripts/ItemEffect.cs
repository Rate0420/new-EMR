using UnityEngine;

// アイテムの効果を定義する抽象クラス
public abstract class ItemEffect : ScriptableObject
{
    /// <summary>
    /// インベントリの内容が変わった時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnInventoryChanged(ItemEffectContext context,int itemlevel) { }

    /// <summary>
    /// メダルを発射した時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnMedalShot(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// メダルが着地した時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="medal"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnMedalLanded(ItemEffectContext context, GameObject medal, int itemlevel) { }

    /// <summary>
    /// メダルが横穴に落ちたとき
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnMedalLost(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// ラウンドがスタート時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnRoundStart(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// ラウンドが終わった時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnRoundEnd(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// スロットが回った時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnSlotRoll(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// スロットで当たった際
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnSlotWin(ItemEffectContext context, int itemlevel) { }

    /// <summary>
    /// 消費アイテム用トリガー
    /// </summary>
    /// <param name="context"></param>
    /// <param name="itemlevel"></param>
    public virtual void OnConsumptionItem(ItemEffectContext context, int itemlevel) { }
}
