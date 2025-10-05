using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces
{
    public interface IWorldObjectService
    {
        public GameObject CurrentPlayer { get; }
        public IWorldData CurrentWorld { get; }
        TPlayerComponent GetPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> component, Vector3 position, Quaternion quaternion) where TPlayerComponent : MonoBehaviour;
        TPlayerComponent GetPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> component, Vector3 position, Quaternion quaternion, Transform transform) where TPlayerComponent : MonoBehaviour;
        void ReturnPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> componentPool, TPlayerComponent component) where TPlayerComponent : MonoBehaviour;
        TComponent GetComponent<TComponent>(IComponentPool<TComponent> componentPool, Vector3 position, Quaternion quaternion) where TComponent : Component;
        TComponent GetComponent<TComponent>(IComponentPool<TComponent> componentPool, Vector3 position, Quaternion quaternion, Transform transform) where TComponent : Component;
        GameObject GetGameObject(IUniGameObjectPool uniGameObjectPool, Vector3 position, Quaternion quaternion);
        GameObject GetGameObject(IUniGameObjectPool uniGameObjectPool, Vector3 position, Quaternion quaternion, Transform transform);
        void ReturnGameObject(IUniGameObjectPool uniGameObjectPool, GameObject gameObject);
        void ReturnComponent<TComponent>(IComponentPool<TComponent> componentPool, TComponent component) where TComponent : Component;
    }
}
