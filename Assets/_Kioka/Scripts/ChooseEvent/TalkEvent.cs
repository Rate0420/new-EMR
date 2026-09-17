using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Game/Event",
    fileName = "Event"
)]
public class TalkEvent : ScriptableObject
{
    [Header("保存するデータ")] public List<EventData> events;
    [Header("選択肢イベント")] public int[] number;

    [System.Serializable]
    public class EventData
    {
        [Header("選択肢(2～3個)")] public ChooseData[] choices;
    }

    [System.Serializable]
    public class ChooseData
    {
        [Header("選択肢")] public string routeString;
        [Header("好感度")] public string chooseString;
    }
}
