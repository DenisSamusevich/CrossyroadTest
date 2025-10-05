using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using MessagePipe;
using System;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads
{
    internal class TR_StatusGameChecker : IStatusGameChecker, ITR_GameService
    {
        public struct PlayerWinMessage { }

        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly IPublisher<PlayerWinMessage> _playerWinPublisher;
        private IDisposable _disposable;

        public TR_StatusGameChecker(ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber)
        {
            _collisionMessageSubscriber = collisionMessageSubscriber;
        }

        public void EnableService()
        {
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(CheckCollision).AddTo(bag);
            _disposable = bag.Build();
            GameResult = GameResult.None;
            HasResult = false;
        }

        public void DisableService()
        {
            _disposable?.Dispose();

        }

        public bool HasResult { get; private set; }
        public GameResult GameResult { get; private set; }

        private void CheckCollision(CollisionPlayerMessage collisionMessage)
        {
            if (collisionMessage.TypeCollision == CollisionType.PlayerObstacle)
            {
                GameResult = GameResult.Fail;
                HasResult = true;
            }
            if (GameResult == GameResult.None && collisionMessage.TypeCollision == CollisionType.PlayerFinalArea)
            {
                GameResult = GameResult.PlayerWin;
                HasResult = true;
                _playerWinPublisher.Publish(new PlayerWinMessage());
            }
        }
    }
}
