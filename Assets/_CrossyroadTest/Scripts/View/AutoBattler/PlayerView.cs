using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Cysharp.Threading.Tasks;
using LitMotion;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler
{
    using CreatePlayerCommand = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.CreateEntityCommand;
    using DisposePlayerCommand = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.DisposeEntityCommand;

    internal class PlayerView : MonoBehaviour
    {
        private readonly Vector3 PlayerPosition = new Vector3(0, 0, 3);
        private IAsyncSubscriber<CreatePlayerCommand> _createPlayerPublisher;
        private ISubscriber<DisposePlayerCommand> _disposePlayerPublisher;
        private IAB_AssetsSettings _assetsSettings;
        private IWorldObjectService _worldObjectService;
        private IDisposable _disposed;
        private PlayerBehaviorView _currentPlayer;

        [Inject]
        public void Initialize(IAsyncSubscriber<CreatePlayerCommand> createPlayerPublisher,
            ISubscriber<DisposePlayerCommand> disposePlayerPublisher,
            IAB_AssetsSettings assetsSettings,
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
            _currentPlayer = _worldObjectService.GetPlayerComponent(_assetsSettings.PlayerBehavior,
                _worldObjectService.CurrentWorld.ZeroPosition + PlayerPosition,
                Quaternion.identity, transform);
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
