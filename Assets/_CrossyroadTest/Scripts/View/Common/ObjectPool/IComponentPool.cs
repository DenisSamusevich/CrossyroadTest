using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.ObjectPool
{
    public interface IComponentPool<T> where T : Component
    {
        ComponentPool<T> ComponentPool { get; }
    }
}
