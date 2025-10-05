using MackySoft.XPool.Unity;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.ObjectPool
{
    public abstract class UniGameObjectPoolBase : IUniGameObjectPool
    {
        [SerializeField] protected GameObjectPool _gameObjectPool;
        GameObjectPool IUniGameObjectPool.GameObjectPool { get => _gameObjectPool; }
    }
}
