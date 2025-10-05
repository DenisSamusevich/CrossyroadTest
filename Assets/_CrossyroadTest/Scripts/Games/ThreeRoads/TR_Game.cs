using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads
{
    using LevelCreator = AsyncEntityService<ITR_LevelSettings, Level, ILevelData, IEntityModel<ILevelData, ILevelBehavior>>;
    using PlayerCreator = AsyncEntityService<ITR_LevelSettings, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>;

    public class TR_Game : GameBase
    {
        public const string KeyGame = "TR_Game";

        private readonly IStatusGameChecker _statusGameChecker;
        private readonly IWorldCreator _worldCreator;
        private readonly LevelCreator _levelCreator;
        private readonly PlayerCreator _playerCreator;
        private readonly ICameraService _cameraService;
        private readonly ITR_LevelSettings _levelSettings;
        private readonly IEnumerable<ITR_GameService> _gameServices;
        private readonly TR_GameSession _gameSession;
        private IDisposable _disposed;

        public TR_Game([Key(KeyGame)] IStatusGameChecker statusGameChecker,
            IWorldCreator worldCreator,
            LevelCreator levelCreator,
            PlayerCreator playerCreator,
            ICameraService cameraService,
            ITR_LevelSettings levelSettings,
            IEnumerable<ITR_GameService> gameServices,
            TR_GameSession gameSession)
        {
            _statusGameChecker = statusGameChecker;
            _worldCreator = worldCreator;
            _levelCreator = levelCreator;
            _playerCreator = playerCreator;
            _cameraService = cameraService;
            _levelSettings = levelSettings;
            _gameServices = gameServices;
            _gameSession = gameSession;
        }

        public override async UniTask PrepareGameAsync(CancellationToken cancellationToken)
        {
            _worldCreator.CreateWorld(_levelSettings);
            _cameraService.SetFocus(CameraTargetType.ThreeRoadsLevel);
            _gameSession.LevelModel = await _levelCreator.CreateEntityAsync(_levelSettings, cancellationToken);
            _gameSession.PlayerModel = await _playerCreator.CreateEntityAsync(_levelSettings, cancellationToken);
        }

        public override void Dispose()
        {
            _levelCreator.Dispose();
            _playerCreator.Dispose();
            _disposed.Dispose();
            _worldCreator.DestroyWorld();
            _gameSession.LevelModel = null;
            _gameSession.PlayerModel = null;
        }

        public override async UniTask<GameResult> GameAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _gameServices)
            {
                service.EnableService();
            }
            _cameraService.SetFocus(CameraTargetType.ThreeRoadsPlayer);
            while (_statusGameChecker.HasResult == false)
            {
                _gameSession.LevelModel.EntityBehavior.Update();
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
