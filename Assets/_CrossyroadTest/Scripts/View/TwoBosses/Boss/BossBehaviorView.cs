using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    public abstract class BossBehaviorView<BossId> : MonoBehaviour
    {
        private IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.SetStateCommand> _setStateSubscriber;
        private IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.CreatingObstacleCommand> _creatingObstacleSubscriber;
        private IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.CreateDamageButtonCommand> _createDamageButtonSubscriber;
        protected ITB_AssetsSettings _assetsSettings;
        protected IWorldObjectService _worldObjectService;
        private IDisposable _disposable;
        protected IBossData _bossData;

        [Inject]
        public void Initialize(IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.SetStateCommand> setStateSubscriber,
            IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.CreatingObstacleCommand> creatingObstacleSubscriber,
            IAsyncSubscriber<BossId, BossBehaviorBase<BossId>.CreateDamageButtonCommand> createDamageButtonSubscriber,
            ITB_AssetsSettings assetsSettings)
        {
            _setStateSubscriber = setStateSubscriber;
            _creatingObstacleSubscriber = creatingObstacleSubscriber;
            _createDamageButtonSubscriber = createDamageButtonSubscriber;
            _assetsSettings = assetsSettings;
        }

        private void OnDestroy()
        {
            _setStateSubscriber = null;
            _creatingObstacleSubscriber = null;
            _createDamageButtonSubscriber = null;
            _assetsSettings = null;
        }

        public void SetBossData(IBossData bossData)
        {
            _bossData = bossData;
            var bag = DisposableBag.CreateBuilder();
            _setStateSubscriber.Subscribe(Id, SetState).AddTo(bag);
            _creatingObstacleSubscriber.Subscribe(Id, CreatingObstacle).AddTo(bag);
            _createDamageButtonSubscriber.Subscribe(Id, CreateDamageButton).AddTo(bag);
            _disposable = bag.Build();
            Init();
        }

        public void DisposeBoss()
        {
            Dispose();
            _disposable?.Dispose();
            _disposable = null;
        }
        protected abstract BossId Id { get; }

        protected abstract void Init();
        protected abstract void Dispose();

        protected abstract UniTask CreateDamageButton(BossBehaviorBase<BossId>.CreateDamageButtonCommand command, CancellationToken token);

        protected abstract UniTask CreatingObstacle(BossBehaviorBase<BossId>.CreatingObstacleCommand command, CancellationToken token);

        protected abstract UniTask SetState(BossBehaviorBase<BossId>.SetStateCommand command, CancellationToken token);
    }
}
