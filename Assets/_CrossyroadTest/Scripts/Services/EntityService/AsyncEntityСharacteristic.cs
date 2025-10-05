using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets._CrossyroadTest.Scripts.Services.EntityService
{
    public static class AsyncEntityCharacteristic<TInitialData, TEntityData, T_IEntityData>
        where TEntityData : class, T_IEntityData, IInitAsyncElement<TInitialData>
    {
        public abstract class AsyncEntity : AsyncEntityBase<TInitialData, T_IEntityData>, IEntityModel<T_IEntityData>
        {
            private readonly AsyncEntityFactory _entityFactory;
            protected TEntityData _entityData;
            public AsyncEntity(AsyncEntityFactory entityFactory)
            {
                _entityFactory = entityFactory;
            }

            public T_IEntityData EntityData { get => _entityData; }

            protected override async UniTask InitElementCore(TInitialData initialData)
            {
                _entityData = await _entityFactory.Create<TInitialData, TEntityData>(initialData);
            }
            protected override void DestroyEntity()
            {
                _entityData.Dispose();
                _entityData = null;
            }
        }
    }

    public static class AsyncEntityCharacteristic<TInitialData, TEntityData, T_IEntityData, TEntityBehavior, T_IEntityBehavior>
        where TEntityData : class, T_IEntityData, IInitAsyncElement<TInitialData>
        where TEntityBehavior : class, T_IEntityBehavior, IInitAsyncElement<TInitialData, TEntityData>
    {
        public abstract class AsyncEntity : AsyncEntityBase<TInitialData, T_IEntityData>, IEntityModel<T_IEntityData, T_IEntityBehavior>
        {
            private readonly AsyncEntityFactory _entityFactory;
            protected TEntityData _entityData;
            protected TEntityBehavior _entityBehavior;

            public AsyncEntity(AsyncEntityFactory entityFactory)
            {
                _entityFactory = entityFactory;
            }

            public T_IEntityData EntityData => _entityData;
            public T_IEntityBehavior EntityBehavior => _entityBehavior;

            protected override async UniTask InitElementCore(TInitialData initialData)
            {
                _entityData = await _entityFactory.Create<TInitialData, TEntityData>(initialData);
                _entityBehavior = await _entityFactory.Create<TInitialData, TEntityData, TEntityBehavior>(initialData, _entityData);
            }

            protected override void DestroyEntity()
            {
                _entityData.Dispose();
                _entityBehavior.Dispose();
                _entityData = null;
                _entityBehavior = null;
            }
        }
    }
}
