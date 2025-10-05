using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads
{
    using CreatePlayerCommand = AsyncEntityService<ITR_LevelSettings, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.CreateEntityCommand;
    using DisposePlayerCommand = AsyncEntityService<ITR_LevelSettings, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.DisposeEntityCommand;

    internal class PlayerView : MonoBehaviour
    {
        private IAsyncSubscriber<CreatePlayerCommand> _createPlayerPublisher;
        private ISubscriber<DisposePlayerCommand> _disposePlayerPublisher;
        private ITR_AssetsSettings _assetsSettings;
        private IWorldObjectService _worldObjectService;
        private IDisposable _disposed;
        private PlayerBehaviorView _currentPlayer;

        [Inject]
        public void Initialize(IAsyncSubscriber<CreatePlayerCommand> createPlayerPublisher,
            ISubscriber<DisposePlayerCommand> disposePlayerPublisher,
            ITR_AssetsSettings assetsSettings,
            IWorldObjectService worldObjectService)
        {
            _createPlayerPublisher = createPlayerPublisher;
            _disposePlayerPublisher = disposePlayerPublisher;
            _assetsSettings = assetsSettings;
            _worldObjectService = worldObjectService;
            var bag = DisposableBag.CreateBuilder();
            _createPlayerPublisher.Subscribe(CreatePlayer).AddTo(bag);
            _disposePlayerPublisher.Subscribe(DestroyPlayer).AddTo(bag);
            _disposed = bag.Build();
        }
        private void OnDestroy()
        {
            _createPlayerPublisher = null;
            _disposePlayerPublisher = null;
            _assetsSettings = null;
            _worldObjectService = null;
            _disposed?.Dispose();
        }

        private async UniTask CreatePlayer(CreatePlayerCommand command, CancellationToken token)
        {
            _currentPlayer = _worldObjectService.GetPlayerComponent(_assetsSettings.PlayerBehavior, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.EntityModel.EntityData.Position.x, 0, command.EntityModel.EntityData.Position.y), Quaternion.identity);
            await LSequence.Create()
                .Append(
                    LMotion.Create(_currentPlayer.transform.position, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.EntityModel.EntityData.Position.x, 0, command.EntityModel.EntityData.Position.y), 2)
                    .WithEase(Ease.InQuint)
                    .BindToPosition(_currentPlayer.transform))
                .Append(
                    LMotion.Create(_currentPlayer.transform.localScale, Vector3.one, 2)
                    .WithEase(Ease.InElastic)
                    .BindToLocalScale(_currentPlayer.transform))
                .Run().ToUniTask();
            _currentPlayer.SetPlayerData(command.EntityModel.EntityData);
        }

        private void DestroyPlayer(DisposePlayerCommand command)
        {
            _currentPlayer?.DisposePlayer();
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly)
            {
                _worldObjectService.ReturnPlayerComponent(_assetsSettings.PlayerBehavior, _currentPlayer);
            }
            _currentPlayer = null;
        }
    }
}
