using System;
using UnityEngine;

namespace EMR.Medal
{
    public enum CollectableType
    {
        Medal,
        Ball,
        Item,
    }

    [Serializable]
    public class CollectableData
    {
        [SerializeField] CollectableType _type;
        [SerializeField] string _name;
        [SerializeField] int _count = 1;

        public CollectableType Type => _type;
        public string Name => _name;
        public int Count => _count;
    }
}
