using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
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
    using CreateObstaclesCommand = AsyncEntityService<IAreaData, ObstaclesContainer, IReadOnlyList<IObstacleData>, IEntityModel<IReadOnlyList<IObstacleData>, IObstaclesBehavior>>.CreateEntityCommand;
    using DisposeObstaclesCommand = AsyncEntityService<IAreaData, ObstaclesContainer, IReadOnlyList<IObstacleData>, IEntityModel<IReadOnlyList<IObstacleData>, IObstaclesBehavior>>.DisposeEntityCommand;

    internal class ObstaclesView : MonoBehaviour
    {
        private IAsyncSubscriber<CreateObstaclesCommand> _createObstaclesSubscriber;
        private ISubscriber<DisposeObstaclesCommand> _disposeObstaclesSubscriber;
        private IWorldObjectService _worldObjectService;
        private ITR_AssetsSettings _assetsSettings;
        private List<GameObject> _obstacleGameObjects = new List<GameObject>();
        private List<IObstacleData> _obstacles = new List<IObstacleData>();
        private IDisposable _disposable;

        [Inject]
        public void Initialize(IAsyncSubscriber<CreateObstaclesCommand> createObstaclesSubscriber,
            ISubscriber<DisposeObstaclesCommand> disposeObstaclesSubscriber,
            IWorldObjectService worldObjectService,
            ITR_AssetsSettings assetsSettings)
        {
            _createObstaclesSubscriber = createObstaclesSubscriber;
            _disposeObstaclesSubscriber = disposeObstaclesSubscriber;
            _worldObjectService = worldObjectService;
            _assetsSettings = assetsSettings;
            var bag = DisposableBag.CreateBuilder();
            _createObstaclesSubscriber.Subscribe(CreateObstacles).AddTo(bag);
            _disposeObstaclesSubscriber.Subscribe(DestroyObstacles).AddTo(bag);
            _disposable = bag.Build();
        }

        private void DestroyObstacles(DisposeObstaclesCommand command)
        {
            foreach (var obstacle in _obstacleGameObjects)
            {
                _worldObjectService.ReturnGameObject(_assetsSettings.Obstacle, obstacle);
            }
            _obstacleGameObjects.Clear();
            _obstacles.Clear();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void Update()
        {
            for (int i = 0; i < _obstacleGameObjects.Count; i++)
            {
                _obstacleGameObjects[i].transform.position = new Vector3(_obstacles[i].Position, 0, _obstacles[i].AreaPosition);
            }
        }

        private async UniTask CreateObstacles(CreateObstaclesCommand command, CancellationToken token)
        {
            _obstacles.AddRange(command.EntityModel.EntityData);
            for (int i = 0; i < command.EntityModel.EntityData.Count; i++)
            {
                var position = new Vector3(command.EntityModel.EntityData[i].Position, 0, command.EntityModel.EntityData[i].AreaPosition);
                var obstacle = _worldObjectService.GetGameObject(_assetsSettings.Obstacle, position + Vector3.up * 5, Quaternion.identity, transform);

                LMotion.Create(obstacle.transform.position, position, 1.2f)
                    .WithEase(Ease.OutBounce)
                    .WithOnComplete(() => _obstacleGameObjects.Add(obstacle))
                    .BindToPosition(obstacle.transform).ToUniTask().Forget();
                await UniTask.WaitForSeconds(0.2f);
            }
        }
    }
}
