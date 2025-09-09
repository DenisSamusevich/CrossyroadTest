using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Input;
using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Assets._CrossyroadTest.Scripts.Common.Input.InputService;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player
{
    internal class Player : PlayerBase<Player>, IPlayerData, IEntityInit<IEnumerable<AreaType>>
    {
        private readonly ISubscriber<InputCommand<InputMovingType>> _inputMovingSubscriber;
        private CancellationTokenSource _cancellationTokenSource;
        private IDisposable _disposable;

        public Player(ISubscriber<InputCommand<InputMovingType>> inputMovingSubscriber,
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            IAsyncPublisher<MovePlayerCommand<Player>> movePlayerPublisher,
            IAsyncPublisher<DancePlayerCommand<Player>> dancePlayerPublisher,
            IAsyncPublisher<PushPlayerCommand<Player>> pushPlayerPublisher,
            ISubscriber<PlayerWinMessage<Player>> playerWinSubscriber) :
            base(collisionMessageSubscriber, movePlayerPublisher, dancePlayerPublisher, pushPlayerPublisher, playerWinSubscriber)
        {
            _inputMovingSubscriber = inputMovingSubscriber;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Vector2 Position { get; private set; }
        public PlayerState State { get; private set; }

        public UniTask InitAsync(IEnumerable<AreaType> levelSettings, CancellationToken cancellationToken)
        {
            var startPosition = -1;
            foreach (var area in levelSettings)
            {
                startPosition++;
                if (area == AreaType.Start) break;
            }
            Position = new Vector2(UnityEngine.Random.Range(0, 10), startPosition);
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(OnPlayerCollision).AddTo(bag);
            _inputMovingSubscriber.Subscribe(OnPlayerMoving).AddTo(bag);
            _playerWinSubscriber.Subscribe(OnWin).AddTo(bag);
            _disposable = bag.Build();
            State = PlayerState.WaitInput;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void OnPlayerMoving(InputCommand<InputMovingType> inputCommand)
        {
            if (State == PlayerState.WaitInput && inputCommand.Input != InputMovingType.None)
            {
                MovePlayer(inputCommand.Input).Forget();
            }

            async UniTask MovePlayer(InputMovingType inputType)
            {
                State = PlayerState.Moving;
                Position += inputType switch
                {
                    InputMovingType.Forward => Vector2.up,
                    InputMovingType.Back => Vector2.down,
                    InputMovingType.Left => Vector2.left,
                    InputMovingType.Right => Vector2.right,
                    _ => Vector2.zero
                };
                var isCancelMove = await _movePlayerPublisher.PublishAsync(new MovePlayerCommand<Player>(), _cancellationTokenSource.Token).SuppressCancellationThrow();
                if (isCancelMove) return;
                State = PlayerState.WaitInput;
            }
        }

        private void OnWin(PlayerWinMessage<Player> message)
        {
            WinDance().Forget();

            async UniTask WinDance()
            {
                await UniTask.WaitWhile(() => State != PlayerState.WaitInput);
                State = PlayerState.Win;
                await _dancePlayerPublisher.PublishAsync(new DancePlayerCommand<Player>());
                State = PlayerState.None;
            }
        }
        private void OnPlayerCollision(CollisionPlayerMessage collisionPlayerMessage)
        {
            if (collisionPlayerMessage.TypeCollision == TypeCollision.PlayerObstacle)
            {
                CollisionForFail().Forget();
            }

            async UniTask CollisionForFail()
            {
                _cancellationTokenSource.Cancel();
                State = PlayerState.Collision;
                await _pushPlayerPublisher.PublishAsync(new PushPlayerCommand<Player>());
                State = PlayerState.None;
            }
        }
    }
}
