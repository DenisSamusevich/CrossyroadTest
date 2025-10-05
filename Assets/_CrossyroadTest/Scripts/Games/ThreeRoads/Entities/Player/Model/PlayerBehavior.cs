using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using static Assets._CrossyroadTest.Scripts.Games.ThreeRoads.TR_StatusGameChecker;
using static Assets._CrossyroadTest.Scripts.View.Common.Services.InputService;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model
{
    public class PlayerBehavior : IInitAsyncElement<ITR_LevelSettings, PlayerData>, IPlayerBehavior
    {
        public struct BehaviorCommand { }

        private readonly BehaviorCommand _command = default;
        private readonly IAsyncPublisher<PlayerCommand, BehaviorCommand> _asyncBehaviorPublisher;
        private readonly ISubscriber<InputCommand<DirectionType>> _inputMovingSubscriber;
        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly ISubscriber<PlayerWinMessage> _playerWinSubscriber;
        private IDisposable _disposable;
        private CancellationTokenSource _cancellationTokenSource;
        private PlayerData _playerData;

        internal PlayerBehavior(IAsyncPublisher<PlayerCommand, BehaviorCommand> asyncBehaviorPublisher,
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

        UniTask IInitAsyncElement<ITR_LevelSettings, PlayerData>.InitAsyncElement(ITR_LevelSettings data1, PlayerData data2)
        {
            _playerData = data2;
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
                MovePlayer(inputCommand.Input).Forget();
            }

            async UniTask MovePlayer(DirectionType inputType)
            {
                _playerData.State = PlayerState.Moving;
                _playerData.Position += inputType switch
                {
                    DirectionType.Forward => Vector2.up,
                    DirectionType.Back => Vector2.down,
                    DirectionType.Left => Vector2.left,
                    DirectionType.Right => Vector2.right,
                    _ => Vector2.zero
                };
                var isCancelMove = await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Move, _command, _cancellationTokenSource.Token).SuppressCancellationThrow();
                if (isCancelMove) return;
                _playerData.State = PlayerState.WaitInput;
            }
        }

        private void OnWin(PlayerWinMessage message)
        {
            WinDance().Forget();

            async UniTask WinDance()
            {
                await UniTask.WaitWhile(() => _playerData.State != PlayerState.WaitInput);
                _playerData.State = PlayerState.Win;
                await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Dance, _command);
                _playerData.State = PlayerState.None;
            }
        }

        private void OnPlayerCollision(CollisionPlayerMessage collisionPlayerMessage)
        {
            if (collisionPlayerMessage.TypeCollision == CollisionType.PlayerObstacle)
            {
                CollisionForFail().Forget();
            }

            async UniTask CollisionForFail()
            {
                _cancellationTokenSource.Cancel();
                _playerData.State = PlayerState.Collision;
                await _asyncBehaviorPublisher.PublishAsync(PlayerCommand.Push, _command);
                _playerData.State = PlayerState.None;
            }
        }
    }
}
