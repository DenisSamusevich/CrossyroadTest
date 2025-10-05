using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler
{
    using LevelCreator = AsyncEntityService<IAB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>;
    using PlayerCreator = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>;
    using EnemyCreator = AsyncEntityService<(int indexEnemy, ILevelData levelData), Enemy, IEnemyData, IEntityModel<IEnemyData, IEnemyBehavior>>;
    internal class AB_Game : GameBase
    {
        public const string KeyGame = "AB_Game";

        private readonly IStatusGameChecker _statusGameChecker;
        private readonly IWorldCreator _worldCreator;
        private readonly LevelCreator _levelCreator;
        private readonly PlayerCreator _playerCreator;
        private readonly EnemyCreator _enemyCreator;
        private readonly ICameraService _cameraService;
        private readonly IAB_LevelSettings _levelSettings;
        private readonly IEnumerable<IAB_GameService> _gameServices;
        private readonly AB_GameSession _gameSession;
        private readonly IAB_Gameplay _gameplay;
        private IDisposable _disposed;

        public AB_Game([Key(KeyGame)] IStatusGameChecker statusGameChecker,
            IWorldCreator worldCreator,
            LevelCreator levelCreator,
            PlayerCreator playerCreator,
            EnemyCreator enemyCreator,
            ICameraService cameraService,
            IAB_LevelSettings levelSettings,
            IEnumerable<IAB_GameService> gameServices,
            AB_GameSession gameSession,
            IAB_Gameplay gameplay)
        {
            _statusGameChecker = statusGameChecker;
            _worldCreator = worldCreator;
            _levelCreator = levelCreator;
            _playerCreator = playerCreator;
            _enemyCreator = enemyCreator;
            _cameraService = cameraService;
            _levelSettings = levelSettings;
            _gameServices = gameServices;
            _gameSession = gameSession;
            _gameplay = gameplay;
        }

        public override async UniTask PrepareGameAsync(CancellationToken cancellationToken)
        {
            _worldCreator.CreateWorld(_levelSettings);
            _cameraService.SetFocus(CameraTargetType.AutoBattlerLevel);
            _gameSession.LevelModel = await _levelCreator.CreateEntityAsync(_levelSettings, cancellationToken);
            _gameSession.PlayerModel = await _playerCreator.CreateEntityAsync(_gameSession.LevelModel.EntityData, cancellationToken);
            var enemyModels = new List<IEntityModel<IEnemyData, IEnemyBehavior>>();
            for (int i = 0; i < _gameSession.LevelModel.EntityData.LevelEnemies.Count; i++)
            {
                enemyModels.Add(await _enemyCreator.CreateEntityAsync((i, _gameSession.LevelModel.EntityData), cancellationToken));
            }
            _gameSession.EnemyModels = enemyModels;
        }

        public override void Dispose()
        {
            _levelCreator?.Dispose();
            _playerCreator?.Dispose();
            _enemyCreator?.Dispose();
            _disposed?.Dispose();
            _worldCreator?.DestroyWorld();
            _gameSession.LevelModel = null;
            _gameSession.PlayerModel = null;
            _gameSession.EnemyModels = null;
        }

        public override async UniTask<GameResult> GameAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _gameServices)
            {
                service.EnableService();
            }
            _cameraService.SetFocus(CameraTargetType.AutoBattlerPlayer);
            await _gameplay.GameCycle();
            await UniTask.WaitWhile(() => _statusGameChecker.HasResult == false);
            foreach (var service in _gameServices)
            {
                service.DisableService();
            }
            return _statusGameChecker.GameResult;
        }
    }
}
