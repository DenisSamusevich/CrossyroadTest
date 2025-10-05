using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Extensions;
using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses
{
    using CreateLevelCommand = AsyncEntityService<ITB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>.CreateEntityCommand;
    using DisposeLevelCommand = AsyncEntityService<ITB_LevelSettings, Level, ILevelData, IEntityModel<ILevelData>>.DisposeEntityCommand;

    internal class LevelView : MonoBehaviour
    {
        private readonly Vector3 OffsetYForElement = new Vector3(0, -0.5f, 0);

        private IAsyncSubscriber<CreateLevelCommand> _createLevelSubscriber;
        private ISubscriber<DisposeLevelCommand> _disposeLevelSubscriber;
        private IWorldObjectService _worldObjectService;
        private ITB_AssetsSettings _assetsSettings;
        private IDisposable _dispose;
        private ILevelData _levelData;
        private bool _levelIsCreated;
        private List<MeshRenderer> _wallElements = new List<MeshRenderer>();
        private List<MeshRenderer> _freeLevelElements = new List<MeshRenderer>();

        [Inject]
        public void Initialize(IAsyncSubscriber<CreateLevelCommand> createLevelSubscriber,
            ISubscriber<DisposeLevelCommand> disposeLevelSubscriber,
            IWorldObjectService worldObjectService,
            ITB_AssetsSettings assetsSettings)
        {
            _createLevelSubscriber = createLevelSubscriber;
            _disposeLevelSubscriber = disposeLevelSubscriber;
            _worldObjectService = worldObjectService;
            _assetsSettings = assetsSettings;
            var bag = DisposableBag.CreateBuilder();
            _createLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _disposeLevelSubscriber.Subscribe(DisposeLevel).AddTo(bag);
            _dispose = bag.Build();
        }

        private void OnDestroy()
        {
            _dispose?.Dispose();
            _createLevelSubscriber = null;
            _disposeLevelSubscriber = null;
            _worldObjectService = null;
            _assetsSettings = null;
            _dispose = null;
        }

        private async UniTask CreateLevel(CreateLevelCommand command, CancellationToken token)
        {
            _levelData = command.EntityModel.EntityData;
            var width = _levelData.Width + 1;
            var length = _levelData.Length + 1;

            if (_levelIsCreated) return;

            await UniTask.WhenAll(
                CreateLine(_worldObjectService.CurrentWorld.ZeroPosition, width, Vector3.right, _assetsSettings.WallElement, _wallElements),
                CreateLine(_worldObjectService.CurrentWorld.ZeroPosition.AddX(width), length, Vector3.forward, _assetsSettings.WallElement, _wallElements),
                CreateLine(_worldObjectService.CurrentWorld.ZeroPosition.AddX(width).AddZ(length), width, Vector3.left, _assetsSettings.WallElement, _wallElements),
                CreateLine(_worldObjectService.CurrentWorld.ZeroPosition.AddZ(length), length, Vector3.back, _assetsSettings.WallElement, _wallElements)
            );

            for (int i = 1; i < length / 2 + 1; i++)
            {
                await UniTask.WhenAll(
                    CreateLine(_worldObjectService.CurrentWorld.ZeroPosition + new Vector3(i, 0, i), width - 2 * i, Vector3.right, _assetsSettings.FreeLevelElement, _freeLevelElements),
                    CreateLine(_worldObjectService.CurrentWorld.ZeroPosition + new Vector3(width - i, 0, +length - i), width - 2 * i, Vector3.left, _assetsSettings.FreeLevelElement, _freeLevelElements)
                );
            }
            for (int j = 1; j < width / 2 + 1; j++)
            {
                await UniTask.WhenAll(
                    CreateLine(_worldObjectService.CurrentWorld.ZeroPosition + new Vector3(width - j, 0, j), length - 2 * j, Vector3.forward, _assetsSettings.FreeLevelElement, _freeLevelElements),
                    CreateLine(_worldObjectService.CurrentWorld.ZeroPosition + new Vector3(j, 0, length - j), length - 2 * j, Vector3.back, _assetsSettings.FreeLevelElement, _freeLevelElements)
                );
            }
            if ((width % 2) == 0 && length == width)
            {
                await CreateLine(new Vector3(width / 2, 0, length / 2), 1, Vector3.right, _assetsSettings.FreeLevelElement, _freeLevelElements);
            }
            _levelIsCreated = true;

            async UniTask CreateLine(Vector3 startPosition, int count, Vector3 direction, ObjectPoolBase<MeshRenderer> objectPoolBase, List<MeshRenderer> elements)
            {
                for (int i = 0; i < count; i++)
                {
                    var position = startPosition + i * direction + OffsetYForElement;
                    var element = _worldObjectService.GetComponent(objectPoolBase, position + Vector3.down * 5, Quaternion.identity, transform);
                    LMotion.Create(element.transform.position, position, 1.5f).WithEase(Ease.OutElastic).BindToPosition(element.transform);
                    elements.Add(element);
                    await UniTask.WaitForSeconds(0.1f);
                }
            }
        }

        private void DisposeLevel(DisposeLevelCommand command)
        {
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly == false)
            {
                foreach (var item in _wallElements)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.WallElement, item);
                }
                _wallElements.Clear();
                foreach (var item in _freeLevelElements)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.FreeLevelElement, item);
                }
                _freeLevelElements.Clear();
                _levelIsCreated = false;
            }

            _levelData = null;
        }
    }
}
