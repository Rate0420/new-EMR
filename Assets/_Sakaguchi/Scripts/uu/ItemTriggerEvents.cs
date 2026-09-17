using UnityEngine;

public static class ItemTriggerEvents
{
    public static System.Action OnInventoryChanged;

    public static System.Action OnMedalShot;
    public static System.Action<GameObject> OnMedalLanded;
    public static System.Action OnMedalLost;

    public static System.Action OnRoundStart;
    public static System.Action OnRoundEnd;

    public static System.Action OnSlotRoll;
    public static System.Action OnSlotWin;

    public static System.Action OnConsumptionItem;
}