using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Threading;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Common.Creators
{
    internal interface IEntityInit<TInitialData>
    {
        UniTask InitAsync(TInitialData initialData, CancellationToken cancellationToken);

        UniTask PostInitAsync(CancellationToken cancellationToken) { return UniTask.CompletedTask; }
    }

    internal class EntityCreator<TInitialData, TData>
    {
        internal struct CreateEntityCommand<T> where T : TData
        {
            public CreateEntityCommand(T entity)
            {
                EntityData = entity;
            }
            public T EntityData { get; }
        }
        internal struct DisposeEntityCommand<T> where T : TData { }

        private readonly IAsyncPublisher<CreateEntityCommand<TData>> _createEntityPublisher;
        private readonly IPublisher<DisposeEntityCommand<TData>> _disposeEntityPublisher;
        private readonly IObjectResolver _objectResolver;

        public EntityCreator(IObjectResolver objectResolver,
            IAsyncPublisher<CreateEntityCommand<TData>> createEntityPublisher,
            IPublisher<DisposeEntityCommand<TData>> disposeEntityPublisher)
        {
            _objectResolver = objectResolver;
            _createEntityPublisher = createEntityPublisher;
            _disposeEntityPublisher = disposeEntityPublisher;
        }

        internal async UniTask<TData> CreateEntityAsync<TEntityInit>(TInitialData initialData, CancellationToken cancellationToken = default) where TEntityInit : IEntityInit<TInitialData>, TData
        {
            var entity = _objectResolver.Resolve<TEntityInit>();
            await entity.InitAsync(initialData, cancellationToken);
            await _createEntityPublisher.PublishAsync(new CreateEntityCommand<TData>(entity), cancellationToken);
            await entity.PostInitAsync(cancellationToken);
            return entity;
        }

        public void Dispose()
        {
            _disposeEntityPublisher.Publish(new DisposeEntityCommand<TData>());
        }
    }
}
