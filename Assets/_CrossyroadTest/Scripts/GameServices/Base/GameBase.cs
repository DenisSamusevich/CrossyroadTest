using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Assets._CrossyroadTest.GameServices.Base
{
    internal abstract class GameBase : IDisposable
    {
        public abstract UniTask PrepareGameAsync(CancellationToken cancellationToken);

        public abstract UniTask<GameStatus> GameAsync(CancellationToken cancellationToken);

        public abstract void Dispose();
    }
}
