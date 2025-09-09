using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;


namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.Creators
{
    using EC_Level = EntityCreator<IEnumerable<AreaType>, ILevelData>;
    using EC_RoadArea = EntityCreator<int, IAreaData>;
    using EC_SimpleArea = EntityCreator<AreaType, IAreaData>;

    internal class LevelCreator : MonoBehaviour
    {
        private readonly Vector3 OffsetYForElement = new Vector3(0, -0.5f, 0);
        private IAsyncSubscriber<EC_Level.CreateEntityCommand<ILevelData>> _createLevelSubscriber;
        private IAsyncSubscriber<EC_SimpleArea.CreateEntityCommand<IAreaData>> _createSimpleAreaSubscriber;
        private IAsyncSubscriber<EC_RoadArea.CreateEntityCommand<IAreaData>> _createRoadAreaSubscriber;
        private IDisposable _dispose;
        private ILevelData _levelData;
        [SerializeField] private MeshRenderer _finalAreaElement;
        [SerializeField] private Color _finalAreaColor;
        [SerializeField] private MeshRenderer _freeAreaElement;
        [SerializeField] private Color _freeAreaColor;
        [SerializeField] private MeshRenderer _roadAreaElement;
        [SerializeField] private Color _roadAreaColor;
        [SerializeField] private MeshRenderer _startAreaElement;
        [SerializeField] private Color _startAreaColor;
        [SerializeField] private BoxCollider _trigerCollider;
        [SerializeField] private int _levelWidth = 10;
        private int _currentLineIndex = 0;
        private bool _levelIsCreated;
        private Dictionary<AreaType, MaterialPropertyBlock> materialBlocks = new Dictionary<AreaType, MaterialPropertyBlock>();
        public int LevelWidth => _levelWidth;
        public int CurrentLineIndex => _currentLineIndex;

        [Inject]
        public void Initialize(IAsyncSubscriber<EC_Level.CreateEntityCommand<ILevelData>> createLevelSubscriber,
            IAsyncSubscriber<EC_SimpleArea.CreateEntityCommand<IAreaData>> createSimpleAreaSubscriber,
            IAsyncSubscriber<EC_RoadArea.CreateEntityCommand<IAreaData>> createRoadAreaSubscriber)
        {
            _createLevelSubscriber = createLevelSubscriber;
            _createSimpleAreaSubscriber = createSimpleAreaSubscriber;
            _createRoadAreaSubscriber = createRoadAreaSubscriber;
            var bag = DisposableBag.CreateBuilder();
            _createLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _createSimpleAreaSubscriber.Subscribe(CreateSimpleArea).AddTo(bag);
            _createRoadAreaSubscriber.Subscribe(CreateRoadArea).AddTo(bag);
            _dispose = bag.Build();
            AddBlock(AreaType.Final, _finalAreaColor);
            AddBlock(AreaType.Free, _freeAreaColor);
            AddBlock(AreaType.Road, _roadAreaColor);
            AddBlock(AreaType.Start, _startAreaColor);

            void AddBlock(AreaType areaType, Color color)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", color);
                materialBlocks.Add(areaType, block);
            }
        }

        private void OnDestroy()
        {
            _dispose.Dispose();
        }

        private async UniTask CreateLevel(EC_Level.CreateEntityCommand<ILevelData> command, CancellationToken token)
        {
            _levelData = command.EntityData;
            _levelIsCreated = true;
            await UniTask.CompletedTask;
        }

        private async UniTask CreateSimpleArea(EC_SimpleArea.CreateEntityCommand<IAreaData> command, CancellationToken token)
        {
            if (_levelIsCreated) return;
            switch (command.EntityData.AreaType)
            {
                case AreaType.Free:
                    await CreateLine(new Vector3(0, 0, _currentLineIndex), _freeAreaElement, materialBlocks[AreaType.Free]);
                    break;
                case AreaType.Start:
                    await CreateLine(new Vector3(0, 0, _currentLineIndex), _startAreaElement, materialBlocks[AreaType.Start]);
                    break;
                case AreaType.Final:
                    await CreateLine(new Vector3(0, 0, _currentLineIndex), _finalAreaElement, materialBlocks[AreaType.Final]);
                    CreateFinalTriggerCollider(_currentLineIndex);
                    break;
            }
            _currentLineIndex++;
        }

        private void CreateFinalTriggerCollider(int currentIndex)
        {
            var position = new Vector3(_levelWidth * 0.5f - 0.5f, 0, _currentLineIndex);
            var trigerCollider = Instantiate(_trigerCollider, position, Quaternion.identity, transform);
            trigerCollider.size = new Vector3(_levelWidth, 0.7f, 0.7f);
        }

        private async UniTask CreateRoadArea(EC_RoadArea.CreateEntityCommand<IAreaData> command, CancellationToken token)
        {
            if (_levelIsCreated) return;
            await CreateLine(new Vector3(0, 0, _currentLineIndex), _roadAreaElement, materialBlocks[AreaType.Road]);
            _currentLineIndex++;
        }

        private async UniTask CreateLine(Vector3 startPosition, MeshRenderer areaElement, MaterialPropertyBlock block)
        {
            for (int i = 0; i < _levelWidth; i++)
            {
                var position = startPosition + i * Vector3.right + OffsetYForElement;
                var meshRenderer = Instantiate(areaElement, position + Vector3.down * 5, Quaternion.identity, transform);
                meshRenderer.SetPropertyBlock(block);
                LMotion.Create(meshRenderer.transform.position, position, 1.5f).WithEase(Ease.OutElastic).BindToPosition(meshRenderer.transform);
                await UniTask.WaitForSeconds(0.02f);
            }
        }
    }
}
