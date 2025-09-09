using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Creators
{
    using EC_Level = EntityCreator<Vector2Int, ILevelData>;
    using EC_ThreeRoadsLevel = EntityCreator<IEnumerable<AreaType>, Scripts.ThreeRoads.Entities.Level.ILevelData>;
    using TR_Level = Scripts.ThreeRoads.Entities.Level;

    internal class LevelCreator : MonoBehaviour
    {
        private readonly Vector3 OffsetYForElement = new Vector3(0, -0.5f, 0);
        private IAsyncSubscriber<EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData>> _createThreeRoadsLevelSubscriber;
        private IAsyncSubscriber<EC_Level.CreateEntityCommand<ILevelData>> _createLevelSubscriber;
        private IDisposable _dispose;
        private ILevelData _levelData;
        [SerializeField] private MeshRenderer _wallAreaElement;
        [SerializeField] private Color _wallAreaColor;
        [SerializeField] private MeshRenderer _freeAreaElement;
        [SerializeField] private Color _freeAreaColor;
        private bool _levelIsCreated;
        private int _threeRoadsLevelAreaCount;
        private MaterialPropertyBlock _wallAreaBlock;
        private MaterialPropertyBlock _freeAreaBlock;
        [Inject]
        public void Initialize(IAsyncSubscriber<EC_Level.CreateEntityCommand<ILevelData>> createLevelSubscriber,
            IAsyncSubscriber<EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData>> createThreeRoadsLevelSubscriber)
        {
            _createLevelSubscriber = createLevelSubscriber;
            _createThreeRoadsLevelSubscriber = createThreeRoadsLevelSubscriber;
            var bag = DisposableBag.CreateBuilder();
            _createLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _createThreeRoadsLevelSubscriber.Subscribe(CreateThreeRoadsLevel).AddTo(bag);
            _dispose = bag.Build();
            _wallAreaBlock = new MaterialPropertyBlock();
            _wallAreaBlock.SetColor("_BaseColor", _wallAreaColor);
            _freeAreaBlock = new MaterialPropertyBlock();
            _freeAreaBlock.SetColor("_BaseColor", _freeAreaColor);
        }

        private void OnDestroy()
        {
            _dispose.Dispose();
        }

        private async UniTask CreateLevel(EC_Level.CreateEntityCommand<ILevelData> command, CancellationToken token)
        {
            _levelData = command.EntityData;
            var width = _levelData.Width + 1;
            var length = _levelData.Length + 1;

            if (_levelIsCreated) return;

            await UniTask.WhenAll(
                CreateLine(new Vector3(0, 0, _threeRoadsLevelAreaCount), width, Vector3.right, _wallAreaElement, _wallAreaBlock),
                CreateLine(new Vector3(width, 0, _threeRoadsLevelAreaCount), length, Vector3.forward, _wallAreaElement, _wallAreaBlock),
                CreateLine(new Vector3(width, 0, _threeRoadsLevelAreaCount + length), width, Vector3.left, _wallAreaElement, _wallAreaBlock),
                CreateLine(new Vector3(0, 0, _threeRoadsLevelAreaCount + length), length, Vector3.back, _wallAreaElement, _wallAreaBlock)
            );

            for (int i = 1; i < length / 2 + 1; i++)
            {
                await UniTask.WhenAll(
                    CreateLine(new Vector3(i, 0, _threeRoadsLevelAreaCount + i), width - 2 * i, Vector3.right, _freeAreaElement, _freeAreaBlock),
                    CreateLine(new Vector3(width - i, 0, _threeRoadsLevelAreaCount + length - i), width - 2 * i, Vector3.left, _freeAreaElement, _freeAreaBlock)
                );
            }
            for (int j = 1; j < width / 2 + 1; j++)
            {
                await UniTask.WhenAll(
                    CreateLine(new Vector3(width - j, 0, _threeRoadsLevelAreaCount + j), length - 2 * j, Vector3.forward, _freeAreaElement, _freeAreaBlock),
                    CreateLine(new Vector3(j, 0, _threeRoadsLevelAreaCount + length - j), length - 2 * j, Vector3.back, _freeAreaElement, _freeAreaBlock)
                );
            }
            if ((width % 2) == 0 && length == width)
            {
                await CreateLine(new Vector3(width / 2, 0, length / 2), 1, Vector3.right, _freeAreaElement, _freeAreaBlock);
            }
            _levelIsCreated = true;
            await UniTask.CompletedTask;
        }

        private async UniTask CreateLine(Vector3 startPosition, int count, Vector3 direction, MeshRenderer areaElement, MaterialPropertyBlock block)
        {
            for (int i = 0; i < count; i++)
            {
                var position = startPosition + i * direction + OffsetYForElement;
                var element = Instantiate(areaElement, position + Vector3.down * 5, Quaternion.identity, transform);
                element.SetPropertyBlock(block);
                LMotion.Create(element.transform.position, position, 1.5f).WithEase(Ease.OutElastic).BindToPosition(element.transform);
                await UniTask.WaitForSeconds(0.1f);
            }
        }

        public Vector3 GetStartWorldPoint()
        {
            return new Vector3(1, 0, _threeRoadsLevelAreaCount + 1);
        }

        private async UniTask CreateThreeRoadsLevel(EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData> command, CancellationToken token)
        {
            _threeRoadsLevelAreaCount = command.EntityData.Areas.Count;
            await UniTask.CompletedTask;
        }
    }
}
