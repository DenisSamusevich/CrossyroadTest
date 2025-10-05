using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using static Assets._CrossyroadTest.Scripts.Games.TwoBosses.TB_StatusGameChecker;
using static Assets._CrossyroadTest.Scripts.View.Common.Services.InputService;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model
{
    public class PlayerBehavior : IInitAsyncElement<ILevelData, PlayerData>, IPlayerBehavior
    {
        public struct BehaviorCommand { }

        private readonly IAsyncPublisher<PlayerCommand, BehaviorCommand> _asyncBehaviorPublisher;
        private readonly ISubscriber<InputCommand<DirectionType>> _inputMovingSubscriber;
        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly ISubscriber<PlayerWinMessage> _playerWinSubscriber;
        private CancellationTokenSource _cancellationTokenSource;
        private PlayerData _playerData;
        private ILevelData _levelData;
        private IDisposable _disposable;

        public PlayerBehavior(IAsyncPublisher<PlayerCommand, BehaviorCommand> asyncBehaviorPublisher,
            ISubscriber<InputCommand<DirectionType>> inputMovingSubscriber,
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            ISubscriber<PlayerWinMessage> playerWinSubscriber)
        {
            _asyncBehaviorPublisher = asyncBehaviorPublisher;
            _inputMovingSubscriber = inputMovingSubscriber;
            _collisionMessageSubscriber = collisionMessageSubscriber;
            _playerWinSubscriber = playerWinSubscriber;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        UniTask IInitAsyncElement<ILevelData, PlayerData>.InitAsyncElement(ILevelData levelData, PlayerData playerData)
        {
            _levelData = levelData;
            _playerData = playerData;
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(OnPlayerCollision).AddTo(bag);
            _inputMovingSubscriber.Subscribe(OnPlayerMoving).AddTo(bag);
            _playerWinSubscriber.Subscribe(OnWin).AddTo(bag);
            _disposable = bag.Build();
            return UniTask.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            _disposable?.Dispose();
        }

        private void OnPlayerMoving(InputCommand<DirectionType> inputCommand)
        {
            if (_playerData.State == PlayerState.WaitInput && inputCommand.Input != DirectionType.None)
            {
                var direction = inputCommand.Input switch
                {
                    DirectionType.Forward => Vector2Int.up,
                    DirectionType.Back => Vector2Int.down,
                    DirectionType.Right => Vector2Int.right,
                    DirectionType.Left => Vector2Int.left,
                    _ => Vector2Int.zero,
                };
                var nextPosition = _playerData.Position + direction;
                if (nextPosition.x < _levelData.Width && nextPosition.x >= 0
                    && nextPosition.y < _levelData.Length && nextPosition.y >= 0)
                {
                    MovePlayer(nextPosition).Forget();
                }
            }

            async UniTask MovePlayer(Vector2Int position)
            {
                _playerData.Position = position;
                _playerData.State = PlayerState.Moving;
                var isCancelMove = await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Move, new BehaviorCommand(), _cancellationTokenSource.Token).SuppressCancellationThrow();
                if (isCancelMove) return;
                _playerData.State = PlayerState.WaitInput;
            }
        }

        private void OnWin(PlayerWinMessage playerWin)
        {
            WinDance().Forget();

            async UniTask WinDance()
            {
                _playerData.State = PlayerState.Win;
                await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Dance, new BehaviorCommand(), CancellationToken.None);
                _playerData.State = PlayerState.None;
            }
        }

        protected void OnPlayerCollision(CollisionPlayerMessage collisionPlayerMessage)
        {
            if (collisionPlayerMessage.TypeCollision == CollisionType.PlayerObstacle)
            {
                CollisionForFail().Forget();
            }

            async UniTask CollisionForFail()
            {
                _cancellationTokenSource.Cancel();
                _playerData.State = PlayerState.Collision;
                await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Push, new BehaviorCommand(), CancellationToken.None);
                _playerData.State = PlayerState.None;
            }
        }
    }
}
