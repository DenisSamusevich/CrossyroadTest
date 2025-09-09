using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Player;
using Assets._CrossyroadTest.Scripts.View.Common;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Creators
{
    using EC_Player = EntityCreator<ILevelData, IPlayerData>;

    internal class PlayerCreator : MonoBehaviour
    {
        [SerializeField] private ThreeRoads.Creators.PlayerCreator _playerCreator;
        [SerializeField] private PlayerView.PlayerView _playerView;
        [SerializeField] private LevelCreator _levelCreator;
        private PlayerViewContainer _playerViewContainer;
        private IObjectResolver _objectResolver;
        private IAsyncSubscriber<EC_Player.CreateEntityCommand<IPlayerData>> _createPlayerPublisher;
        private ISubscriber<EC_Player.DisposeEntityCommand<IPlayerData>> _disposePlayerPublisher;
        private IDisposable _disposed;
        private PlayerView.PlayerView _currentPlayer;

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
            var position = _levelCreator.GetStartWorldPoint() + new Vector3(command.EntityData.Position.x, 0, command.EntityData.Position.y);
            if (_currentPlayer == null && _playerCreator.CurrentPlayer == null)
            {
                _currentPlayer = _objectResolver.Instantiate(_playerView, position, Quaternion.identity);
                _playerViewContainer.AddPlayerView(_currentPlayer);
            }
            else
            {
                if (_currentPlayer == null)
                {
                    _currentPlayer = _playerCreator.CurrentPlayer.GetComponent<PlayerView.PlayerView>();
                    _playerViewContainer.AddPlayerView(_currentPlayer);
                }
                else
                {
                    _currentPlayer.gameObject.SetActive(true);
                }
                await LSequence.Create().Append(LMotion.Create(_currentPlayer.transform.position, position, 2).WithEase(Ease.InQuint).BindToPosition(_currentPlayer.transform))
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
