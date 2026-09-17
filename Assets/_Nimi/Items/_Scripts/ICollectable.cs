using System;

namespace EMR.Medal
{
    public interface ICollectable
    {
        public CollectableData Info { get; }

        public int Count { get; }

        public event Action OnCollect;

        public void Collect();
    }
}