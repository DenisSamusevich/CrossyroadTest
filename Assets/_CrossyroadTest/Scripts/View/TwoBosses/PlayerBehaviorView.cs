using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
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
    using BehaviorCommand = PlayerBehavior.BehaviorCommand;

    public class PlayerBehaviorView : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider _capsuleCollider;
        private IAsyncSubscriber<PlayerCommand, BehaviorCommand> _asyncBehaviorSubscriber;
        private IWorldObjectService _worldObjectService;
        private IDisposable _disposable;
        private IPlayerData _playerData;

        [Inject]
        public void Initialize(IAsyncSubscriber<PlayerCommand, BehaviorCommand> asyncBehaviorSubscriber,
            IWorldObjectService worldObjectService)
        {
            _asyncBehaviorSubscriber = asyncBehaviorSubscriber;
            _worldObjectService = worldObjectService;
        }

        private void OnDestroy()
        {
            _capsuleCollider = null;
            _asyncBehaviorSubscriber = null;
            _worldObjectService = null;
        }

        public void SetPlayerData(IPlayerData playerData)
        {
            _capsuleCollider.enabled = true;
            _playerData = playerData;
            var bag = DisposableBag.CreateBuilder();
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Move, MovePlayer).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Dance, DancePlayer).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Push, PushPlayer).AddTo(bag);
            _disposable = bag.Build();
        }

        public void DisposePlayer()
        {
            _capsuleCollider.enabled = false;
            _disposable?.Dispose();
            _playerData = null;
            _disposable = null;
        }

        protected async UniTask PushPlayer(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(transform.position, new Vector3(transform.position.x, 0, transform.position.z), 0.3f).BindToPosition(transform);
            await LMotion.Create(Vector3.one, new Vector3(1f, 0.05f, 1f), 1f).BindToLocalScale(transform);
        }

        protected async UniTask DancePlayer(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(transform.position, transform.position + Vector3.up, 0.4f)
                .WithEase(Ease.InCubic).WithLoops(6, LoopType.Yoyo).BindToPosition(transform);
        }

        protected async UniTask MovePlayer(BehaviorCommand command, CancellationToken token)
        {
            await LSequence.Create()
                .Append(LMotion.Create(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(_worldObjectService.CurrentWorld.ZeroPosition.x + _playerData.Position.x, _worldObjectService.CurrentWorld.ZeroPosition.z + _playerData.Position.y), 0.3f).BindToPositionXZ(transform))
                .Join(LMotion.Create(transform.position.y, 1, 0.15f).WithEase(Ease.InOutCubic).WithLoops(2, LoopType.Yoyo).BindToPositionY(transform))
                .Run().ToUniTask(token).SuppressCancellationThrow();
        }
    }
}
