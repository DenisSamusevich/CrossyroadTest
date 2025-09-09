using Assets._CrossyroadTest.GameServices.Base;
using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Input;
using Assets._CrossyroadTest.Scripts.Common.Interface;
using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Player;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses
{
    using PB = PlayerBase<Player>;
    internal class BattleWithTwoBosses : GameBase
    {
        private readonly Vector2Int _levelSettings = new Vector2Int(12, 11);

        private readonly EntityCreator<Vector2Int, ILevelData> _levelCreator;
        private readonly EntityCreator<ILevelData, IPlayerData> _playerCreator;
        private readonly EntityCreator<ILevelData, IFirstBossData> _firstBossCreator;
        private readonly EntityCreator<ILevelData, ISecondBossData> _secondBossCreator;
        private readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        private readonly IPublisher<PB.PlayerWinMessage<Player>> _playerWinPublisher;
        private readonly InputService _inputService;
        private readonly ICameraService _cameraService;
        private ILevelData _levelData;
        private IPlayerData _playerData;
        private IFirstBossData _firstBossData;
        private ISecondBossData _secondBossData;
        private GameStatus _gameStatus;
        private IDisposable _disposed;
        private List<IUpdate> _entityForUpdate;

        public BattleWithTwoBosses(
            EntityCreator<Vector2Int, ILevelData> levelCreator,
            EntityCreator<ILevelData, IPlayerData> playerCreator,
            EntityCreator<ILevelData, IFirstBossData> firstBossCreator,
            EntityCreator<ILevelData, ISecondBossData> secondBossCreator,
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            InputService inputService,
            ICameraService cameraService,
            IPublisher<PB.PlayerWinMessage<Player>> playerWinPublisher)
        {
            _levelCreator = levelCreator;
            _playerCreator = playerCreator;
            _firstBossCreator = firstBossCreator;
            _secondBossCreator = secondBossCreator;
            _collisionMessageSubscriber = collisionMessageSubscriber;
            _playerWinPublisher = playerWinPublisher;
            _cameraService = cameraService;
            _inputService = inputService;
            _entityForUpdate = new List<IUpdate>();
        }

        public override async UniTask PrepareGameAsync(CancellationToken cancellationToken)
        {
            _cameraService.SetFocus(CameraTargetType.TwoBossLevel);
            _levelData = await _levelCreator.CreateEntityAsync<Level>(_levelSettings, cancellationToken);
            _playerData = await _playerCreator.CreateEntityAsync<Player>(_levelData, cancellationToken);
            _firstBossData = await _firstBossCreator.CreateEntityAsync<FirstBoss>(_levelData, cancellationToken);
            _secondBossData = await _secondBossCreator.CreateEntityAsync<SecondBoss>(_levelData, cancellationToken);
            _disposed = _collisionMessageSubscriber.Subscribe(CheckCollision);
            if (_levelData is IUpdate level) _entityForUpdate.Add(level);
            if (_firstBossData is IUpdate firstBoss) _entityForUpdate.Add(firstBoss);
            if (_secondBossData is IUpdate secondBoss) _entityForUpdate.Add(secondBoss);
        }

        public override void Dispose()
        {
            _gameStatus = GameStatus.None;
            _entityForUpdate.Clear();
            _levelCreator.Dispose();
            _playerCreator.Dispose();
            _firstBossCreator.Dispose();
            _secondBossCreator.Dispose();
            _levelData = null;
            _playerData = null;
            _disposed?.Dispose();
        }

        public override async UniTask<GameStatus> GameAsync(CancellationToken cancellationToken)
        {
            _cameraService.SetFocus(CameraTargetType.TwoBossPlayer);
            _inputService.Enable();
            while (_gameStatus == GameStatus.None)
            {
                foreach (var entity in _entityForUpdate)
                {
                    entity.Update();
                }
                await UniTask.Yield();
                if (_firstBossData.IsDefeated && _secondBossData.IsDefeated)
                {
                    _gameStatus = GameStatus.PlayerWin;
                    _playerWinPublisher.Publish(new PB.PlayerWinMessage<Player>());
                }
            }
            _inputService.Disable();
            await UniTask.WaitWhile(() => _playerData.State != PlayerState.None);
            return _gameStatus;
        }

        private void CheckCollision(CollisionPlayerMessage collisionMessage)
        {
            if (collisionMessage.TypeCollision == TypeCollision.PlayerObstacle)
            {
                _gameStatus = GameStatus.Fail;
                _disposed.Dispose();
            }
        }
    }
}
