using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Input;
using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using static Assets._CrossyroadTest.Scripts.Common.Input.InputService;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Player
{
    internal class Player : PlayerBase<Player>, IPlayerData, IEntityInit<ILevelData>
    {
        private readonly ISubscriber<InputCommand<InputMovingType>> _inputMovingSubscriber;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private ILevelData _level;
        private IDisposable _disposable;

        public Player(ISubscriber<InputCommand<InputMovingType>> inputMovingSubscriber,
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            IAsyncPublisher<MovePlayerCommand<Player>> movePlayerPublisher,
            IAsyncPublisher<DancePlayerCommand<Player>> dancePlayerPublisher,
            IAsyncPublisher<PushPlayerCommand<Player>> pushPlayerPublisher,
            ISubscriber<PlayerWinMessage<Player>> playerWinSubscriber)
            : base(collisionMessageSubscriber, movePlayerPublisher, dancePlayerPublisher, pushPlayerPublisher, playerWinSubscriber)
        {
            _inputMovingSubscriber = inputMovingSubscriber;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Vector2Int Position { get; private set; }
        public PlayerState State { get; private set; }

        public async UniTask InitAsync(ILevelData levelData, CancellationToken cancellationToken)
        {
            _level = levelData;
            Position = new Vector2Int(_level.Width / 2, _level.Length / 2);
            await _movePlayerPublisher.PublishAsync(new MovePlayerCommand<Player>(), cancellationToken);
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(OnPlayerCollision).AddTo(bag);
            _inputMovingSubscriber.Subscribe(OnPlayerMoving).AddTo(bag);
            _playerWinSubscriber.Subscribe(OnWin).AddTo(bag);
            _disposable = bag.Build();
            State = PlayerState.WaitInput;
        }

        internal void Destroy()
        {
            _disposable?.Dispose();
        }

        private void OnPlayerMoving(InputCommand<InputMovingType> inputCommand)
        {
            if (State == PlayerState.WaitInput && inputCommand.Input != InputMovingType.None)
            {
                var direction = inputCommand.Input switch
                {
                    InputMovingType.Forward => Vector2Int.up,
                    InputMovingType.Back => Vector2Int.down,
                    InputMovingType.Right => Vector2Int.right,
                    InputMovingType.Left => Vector2Int.left,
                    _ => Vector2Int.zero,
                };
                var nextPosition = Position + direction;
                if (nextPosition.x < _level.Width && nextPosition.x >= 0
                    && nextPosition.y < _level.Length && nextPosition.y >= 0)
                {
                    MovePlayer(nextPosition).Forget();
                }
            }

            async UniTask MovePlayer(Vector2Int position)
            {
                Position = position;
                State = PlayerState.Moving;
                var isCancelMove = await _movePlayerPublisher.PublishAsync(new MovePlayerCommand<Player>(), _cancellationTokenSource.Token).SuppressCancellationThrow();
                if (isCancelMove) return;
                State = PlayerState.WaitInput;
            }
        }

        private void OnWin(PlayerWinMessage<Player> playerWin)
        {
            WinDance().Forget();

            async UniTask WinDance()
            {
                State = PlayerState.Win;
                await _dancePlayerPublisher.PublishAsync(new DancePlayerCommand<Player>(), CancellationToken.None);
                State = PlayerState.None;
            }
        }

        protected void OnPlayerCollision(CollisionPlayerMessage collisionPlayerMessage)
        {
            if (collisionPlayerMessage.TypeCollision == TypeCollision.PlayerObstacle)
            {
                CollisionForFail().Forget();
            }

            async UniTask CollisionForFail()
            {
                _cancellationTokenSource.Cancel();
                State = PlayerState.Collision;
                await _pushPlayerPublisher.PublishAsync(new PushPlayerCommand<Player>(), CancellationToken.None);
                State = PlayerState.None;
            }
        }
    }
}
