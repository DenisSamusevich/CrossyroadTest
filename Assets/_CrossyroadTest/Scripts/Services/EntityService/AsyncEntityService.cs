using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.Services.EntityService
{
    public class AsyncEntityService<TInitialData, TEntity, T_IEntityData, T_IReadOnlyModel>
        where TEntity : AsyncEntityBase<TInitialData, T_IEntityData>, T_IReadOnlyModel
    {
        public struct CreateEntityCommand
        {
            public CreateEntityCommand(T_IReadOnlyModel entity)
            {
                EntityModel = entity;
            }
            public T_IReadOnlyModel EntityModel { get; }
        }
        public struct DisposeEntityCommand { }

        private readonly AsyncEntityFactory _entityFactory;
        private readonly IAsyncPublisher<CreateEntityCommand> _createEntityPublisher;
        private readonly IPublisher<DisposeEntityCommand> _disposeEntityPublisher;
        private readonly List<IDisposable> _listDisposable;

        public AsyncEntityService(AsyncEntityFactory entityFactory,
            IAsyncPublisher<CreateEntityCommand> createEntityPublisher,
            IPublisher<DisposeEntityCommand> disposeEntityPublisher)
        {
            _entityFactory = entityFactory;
            _createEntityPublisher = createEntityPublisher;
            _disposeEntityPublisher = disposeEntityPublisher;
            _listDisposable = new List<IDisposable>();
        }

        public async UniTask<T_IReadOnlyModel> CreateEntityAsync(TInitialData initialData, CancellationToken cancellationToken = default)
        {
            var entity = await _entityFactory.Create<TEntity>();
            await entity.InitAsyncElement(initialData);
            await entity.PrePublishAsync(cancellationToken);
            await _createEntityPublisher.PublishAsync(new CreateEntityCommand(entity), cancellationToken);
            await entity.PostPublishAsync(cancellationToken);
            _listDisposable.Add(entity);
            return entity;
        }

        public void Dispose()
        {
            _disposeEntityPublisher.Publish(new DisposeEntityCommand());
            foreach (var entity in _listDisposable)
            {
                entity.Dispose();
            }
            _listDisposable.Clear();
        }
    }
}
