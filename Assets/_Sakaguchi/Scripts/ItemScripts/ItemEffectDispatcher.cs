using UnityEngine;

// ItemTriggerEventsを購読し、発火したタイミングで
// PlayerInventory内の全アイテムのItemEffectを呼び出すクラス。
//
// 役割分担:
// ・「何を持っているか」        → PlayerInventory
// ・「イベントが起きたら何をするか」 → こちら(ItemEffectDispatcher)
public class ItemEffectDispatcher : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] ItemEffectContext context;
    [SerializeField] StatusGet statusGet;

    void OnEnable()
    {
        ItemTriggerEvents.OnInventoryChanged += OnInventoryChanged;
        ItemTriggerEvents.OnMedalShot += OnMedalShot;
        ItemTriggerEvents.OnMedalLanded += OnMedalLanded;
        ItemTriggerEvents.OnMedalLost += OnMedalLost;
        ItemTriggerEvents.OnRoundStart += OnRoundStart;
        ItemTriggerEvents.OnRoundEnd += OnRoundEnd;
        ItemTriggerEvents.OnSlotRoll += OnSlotRoll;
        ItemTriggerEvents.OnSlotWin += OnSlotWin;
        ItemTriggerEvents.OnConsumptionItem += OnConsumptionItem;
    }

    void OnDisable()
    {
        ItemTriggerEvents.OnInventoryChanged -= OnInventoryChanged;
        ItemTriggerEvents.OnMedalShot -= OnMedalShot;
        ItemTriggerEvents.OnMedalLanded -= OnMedalLanded;
        ItemTriggerEvents.OnMedalLost -= OnMedalLost;
        ItemTriggerEvents.OnRoundStart -= OnRoundStart;
        ItemTriggerEvents.OnRoundEnd -= OnRoundEnd;
        ItemTriggerEvents.OnSlotRoll -= OnSlotRoll;
        ItemTriggerEvents.OnSlotWin -= OnSlotWin;
        ItemTriggerEvents.OnConsumptionItem -= OnConsumptionItem;
    }

    void OnInventoryChanged()
    {
        Debug.Log("oninventorychanged呼ばれた");
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                Debug.Log("実行まで行った");
                Debug.Log(item.effect);
                item.effect?.OnInventoryChanged(context, item.level);
            }
        }
    }

    void OnMedalShot()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnMedalShot(context, item.level);
            }
        }
    }

    void OnMedalLanded(GameObject medal)
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnMedalLanded(context, medal, item.level);
            }
        }
    }

    void OnMedalLost()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnMedalLost(context, item.level);
            }
        }
    }

    void OnRoundStart()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnRoundStart(context, item.level);
            }
        }
    }

    void OnRoundEnd()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnRoundEnd(context, item.level);
            }
        }
    }

    void OnSlotRoll()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnSlotRoll(context, item.level);
            }
        }
    }

    void OnSlotWin()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnSlotWin(context, item.level);
            }
        }
    }

    void OnConsumptionItem()
    {
        foreach (var item in statusGet.buffStats)
        {
            if (item != null)
            {
                item.effect?.OnConsumptionItem(context, item.level);
            }
        }
    }
}
