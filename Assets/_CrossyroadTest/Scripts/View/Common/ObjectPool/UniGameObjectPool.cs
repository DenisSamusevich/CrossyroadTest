using System;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.ObjectPool
{
    [Serializable]
    public class UniGameObjectPool : UniGameObjectPoolBase
    {
        public void SetOnCreateAction(Action<GameObject> action) => _gameObjectPool.OnCreate = action;
    }
}
