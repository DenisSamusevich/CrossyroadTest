using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.Services.EntityService
{
    public abstract class AsyncEntityBase<TInitialData, IEntityData> : IInitAsyncElement<TInitialData>
    {
        public UniTask PrePublishAsync(CancellationToken cancellationToken) => PrePublishCore(cancellationToken);
        public UniTask PostPublishAsync(CancellationToken cancellationToken) => PostPublishCore(cancellationToken);
        public UniTask InitAsyncElement(TInitialData initialData) => InitElementCore(initialData);
        void IDisposable.Dispose() => DestroyEntity();
        protected virtual UniTask InitElementCore(TInitialData initialData) { return UniTask.CompletedTask; }
        protected virtual UniTask PrePublishCore(CancellationToken cancellationToken) { return UniTask.CompletedTask; }
        protected virtual UniTask PostPublishCore(CancellationToken cancellationToken) { return UniTask.CompletedTask; }
        protected abstract void DestroyEntity();
    }

}
