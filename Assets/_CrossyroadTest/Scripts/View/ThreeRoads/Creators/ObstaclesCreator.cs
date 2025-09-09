using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Obstacle;
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
    using EC_Obstacles = EntityCreator<int, IReadOnlyList<IObstacleData>>;

    internal class ObstaclesCreator : MonoBehaviour
    {
        [SerializeField] private LevelCreator _levelCreator;
        [SerializeField] private GameObject _obstacle;
        [SerializeField] private Color _obstacleColor;
        private IAsyncSubscriber<EC_Obstacles.CreateEntityCommand<IReadOnlyList<IObstacleData>>> _createObstaclesSubscriber;
        private ISubscriber<EC_Obstacles.DisposeEntityCommand<IReadOnlyList<IObstacleData>>> _disposeObstaclesSubscriber;
        private List<GameObject> _obstacleGameObjects = new List<GameObject>();
        private List<GameObject> _obstacleReserve = new List<GameObject>();
        private List<IObstacleData> _obstacles = new List<IObstacleData>();
        private IDisposable _disposable;
        private MaterialPropertyBlock _materialPropertyBlock;
        [Inject]
        public void Initialize(IAsyncSubscriber<EC_Obstacles.CreateEntityCommand<IReadOnlyList<IObstacleData>>> createObstaclesSubscriber,
            ISubscriber<EC_Obstacles.DisposeEntityCommand<IReadOnlyList<IObstacleData>>> disposeObstaclesSubscriber)
        {
            _createObstaclesSubscriber = createObstaclesSubscriber;
            _disposeObstaclesSubscriber = disposeObstaclesSubscriber;
            var bag = DisposableBag.CreateBuilder();
            _createObstaclesSubscriber.Subscribe(CreateObstacles).AddTo(bag);
            _disposeObstaclesSubscriber.Subscribe(DestroyObstacles).AddTo(bag);
            _disposable = bag.Build();
            _materialPropertyBlock = new MaterialPropertyBlock();
            _materialPropertyBlock.SetColor("_BaseColor", _obstacleColor);
        }

        private void DestroyObstacles(EC_Obstacles.DisposeEntityCommand<IReadOnlyList<IObstacleData>> command)
        {
            foreach (var obstacle in _obstacleGameObjects)
            {
                obstacle.SetActive(false);
                _obstacleReserve.Add(obstacle);
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
                _obstacleGameObjects[i].transform.position = new Vector3(_obstacles[i].Position * _levelCreator.LevelWidth, 0, _obstacles[i].Road);
            }
        }

        private async UniTask CreateObstacles(EC_Obstacles.CreateEntityCommand<IReadOnlyList<IObstacleData>> command, CancellationToken token)
        {
            _obstacles.AddRange(command.EntityData);
            for (int i = 0; i < command.EntityData.Count; i++)
            {
                var position = new Vector3(command.EntityData[i].Position * _levelCreator.LevelWidth, 0, command.EntityData[i].Road);
                GameObject obstacle;
                if (_obstacleReserve.Count > 0)
                {
                    obstacle = _obstacleReserve[^1];
                    _obstacleReserve.RemoveAt(_obstacleReserve.Count - 1);
                    obstacle.transform.position = position + Vector3.up * 5;
                    obstacle.SetActive(true);
                }
                else
                {
                    obstacle = Instantiate(_obstacle, position + Vector3.up * 5, Quaternion.identity, transform);
                    obstacle.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                }
                LMotion.Create(obstacle.transform.position, position, 1.2f)
                    .WithEase(Ease.OutBounce)
                    .WithOnComplete(() => _obstacleGameObjects.Add(obstacle))
                    .BindToPosition(obstacle.transform).ToUniTask().Forget();
                await UniTask.WaitForSeconds(0.2f);
            }
        }
    }
}
