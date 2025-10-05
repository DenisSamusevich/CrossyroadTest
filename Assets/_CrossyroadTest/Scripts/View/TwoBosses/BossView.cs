using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss;
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
    using CreateFirstBossCommand = AsyncEntityService<ILevelData, FirstBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>.CreateEntityCommand;
    using DisposeFirstBossCommand = AsyncEntityService<ILevelData, FirstBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>.DisposeEntityCommand;
    using CreateSecondBossCommand = AsyncEntityService<ILevelData, SecondBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>.CreateEntityCommand;
    using DisposeSecondBossCommand = AsyncEntityService<ILevelData, SecondBoss, IBossData, IEntityModel<IBossData, IBossBehavior>>.DisposeEntityCommand;

    internal class BossView : MonoBehaviour
    {
        private IAsyncSubscriber<CreateFirstBossCommand> _createFirstBossSubscriber;
        private ISubscriber<DisposeFirstBossCommand> _disposeFirstBossSubscriber;
        private IAsyncSubscriber<CreateSecondBossCommand> _createSecondBossSubscriber;
        private ISubscriber<DisposeSecondBossCommand> _disposeSecondBossSubscriber;
        private ITB_AssetsSettings _assetsSettings;
        private IWorldObjectService _worldObjectService;
        private IDisposable _disposed;
        private FirstBossView _firstBossView;
        private SecondBossView _secondBossView;

        [Inject]
        public void Initialize(
            IAsyncSubscriber<CreateFirstBossCommand> createFirstBossSubscriber,
            ISubscriber<DisposeFirstBossCommand> disposeFirstBossSubscriber,
            IAsyncSubscriber<CreateSecondBossCommand> createSecondBossSubscriber,
            ISubscriber<DisposeSecondBossCommand> disposeSecondBossSubscriber,
            ITB_AssetsSettings assetsSettings,
            IWorldObjectService worldObjectService)
        {
            _createFirstBossSubscriber = createFirstBossSubscriber;
            _disposeFirstBossSubscriber = disposeFirstBossSubscriber;
            _createSecondBossSubscriber = createSecondBossSubscriber;
            _disposeSecondBossSubscriber = disposeSecondBossSubscriber;
            _assetsSettings = assetsSettings;
            _worldObjectService = worldObjectService;
            var bag = DisposableBag.CreateBuilder();
            _createFirstBossSubscriber.Subscribe(CreateFirstBoss).AddTo(bag);
            _disposeFirstBossSubscriber.Subscribe(DestroyFirstBoss).AddTo(bag);
            _createSecondBossSubscriber.Subscribe(CreateSecondBoss).AddTo(bag);
            _disposeSecondBossSubscriber.Subscribe(DestroySecondBoss).AddTo(bag);
            _disposed = bag.Build();
        }

        private void OnDestroy()
        {
            _disposed?.Dispose();
            _createFirstBossSubscriber = null;
            _disposeFirstBossSubscriber = null;
            _createSecondBossSubscriber = null;
            _disposeSecondBossSubscriber = null;
            _assetsSettings = null;
            _worldObjectService = null;
        }

        private async UniTask CreateFirstBoss(CreateFirstBossCommand command, CancellationToken token)
        {
            var position = _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.EntityModel.EntityData.BossPosition.x, 0, command.EntityModel.EntityData.BossPosition.y);
            _firstBossView = _worldObjectService.GetComponent(_assetsSettings.FirstBoss, position, Quaternion.identity);
            await LSequence.Create().Append(LMotion.Create(_firstBossView.transform.position, position, 2).WithEase(Ease.InQuint).BindToPosition(_firstBossView.transform))
                .Append(LMotion.Create(_firstBossView.transform.localScale, Vector3.one, 2).WithEase(Ease.InElastic).BindToLocalScale(_firstBossView.transform)).Run().ToUniTask();
            _firstBossView.SetBossData(command.EntityModel.EntityData);
        }

        private void DestroyFirstBoss(DisposeFirstBossCommand command)
        {
            _firstBossView?.DisposeBoss();
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly == false)
            {
                _worldObjectService.ReturnComponent(_assetsSettings.FirstBoss, _firstBossView);
            }
            _firstBossView = null;
        }

        private async UniTask CreateSecondBoss(CreateSecondBossCommand command, CancellationToken token)
        {
            var position = _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.EntityModel.EntityData.BossPosition.x, 0, command.EntityModel.EntityData.BossPosition.y);
            _secondBossView = _worldObjectService.GetComponent(_assetsSettings.SecondBoss, position, Quaternion.identity);
            await LSequence.Create().Append(LMotion.Create(_secondBossView.transform.position, position, 2).WithEase(Ease.InQuint).BindToPosition(_secondBossView.transform))
                .Append(LMotion.Create(_secondBossView.transform.localScale, Vector3.one, 2).WithEase(Ease.InElastic).BindToLocalScale(_secondBossView.transform)).Run().ToUniTask();
            _secondBossView.SetBossData(command.EntityModel.EntityData);
        }

        private void DestroySecondBoss(DisposeSecondBossCommand command)
        {
            _secondBossView?.DisposeBoss();
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly == false)
            {
                _worldObjectService.ReturnComponent(_assetsSettings.SecondBoss, _secondBossView);
            }
            _secondBossView = null;
        }
    }
}
