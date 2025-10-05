using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler
{
    using CreateLevelCommand = AsyncEntityService<IAB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>.CreateEntityCommand;
    using DisposeLevelCommand = AsyncEntityService<IAB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>.DisposeEntityCommand;

    internal class LevelView : MonoBehaviour
    {
        private readonly Vector3 OffsetYForElement = new Vector3(0, -0.5f, 0);
        private IAsyncSubscriber<CreateLevelCommand> _createLevelSubscriber;
        private ISubscriber<DisposeLevelCommand> _disposeLevelSubscriber;
        private IWorldObjectService _worldObjectService;
        private IAB_AssetsSettings _assetsSettings;
        private IDisposable _dispose;
        private ILevelData _levelData;
        private bool _levelIsCreated;
        private List<MeshRenderer> _levelElements = new List<MeshRenderer>();

        [Inject]
        public void Initialize(IAsyncSubscriber<CreateLevelCommand> createLevelSubscriber,
            ISubscriber<DisposeLevelCommand> disposeLevelSubscriber,
            IWorldObjectService worldObjectService,
            IAB_AssetsSettings assetsSettings)
        {
            _createLevelSubscriber = createLevelSubscriber;
            _disposeLevelSubscriber = disposeLevelSubscriber;
            _worldObjectService = worldObjectService;
            _assetsSettings = assetsSettings;
            var bag = DisposableBag.CreateBuilder();
            _createLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _disposeLevelSubscriber.Subscribe(DestroyLevel).AddTo(bag);
            _dispose = bag.Build();
        }

        private async UniTask CreateLevel(CreateLevelCommand command, CancellationToken token)
        {
            _levelData = command.EntityModel.EntityData;
            if (_levelIsCreated) return;
            await CreateLine(_worldObjectService.CurrentWorld.ZeroPosition, 10, _assetsSettings.LevelElement, _levelElements);
            _levelIsCreated = true;
        }

        private void DestroyLevel(DisposeLevelCommand command)
        {
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || (_worldObjectService.CurrentWorld.IsUsedRepeatedly == false))
            {
                foreach (var item in _levelElements)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.LevelElement, item);
                }
                _levelElements.Clear();
                _levelData = null;
                _levelIsCreated = false;
            }
        }

        private void OnDestroy()
        {
            _dispose.Dispose();
            _dispose = null;
            _createLevelSubscriber = null;
            _disposeLevelSubscriber = null;
            _assetsSettings = null;
        }

        private async UniTask CreateLine(Vector3 startPosition, int levelWidth, ObjectPoolBase<MeshRenderer> objectPoolBase, List<MeshRenderer> elements)
        {
            for (int i = 0; i < levelWidth; i++)
            {
                var position = startPosition + i * Vector3.forward + OffsetYForElement;
                var meshRenderer = _worldObjectService.GetComponent(objectPoolBase, position + Vector3.down * 5, Quaternion.identity, transform);
                elements.Add(meshRenderer);
                LMotion.Create(meshRenderer.transform.position, position, 1.5f).WithEase(Ease.OutElastic).BindToPosition(meshRenderer.transform);
                await UniTask.WaitForSeconds(0.02f);
            }
        }
    }
}
