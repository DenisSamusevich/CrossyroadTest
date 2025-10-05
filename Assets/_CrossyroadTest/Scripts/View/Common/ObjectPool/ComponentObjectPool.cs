using System;

namespace Assets._CrossyroadTest.Scripts.View.Common.ObjectPool
{
    [Serializable]
    public class ComponentObjectPool<T> : ObjectPoolBase<T> where T : UnityEngine.Component
    {
        public void SetOnCreateAction(Action<T> action) => _componentPool.OnCreate = action;
    }
}
