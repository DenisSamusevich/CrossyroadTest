using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Data;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services
{
    public class WorldObjectService : IWorldObjectService
    {
        private readonly IObjectResolver _objectResolver;

        public WorldObjectService(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public IWorldData CurrentWorld { get; private set; }
        public GameObject CurrentPlayer { get; private set; }

        public void CreateNewWorld(Vector3 zeroPosition, bool isDestroyAfterGame, bool isUsedRepeatedly)
        {
            var world = _objectResolver.Resolve<WorldData>();
            world.ZeroPosition = zeroPosition;
            world.IsDestroyAfterGame = isDestroyAfterGame;
            world.IsUsedRepeatedly = isUsedRepeatedly;
            CurrentWorld = world;
        }

        public void SetCurrentWorld(IWorldData worldData)
        {
            CurrentWorld = worldData;
        }

        public void DestroyCurrentWorld()
        {
            if (CurrentPlayer != null)
            {
                GameObject.Destroy(CurrentPlayer);
            }
            CurrentWorld = null;
            CurrentPlayer = null;
        }

        TPlayerComponent IWorldObjectService.GetPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> componentPool, Vector3 position, Quaternion quaternion)
        {
            if (CurrentPlayer == null)
            {
                var playerComponent = componentPool.ComponentPool.Rent(position, quaternion);
                CurrentPlayer = playerComponent.gameObject;
                return playerComponent;
            }
            else
            {
                if (CurrentPlayer.TryGetComponent<TPlayerComponent>(out TPlayerComponent playerComponent))
                {
                    return playerComponent;
                }
                throw new System.Exception("PlayerComponent not find");
            }
        }

        TPlayerComponent IWorldObjectService.GetPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> componentPool, Vector3 position, Quaternion quaternion, Transform transform)
        {
            if (CurrentPlayer == null)
            {
                var playerComponent = componentPool.ComponentPool.Rent(position, quaternion, transform);
                CurrentPlayer = playerComponent.gameObject;
                return playerComponent;
            }
            else
            {
                if (CurrentPlayer.TryGetComponent<TPlayerComponent>(out TPlayerComponent playerComponent))
                {
                    return playerComponent;
                }
                throw new System.Exception("PlayerComponent not find");
            }
        }
        void IWorldObjectService.ReturnPlayerComponent<TPlayerComponent>(IComponentPool<TPlayerComponent> componentPool, TPlayerComponent component)
        {
            CurrentPlayer = null;
            componentPool.ComponentPool.Return(component);
        }
        TComponent IWorldObjectService.GetComponent<TComponent>(IComponentPool<TComponent> componentPool, Vector3 position, Quaternion quaternion)
        {
            var component = componentPool.ComponentPool.Rent(position, quaternion);
            return component;
        }
        public TComponent GetComponent<TComponent>(IComponentPool<TComponent> componentPool, Vector3 position, Quaternion quaternion, Transform transform) where TComponent : Component
        {
            var component = componentPool.ComponentPool.Rent(position, quaternion, transform);
            return component;
        }
        GameObject IWorldObjectService.GetGameObject(IUniGameObjectPool uniGameObjectPool, Vector3 position, Quaternion quaternion)
        {
            var gameObject = uniGameObjectPool.GameObjectPool.Rent(position, quaternion);
            return gameObject;
        }

        GameObject IWorldObjectService.GetGameObject(IUniGameObjectPool uniGameObjectPool, Vector3 position, Quaternion quaternion, Transform transform)
        {
            var gameObject = uniGameObjectPool.GameObjectPool.Rent(position, Quaternion.identity, transform);
            return gameObject;
        }

        void IWorldObjectService.ReturnGameObject(IUniGameObjectPool uniGameObjectPool, GameObject gameObject)
        {
            uniGameObjectPool.GameObjectPool.Return(gameObject);
        }

        void IWorldObjectService.ReturnComponent<TComponent>(IComponentPool<TComponent> componentPool, TComponent component)
        {
            componentPool.ComponentPool.Return(component);
        }
    }
}
