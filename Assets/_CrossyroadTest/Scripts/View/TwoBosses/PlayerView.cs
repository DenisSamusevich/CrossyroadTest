using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses
{
    using CreatePlayerCommand = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.CreateEntityCommand;
    using DisposePlayerCommand = AsyncEntityService<ILevelData, Player, IPlayerData, IEntityModel<IPlayerData, IPlayerBehavior>>.DisposeEntityCommand;

    internal class PlayerView : MonoBehaviour
    {
        private IAsyncSubscriber<CreatePlayerCommand> _createPlayerPublisher;
        private ISubscriber<DisposePlayerCommand> _disposePlayerPublisher;
        private IWorldObjectService _worldObjectService;
        private ITB_AssetsSettings _assetsSettings;
        private IDisposable _disposed;
        private PlayerBehaviorView _currentPlayer;

        [Inject]
        public void Initialize(
            IAsyncSubscriber<CreatePlayerCommand> createPlayerPublisher,
            ISubscriber<DisposePlayerCommand> disposePlayerPublisher,
            IWorldObjectService worldObjectService,
            ITB_AssetsSettings assetsSettings)
        {
            _createPlayerPublisher = createPlayerPublisher;
            _disposePlayerPublisher = disposePlayerPublisher;
            _worldObjectService = worldObjectService;
            _assetsSettings = assetsSettings;
            var bag = DisposableBag.CreateBuilder();
            _createPlayerPublisher.Subscribe(CreatePlayer).AddTo(bag);
            _disposePlayerPublisher.Subscribe(DestroyPlayer).AddTo(bag);
            _disposed = bag.Build();
        }

        private async UniTask CreatePlayer(CreatePlayerCommand command, CancellationToken token)
        {
            var position = _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.EntityModel.EntityData.Position.x, 0, command.EntityModel.EntityData.Position.y);
            _currentPlayer = _worldObjectService.GetPlayerComponent(_assetsSettings.PlayerBehavior, position, Quaternion.identity);
            await LSequence.Create().Append(LMotion.Create(_currentPlayer.transform.position, position, 2).WithEase(Ease.InQuint).BindToPosition(_currentPlayer.transform))
                    .Append(LMotion.Create(_currentPlayer.transform.localScale, Vector3.one, 2).WithEase(Ease.InElastic).BindToLocalScale(_currentPlayer.transform)).Run().ToUniTask();
            _currentPlayer.SetPlayerData(command.EntityModel.EntityData);
        }

        private void DestroyPlayer(DisposePlayerCommand command)
        {
            _currentPlayer?.DisposePlayer();
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly == false)
            {
                _worldObjectService.ReturnPlayerComponent(_assetsSettings.PlayerBehavior, _currentPlayer);
            }
            _currentPlayer = null;
        }

        private void OnDestroy()
        {
            _disposed?.Dispose();
            _createPlayerPublisher = null;
            _disposePlayerPublisher = null;
            _worldObjectService = null;
            _assetsSettings = null;
            _disposed = null;
        }
    }
}
