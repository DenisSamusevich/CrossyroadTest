using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.Services.GameService.Base
{
    public abstract class GameBase : IDisposable
    {
        public abstract UniTask PrepareGameAsync(CancellationToken cancellationToken);

        public abstract UniTask<GameResult> GameAsync(CancellationToken cancellationToken);

        public abstract void Dispose();
    }
}
