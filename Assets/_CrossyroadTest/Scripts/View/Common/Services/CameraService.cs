using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
namespace Assets._CrossyroadTest.Scripts.View.Common.Services
{
    internal class CameraService : MonoBehaviour, ICameraService
    {
        private IWorldObjectService _worldObjectService;
        [SerializeField] private CinemachineCamera _threeRoadsLevelCamera;
        [SerializeField] private CinemachineCamera _threeRoadsPlayerCamera;
        [SerializeField] private CinemachineCamera _twoBossLevelCamera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _lb;
        [SerializeField] private Transform _rt;
        [SerializeField] private CinemachineCamera _twoBossPlayerCamera;
        [SerializeField] private Transform _lb_autoBattler;
        [SerializeField] private Transform _rt_autoBattler;
        [SerializeField] private CinemachineCamera _autoBattlerPlayerCamera;
        private List<CinemachineCamera> _cameras = new List<CinemachineCamera>();
        private MotionHandle _cameraAnimation;

        [Inject]
        public void Initialize(IWorldObjectService worldObjectService)
        {
            _worldObjectService = worldObjectService;
            _cameras.Add(_threeRoadsLevelCamera);
            _cameras.Add(_threeRoadsPlayerCamera);
            _cameras.Add(_twoBossLevelCamera);
            _cameras.Add(_twoBossPlayerCamera);
        }

        private void OnDestroy()
        {
            _cameras.Clear();
        }

        public void EndAnimation()
        {
            if (_cameraAnimation.IsPlaying())
            {
                _cameraAnimation.Cancel();
            }
        }

        public void SetFocus(CameraTargetType cameraTarget)
        {
            foreach (var camera in _cameras)
            {
                camera.Priority = 0;
            }
            switch (cameraTarget)
            {
                case CameraTargetType.ThreeRoadsLevel:
                    SetCamera(_threeRoadsLevelCamera);
                    _cameraAnimation = LMotion.Create(0f, 360f, 6).WithLoops(-1).BindToEulerAnglesY(_threeRoadsLevelCamera.transform);
                    break;
                case CameraTargetType.ThreeRoadsPlayer:
                    _threeRoadsPlayerCamera.Target = new CameraTarget() { TrackingTarget = _worldObjectService.CurrentPlayer.transform };
                    SetCamera(_threeRoadsPlayerCamera);
                    break;
                case CameraTargetType.TwoBossLevel:
                    SetCamera(_twoBossLevelCamera);
                    _cameraAnimation = LMotion.Create(0f, 360f, 6).WithLoops(-1).BindToEulerAnglesY(_twoBossLevelCamera.transform);
                    break;
                case CameraTargetType.TwoBossPlayer:
                    SetCamera(_twoBossPlayerCamera);
                    break;
                case CameraTargetType.AutoBattlerLevel:
                case CameraTargetType.AutoBattlerPlayer:
                    SetCamera(_autoBattlerPlayerCamera);
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
    }
}
