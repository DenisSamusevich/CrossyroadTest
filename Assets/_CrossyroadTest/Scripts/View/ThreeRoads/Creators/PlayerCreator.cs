using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.View.Common;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.Creators
{
    using EC_Player = EntityCreator<IEnumerable<AreaType>, IPlayerData>;

    internal class PlayerCreator : MonoBehaviour
    {
        [SerializeField] private PlayerView.PlayerView _playerView;
        private IObjectResolver _objectResolver;
        private IAsyncSubscriber<EC_Player.CreateEntityCommand<IPlayerData>> _createPlayerPublisher;
        private ISubscriber<EC_Player.DisposeEntityCommand<IPlayerData>> _disposePlayerPublisher;
        private PlayerViewContainer _playerViewContainer;
        private IDisposable _disposed;
        private PlayerView.PlayerView _currentPlayer;
        public PlayerView.PlayerView CurrentPlayer { get => _currentPlayer; }

        [Inject]
        public void Initialize(IObjectResolver objectResolver,
            IAsyncSubscriber<EC_Player.CreateEntityCommand<IPlayerData>> createPlayerPublisher,
            ISubscriber<EC_Player.DisposeEntityCommand<IPlayerData>> disposePlayerPublisher,
            PlayerViewContainer playerViewContainer)
        {
            _objectResolver = objectResolver;
            _createPlayerPublisher = createPlayerPublisher;
            _disposePlayerPublisher = disposePlayerPublisher;
            _playerViewContainer = playerViewContainer;
            var bag = DisposableBag.CreateBuilder();
            _createPlayerPublisher.Subscribe(CreatePlayer).AddTo(bag);
            _disposePlayerPublisher.Subscribe(DestroyPlayer).AddTo(bag);
            _disposed = bag.Build();
        }

        private async UniTask CreatePlayer(EC_Player.CreateEntityCommand<IPlayerData> command, CancellationToken token)
        {
            if (_currentPlayer == null)
            {
                _currentPlayer = _objectResolver.Instantiate(_playerView, new Vector3(command.EntityData.Position.x, 0, command.EntityData.Position.y), Quaternion.identity);
                _playerViewContainer.AddPlayerView(_currentPlayer);
            }
            else
            {
                await LSequence.Create().Append(LMotion.Create(_currentPlayer.transform.position, new Vector3(command.EntityData.Position.x, 0, command.EntityData.Position.y), 2).WithEase(Ease.InQuint).BindToPosition(_currentPlayer.transform))
                    .Append(LMotion.Create(_currentPlayer.transform.localScale, Vector3.one, 2).WithEase(Ease.InElastic).BindToLocalScale(_currentPlayer.transform)).Run().ToUniTask();
            }
            _currentPlayer.SetPlayerData(command.EntityData);
        }

        private void DestroyPlayer(EC_Player.DisposeEntityCommand<IPlayerData> command)
        {
            _currentPlayer?.DisposePlayer();
        }

        private void OnDestroy()
        {
            _disposed.Dispose();
        }
    }
}
