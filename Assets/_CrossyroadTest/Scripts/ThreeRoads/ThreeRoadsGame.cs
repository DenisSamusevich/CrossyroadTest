using Assets._CrossyroadTest.GameServices.Base;
using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Input;
using Assets._CrossyroadTest.Scripts.Common.Interface;
using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads
{
    using PB = PlayerBase<Player>;

    internal class ThreeRoadsGame : GameBase
    {
        private readonly IReadOnlyList<AreaType> levelSettings = new List<AreaType>()  { AreaType.Start, AreaType.Free, AreaType.Road, AreaType.Free,
            AreaType.Road, AreaType.Free, AreaType.Free, AreaType.Road, AreaType.Final, };

        private readonly EntityCreator<IEnumerable<AreaType>, ILevelData> _levelCreator;
        private readonly EntityCreator<IEnumerable<AreaType>, IPlayerData> _playerCreator;
        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly IPublisher<PB.PlayerWinMessage<Player>> _playerWinPublisher;
        private readonly InputService _inputService;
        private readonly ICameraService _cameraService;
        private ILevelData _levelData;
        private IPlayerData _playerData;
        private GameStatus _gameStatus;
        private IDisposable _disposed;
        private List<IUpdate> _entityForUpdate;

        public ThreeRoadsGame(
            EntityCreator<IEnumerable<AreaType>, ILevelData> levelCreator,
            EntityCreator<IEnumerable<AreaType>, IPlayerData> playerCreator,
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            InputService inputService,
            ICameraService cameraService,
            IPublisher<PB.PlayerWinMessage<Player>> playerWinPublisher)
        {
            _levelCreator = levelCreator;
            _playerCreator = playerCreator;
            _collisionMessageSubscriber = collisionMessageSubscriber;
            _inputService = inputService;
            _cameraService = cameraService;
            _playerWinPublisher = playerWinPublisher;
            _entityForUpdate = new List<IUpdate>();
        }

        public override async UniTask PrepareGameAsync(CancellationToken cancellationToken)
        {
            _cameraService.SetFocus(CameraTargetType.ThreeRoadsLevel);
            _levelData = await _levelCreator.CreateEntityAsync<Level>(levelSettings, cancellationToken);
            _playerData = await _playerCreator.CreateEntityAsync<Player>(levelSettings, cancellationToken);
            var bag = DisposableBag.CreateBuilder();
            _collisionMessageSubscriber.Subscribe(CheckCollision).AddTo(bag);
            if (_levelData is IUpdate level) _entityForUpdate.Add(level);
            if (_playerData is IUpdate player) _entityForUpdate.Add(player);
            if (_levelData is IDisposable levelDisposable) levelDisposable.AddTo(bag);
            if (_playerData is IDisposable playerDisposable) playerDisposable.AddTo(bag);
            _disposed = bag.Build();
        }

        public override void Dispose()
        {
            _gameStatus = GameStatus.None;
            _entityForUpdate.Clear();
            _levelCreator.Dispose();
            _playerCreator.Dispose();
            _disposed.Dispose();
            _levelData = null;
            _playerData = null;
        }

        public override async UniTask<GameStatus> GameAsync(CancellationToken cancellationToken)
        {
            _inputService.Enable();
            _cameraService.SetFocus(CameraTargetType.ThreeRoadsPlayer);
            while (_gameStatus == GameStatus.None || _playerData.State != PlayerState.None)
            {
                foreach (var entity in _entityForUpdate)
                {
                    entity.Update();
                }
                await UniTask.Yield();
            }
            _inputService.Disable();
            return _gameStatus;
        }

        private void CheckCollision(CollisionPlayerMessage collisionMessage)
        {
            if (collisionMessage.TypeCollision == TypeCollision.PlayerObstacle)
            {
                _gameStatus = GameStatus.Fail;
            }
            if (_gameStatus == GameStatus.None && collisionMessage.TypeCollision == TypeCollision.PlayerFinalArea)
            {
                _gameStatus = GameStatus.PlayerWin;
                _playerWinPublisher.Publish(new PB.PlayerWinMessage<Player>());
            }
        }
    }
}
