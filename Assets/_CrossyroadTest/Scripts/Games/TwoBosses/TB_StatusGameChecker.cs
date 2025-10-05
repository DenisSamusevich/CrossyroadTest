using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses
{
    public class TB_StatusGameChecker : IStatusGameChecker, ITB_GameService
    {
        public struct PlayerWinMessage { }

        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly IPublisher<PlayerWinMessage> _playerWinPublisher;
        private readonly ITB_GameSession _gameSession;
        private CancellationTokenSource _cancellationTokenSource;
        private IDisposable _disposable;

        public TB_StatusGameChecker(
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            IPublisher<PlayerWinMessage> playerWinPublisher,
            ITB_GameSession gameSession)
        {
            _collisionMessageSubscriber = collisionMessageSubscriber;
            _playerWinPublisher = playerWinPublisher;
            _gameSession = gameSession;
        }

        public bool HasResult { get; private set; }
        public GameResult GameResult { get; private set; }

        public void EnableService()
        {
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(CheckCollision).AddTo(bag);
            _disposable = bag.Build();
            GameResult = GameResult.None;
            HasResult = false;
            _cancellationTokenSource = new CancellationTokenSource();
            CheckBossesDefeated(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask CheckBossesDefeated(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                if (_gameSession.FirstBossModel.EntityData.IsDefeated && _gameSession.SecondBossModel.EntityData.IsDefeated)
                {
                    GameResult = GameResult.PlayerWin;
                    HasResult = true;
                    _playerWinPublisher.Publish(new PlayerWinMessage());
                }
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token).SuppressCancellationThrow();
            }
        }

        public void DisableService()
        {
            _cancellationTokenSource?.Cancel();
            _disposable?.Dispose();
        }

        private void CheckCollision(CollisionPlayerMessage collisionMessage)
        {
            if (collisionMessage.TypeCollision == CollisionType.PlayerObstacle)
            {
                GameResult = GameResult.Fail;
                HasResult = true;
            }
        }
    }
}
