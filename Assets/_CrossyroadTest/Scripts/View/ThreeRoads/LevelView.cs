using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;


namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads
{
    using CreateLevelCommand = AsyncEntityService<ITR_LevelSettings, Level, ILevelData, IEntityModel<ILevelData, ILevelBehavior>>.CreateEntityCommand;
    using CreateSimpleAreaCommand = AsyncEntityService<(int areaPosition, int levelWidth, AreaType areaType), SimpleArea, IAreaData, IEntityModel<IAreaData>>.CreateEntityCommand;
    using DisposeLevelCommand = AsyncEntityService<ITR_LevelSettings, Level, ILevelData, IEntityModel<ILevelData, ILevelBehavior>>.DisposeEntityCommand;

    internal class LevelView : MonoBehaviour
    {
        private readonly Vector3 OffsetYForElement = new Vector3(0, -0.5f, 0);
        private IAsyncSubscriber<CreateLevelCommand> _createLevelSubscriber;
        private ISubscriber<DisposeLevelCommand> _disposeLevelSubscriber;
        private IAsyncSubscriber<CreateSimpleAreaCommand> _createSimpleAreaSubscriber;
        private IWorldObjectService _worldObjectService;
        private ITR_AssetsSettings _assetsSettings;
        private IDisposable _dispose;
        private ILevelData _levelData;
        private bool _levelIsCreated;
        private List<MeshRenderer> _freeAreaElement = new List<MeshRenderer>();
        private List<MeshRenderer> _startAreaElement = new List<MeshRenderer>();
        private List<MeshRenderer> _roadAreaElement = new List<MeshRenderer>();
        private List<MeshRenderer> _finalAreaElement = new List<MeshRenderer>();
        private BoxCollider _trigerCollider;

        [Inject]
        public void Initialize(IAsyncSubscriber<CreateLevelCommand> createLevelSubscriber,
            ISubscriber<DisposeLevelCommand> disposeLevelSubscriber,
            IAsyncSubscriber<CreateSimpleAreaCommand> createSimpleAreaSubscriber,
            IWorldObjectService worldObjectService,
            ITR_AssetsSettings assetsSettings)
        {
            _createLevelSubscriber = createLevelSubscriber;
            _disposeLevelSubscriber = disposeLevelSubscriber;
            _createSimpleAreaSubscriber = createSimpleAreaSubscriber;
            _worldObjectService = worldObjectService;
            _assetsSettings = assetsSettings;
            var bag = DisposableBag.CreateBuilder();
            _createLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _disposeLevelSubscriber.Subscribe(DestroyLevel).AddTo(bag);
            _createSimpleAreaSubscriber.Subscribe(CreateSimpleArea).AddTo(bag);
            _dispose = bag.Build();
        }
        private void OnDestroy()
        {
            _dispose?.Dispose();
            _dispose = null;
            _createLevelSubscriber = null;
            _disposeLevelSubscriber = null;
            _createSimpleAreaSubscriber = null;
            _assetsSettings = null;
        }

        private async UniTask CreateLevel(CreateLevelCommand command, CancellationToken token)
        {
            _levelData = command.EntityModel.EntityData;
            _levelIsCreated = true;
            await UniTask.CompletedTask;
        }

        private async UniTask CreateSimpleArea(CreateSimpleAreaCommand command, CancellationToken token)
        {
            if (_levelIsCreated) return;
            await CreateLine(_worldObjectService.CurrentWorld.ZeroPosition + new Vector3(0, 0, command.EntityModel.EntityData.AreaPosition), command.EntityModel.EntityData.LevelWidth,
            command.EntityModel.EntityData.AreaType switch
            {
                AreaType.Free => _assetsSettings.FreeAreaElement,
                AreaType.Start => _assetsSettings.StartAreaElement,
                AreaType.Road => _assetsSettings.RoadAreaElement,
                AreaType.Final => _assetsSettings.FinalAreaElement,
                _ => throw new NotImplementedException()
            },
            command.EntityModel.EntityData.AreaType switch
            {
                AreaType.Free => _freeAreaElement,
                AreaType.Start => _startAreaElement,
                AreaType.Road => _roadAreaElement,
                AreaType.Final => _finalAreaElement,
                _ => throw new NotImplementedException()
            });
            if (command.EntityModel.EntityData.AreaType == AreaType.Final)
            {
                CreateFinalTriggerCollider(command.EntityModel.EntityData.AreaPosition, command.EntityModel.EntityData.LevelWidth);
            }
        }

        private void CreateFinalTriggerCollider(int currentIndex, int levelWidth)
        {
            var position = new Vector3(levelWidth * 0.5f - 0.5f, 0, currentIndex);
            _trigerCollider = _worldObjectService.GetComponent(_assetsSettings.TriggerCollider, position, Quaternion.identity, transform);
            _trigerCollider.size = new Vector3(levelWidth, 0.7f, 0.7f);
        }

        private async UniTask CreateLine(Vector3 startPosition, int levelWidth, ObjectPoolBase<MeshRenderer> objectPoolBase, List<MeshRenderer> elements)
        {
            for (int i = 0; i < levelWidth; i++)
            {
                var position = startPosition + i * Vector3.right + OffsetYForElement;
                var meshRenderer = _worldObjectService.GetComponent(objectPoolBase, position + Vector3.down * 5, Quaternion.identity, transform);
                elements.Add(meshRenderer);
                LMotion.Create(meshRenderer.transform.position, position, 1.5f).WithEase(Ease.OutElastic).BindToPosition(meshRenderer.transform);
                await UniTask.WaitForSeconds(0.02f);
            }
        }

        private void DestroyLevel(DisposeLevelCommand command)
        {
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly == false)
            {
                foreach (var item in _freeAreaElement)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.FreeAreaElement, item);
                }
                _freeAreaElement.Clear();
                foreach (var item in _startAreaElement)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.StartAreaElement, item);
                }
                _startAreaElement.Clear();
                foreach (var item in _roadAreaElement)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.RoadAreaElement, item);
                }
                _roadAreaElement.Clear();
                foreach (var item in _finalAreaElement)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.FinalAreaElement, item);
                }
                _finalAreaElement.Clear();
                _worldObjectService.ReturnComponent(_assetsSettings.TriggerCollider, _trigerCollider);
                _trigerCollider = null;
                _levelData = null;
                _levelIsCreated = false;
            }
        }
    }
}
