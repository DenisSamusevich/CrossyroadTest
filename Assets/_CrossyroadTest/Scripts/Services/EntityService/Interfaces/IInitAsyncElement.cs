using Cysharp.Threading.Tasks;
using System;

namespace Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces
{
    public interface IInitAsyncElement<T1> : IDisposable
    {
        public UniTask InitAsyncElement(T1 data1);
        void IDisposable.Dispose() { }
    }

    public interface IInitAsyncElement<T1, T2> : IDisposable
    {
        public UniTask InitAsyncElement(T1 data1, T2 data2);
        void IDisposable.Dispose() { }
    }
}
