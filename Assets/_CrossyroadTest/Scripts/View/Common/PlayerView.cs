using Assets._CrossyroadTest.Scripts.Common.Player;
using Cysharp.Threading.Tasks;
using LitMotion;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.Common
{
    internal abstract class PlayerView<TPlayer, TPlayerData> : MonoBehaviour where TPlayer : PlayerBase<TPlayer>
    {
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private Color _color;
        [SerializeField] private MeshRenderer _meshRenderer;
        private IAsyncSubscriber<PlayerBase<TPlayer>.MovePlayerCommand<TPlayer>> _movePlayerSubscriber;
        private IAsyncSubscriber<PlayerBase<TPlayer>.DancePlayerCommand<TPlayer>> _dancePlayerSubscriber;
        private IAsyncSubscriber<PlayerBase<TPlayer>.PushPlayerCommand<TPlayer>> _pushPlayerSubscriber;
        private IDisposable _disposable;
        protected TPlayerData _playerData;
        [Inject]
        public void Initialize(IAsyncSubscriber<PlayerBase<TPlayer>.MovePlayerCommand<TPlayer>> movePlayerSubscriber,
            IAsyncSubscriber<PlayerBase<TPlayer>.DancePlayerCommand<TPlayer>> dancePlayerSubscriber,
            IAsyncSubscriber<PlayerBase<TPlayer>.PushPlayerCommand<TPlayer>> pushPlayerSubscriber)
        {
            _movePlayerSubscriber = movePlayerSubscriber;
            _dancePlayerSubscriber = dancePlayerSubscriber;
            _pushPlayerSubscriber = pushPlayerSubscriber;
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", _color);
            _meshRenderer.SetPropertyBlock(block);
        }

        public void SetPlayerData(TPlayerData playerData)
        {
            capsuleCollider.enabled = true;
            _playerData = playerData;
            var bag = DisposableBag.CreateBuilder();
            _movePlayerSubscriber.Subscribe(MovePlayer).AddTo(bag);
            _dancePlayerSubscriber.Subscribe(DancePlayer).AddTo(bag);
            _pushPlayerSubscriber.Subscribe(PushPlayer).AddTo(bag);
            _disposable = bag.Build();
            Init();
        }

        public void DisposePlayer()
        {
            capsuleCollider.enabled = false;
            _disposable?.Dispose();
        }
        protected abstract void Init();
        protected abstract UniTask PushPlayer(PlayerBase<TPlayer>.PushPlayerCommand<TPlayer> command, CancellationToken token);

        protected abstract UniTask DancePlayer(PlayerBase<TPlayer>.DancePlayerCommand<TPlayer> command, CancellationToken token);

        protected abstract UniTask MovePlayer(PlayerBase<TPlayer>.MovePlayerCommand<TPlayer> command, CancellationToken token);
    }
}
