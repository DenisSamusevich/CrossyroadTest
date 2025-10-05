using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses
{
    using FirstBossCreator = AsyncEntityService<ILevelData, FirstBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>;
    using LevelCreator = AsyncEntityService<ITB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>;
    using PlayerCreator = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>;
    using SeondBossCreator = AsyncEntityService<ILevelData, SecondBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>;


    internal class TB_Game : GameBase
    {
        public const string KeyGame = "TB_Game";

        internal struct PlayerWinMessage { }

        private readonly IStatusGameChecker _statusGameChecker;
        private readonly IWorldCreator _worldCreator;
        private readonly LevelCreator _levelCreator;
        private readonly PlayerCreator _playerCreator;
        private readonly FirstBossCreator _firstBossCreator;
        private readonly SeondBossCreator _secondBossCreator;
        private readonly ICameraService _cameraService;
        private readonly ITB_LevelSettings _levelSettings;
        private readonly IEnumerable<ITR_GameService> _gameServices;
        private readonly TB_GameSession _gameSession;
        private IDisposable _disposed;

        public TB_Game([Key(KeyGame)] IStatusGameChecker statusGameChecker,
            IWorldCreator worldCreator,
            LevelCreator levelCreator,
            PlayerCreator playerCreator,
            FirstBossCreator firstBossCreator,
            SeondBossCreator secondBossCreator,
            ICameraService cameraService,
            IPublisher<PlayerWinMessage> playerWinPublisher,
            ITB_LevelSettings levelSettings,
            IEnumerable<ITR_GameService> gameServices,
            TB_GameSession gameSession)
        {
            _statusGameChecker = statusGameChecker;
            _worldCreator = worldCreator;
            _levelCreator = levelCreator;
            _playerCreator = playerCreator;
            _firstBossCreator = firstBossCreator;
            _secondBossCreator = secondBossCreator;
            _cameraService = cameraService;
            _levelSettings = levelSettings;
            _gameServices = gameServices;
            _gameSession = gameSession;
        }

        public override async UniTask PrepareGameAsync(CancellationToken cancellationToken)
        {
            _worldCreator.CreateWorld(_levelSettings);
            _cameraService.SetFocus(CameraTargetType.TwoBossLevel);
            _gameSession.LevelModel = await _levelCreator.CreateEntityAsync(_levelSettings, cancellationToken);
            _gameSession.PlayerModel = await _playerCreator.CreateEntityAsync(_gameSession.LevelModel.EntityData, cancellationToken);
            _gameSession.FirstBossModel = await _firstBossCreator.CreateEntityAsync(_gameSession.LevelModel.EntityData, cancellationToken);
            _gameSession.SecondBossModel = await _secondBossCreator.CreateEntityAsync(_gameSession.LevelModel.EntityData, cancellationToken);
        }

        public override void Dispose()
        {
            _disposed?.Dispose();
            _levelCreator.Dispose();
            _playerCreator.Dispose();
            _firstBossCreator.Dispose();
            _secondBossCreator.Dispose();
            _gameSession.LevelModel = null;
            _gameSession.PlayerModel = null;
            _gameSession.FirstBossModel = null;
            _gameSession.SecondBossModel = null;
        }

        public override async UniTask<GameResult> GameAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _gameServices)
            {
                service.EnableService();
            }
            _cameraService.SetFocus(CameraTargetType.TwoBossPlayer);
            while (_statusGameChecker.HasResult == false)
            {
                _gameSession.FirstBossModel.EntityBehavior.Update();
                _gameSession.SecondBossModel.EntityBehavior.Update();
                await UniTask.Yield();
            }
            await UniTask.WaitWhile(() => _gameSession.PlayerModel.EntityData.State != PlayerState.None);
            foreach (var service in _gameServices)
            {
                service.DisableService();
            }
            return _statusGameChecker.GameResult;
        }
    }
}
