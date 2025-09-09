using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Assets._CrossyroadTest.Scripts.View.ThreeRoads.PlayerView;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using static Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera.CameraService;

namespace Assets._CrossyroadTest.Scripts.View.Common
{
    using EC_ThreeRoadsLevel = EntityCreator<IEnumerable<AreaType>, Scripts.ThreeRoads.Entities.Level.ILevelData>;
    using EC_TwoBossLevel = EntityCreator<Vector2Int, Scripts.TwoBosses.Entities.Level.ILevelData>;
    using TB_Level = Scripts.TwoBosses.Entities.Level;
    using TR_Level = Scripts.ThreeRoads.Entities.Level;

    internal class CameraManagerView : MonoBehaviour
    {
        private IAsyncSubscriber<EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData>> _createThreeRoadsLevelSubscriber;
        private IAsyncSubscriber<EC_TwoBossLevel.CreateEntityCommand<TB_Level.ILevelData>> _createTwoBossLevelSubscriber;
        private ISubscriber<SetCameraTargetCommand> _setCameraTargetSubscriber;
        private PlayerViewContainer _playerViewContainer;
        [SerializeField] private CinemachineCamera _threeRoadsLevelCamera;
        [SerializeField] private CinemachineCamera _threeRoadsPlayerCamera;
        [SerializeField] private CinemachineCamera _twoBossLevelCamera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _lb;
        [SerializeField] private Transform _rt;
        [SerializeField] private CinemachineCamera _twoBossPlayerCamera;
        private List<CinemachineCamera> _cameras = new List<CinemachineCamera>();
        private int _threeRoadsLevelAreaCount;
        private MotionHandle _cameraAnimaction;
        private IDisposable _dispose;

        [Inject]
        public void Initialize(IAsyncSubscriber<EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData>> createThreeRoadsLevelSubscriber,
            IAsyncSubscriber<EC_TwoBossLevel.CreateEntityCommand<TB_Level.ILevelData>> createTwoBossLevelSubscriber,
            ISubscriber<SetCameraTargetCommand> setCameraTargetSubscriber,
            PlayerViewContainer playerViewContainer)
        {
            _setCameraTargetSubscriber = setCameraTargetSubscriber;
            _createThreeRoadsLevelSubscriber = createThreeRoadsLevelSubscriber;
            _createTwoBossLevelSubscriber = createTwoBossLevelSubscriber;
            _playerViewContainer = playerViewContainer;
            var bag = DisposableBag.CreateBuilder();
            _setCameraTargetSubscriber.Subscribe(SetTargetCamera).AddTo(bag);
            _createThreeRoadsLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _createTwoBossLevelSubscriber.Subscribe(CreateLevel).AddTo(bag);
            _dispose = bag.Build();
            _cameras.Add(_threeRoadsLevelCamera);
            _cameras.Add(_threeRoadsPlayerCamera);
            _cameras.Add(_twoBossLevelCamera);
            _cameras.Add(_twoBossPlayerCamera);
        }

        private void OnDestroy()
        {
            _dispose?.Dispose();
        }

        public void EndAnimation()
        {
            if (_cameraAnimaction.IsPlaying())
            {
                _cameraAnimaction.Cancel();
            }
        }

        public void SetTargetCamera(SetCameraTargetCommand command)
        {
            foreach (var camera in _cameras)
            {
                camera.Priority = 0;
            }
            switch (command.Target)
            {
                case CameraTargetType.ThreeRoadsLevel:
                    SetCamera(_threeRoadsLevelCamera);
                    _cameraAnimaction = LMotion.Create(0f, 360f, 6).WithLoops(-1).BindToEulerAnglesY(_threeRoadsLevelCamera.transform);
                    break;
                case CameraTargetType.ThreeRoadsPlayer:
                    _threeRoadsPlayerCamera.Target = new CameraTarget() { TrackingTarget = _playerViewContainer.GetPlayerView<PlayerView>().transform };
                    SetCamera(_threeRoadsPlayerCamera);
                    break;
                case CameraTargetType.TwoBossLevel:
                    SetCamera(_twoBossLevelCamera);
                    _cameraAnimaction = LMotion.Create(0f, 360f, 6).WithLoops(-1).BindToEulerAnglesY(_twoBossLevelCamera.transform);
                    break;
                case CameraTargetType.TwoBossPlayer:
                    SetCamera(_twoBossPlayerCamera);
                    break;
                default:
                    throw new NotImplementedException();
            }

            void SetCamera(CinemachineCamera camera)
            {
                camera.Priority = 1;
                camera.Prioritize();
            }
        }

        private async UniTask CreateLevel(EC_ThreeRoadsLevel.CreateEntityCommand<TR_Level.ILevelData> command, CancellationToken token)
        {
            _threeRoadsLevelAreaCount = command.EntityData.Areas.Count;
            await UniTask.CompletedTask;
        }

        private async UniTask CreateLevel(EC_TwoBossLevel.CreateEntityCommand<TB_Level.ILevelData> command, CancellationToken token)
        {
            _cameraTarget.position = new Vector3(command.EntityData.Width * 0.5f, 0, _threeRoadsLevelAreaCount + command.EntityData.Length * 0.5f);
            _rt.position = new Vector3(0, 0, _threeRoadsLevelAreaCount + command.EntityData.Length + 1);
            _lb.position = new Vector3(command.EntityData.Width + 1, 0, 0);
            await UniTask.CompletedTask;
        }
    }
}
