using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.ObjectPool
{
    public abstract class ObjectPoolBase<T> : IComponentPool<T> where T : Component
    {
        [SerializeField] protected ComponentPool<T> _componentPool;
        ComponentPool<T> IComponentPool<T>.ComponentPool { get => _componentPool; }
    }
}
