using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Services.EntityService
{
    public class AsyncEntityFactory
    {
        private readonly IObjectResolver _objectResolver;
        public AsyncEntityFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public UniTask<TEntity> Create<TEntity>()
        {
            var entityData = _objectResolver.Resolve<TEntity>();
            return UniTask.FromResult(entityData);
        }
        public async UniTask<TEntity> Create<T1, TEntity>(T1 data1) where TEntity : IInitAsyncElement<T1>
        {
            var entityData = _objectResolver.Resolve<TEntity>();
            await entityData.InitAsyncElement(data1);
            return entityData;
        }

        public async UniTask<TEntity> Create<T1, T2, TEntity>(T1 data1, T2 data2) where TEntity : IInitAsyncElement<T1, T2>
        {
            var entityData = _objectResolver.Resolve<TEntity>();
            await entityData.InitAsyncElement(data1, data2);
            return entityData;
        }
    }
}
