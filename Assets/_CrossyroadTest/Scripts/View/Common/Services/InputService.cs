using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services
{
    public class InputService : ITR_GameService, ITB_GameService
    {
        public struct InputCommand<T>
        {
            public InputCommand(T input)
            {
                Input = input;
            }

            public T Input { get; }
        }

        private IPublisher<InputCommand<DirectionType>> _inputMovingPublisher;
        private InputSystem_Actions _actions;
        private Camera _mainCamera;

        public InputService(Camera mainCamera, IPublisher<InputCommand<DirectionType>> inputMovingPublisher)
        {
            _mainCamera = mainCamera;
            _inputMovingPublisher = inputMovingPublisher;
            _actions = new InputSystem_Actions();
        }

        public DirectionType CurrentInput { get; private set; }

        void IGameService.EnableService()
        {
            _actions.Enable();
            _actions.Touch.Enable();
            _actions.Touch.Tap.performed += OnTapPerformed;
        }

        void IGameService.DisableService()
        {
            _actions.Disable();
            _actions.Touch.Disable();
            _actions.Touch.Tap.performed -= OnTapPerformed;
        }

        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            var playerPosition = GetPlayerPosition();
            var touchScreenPosition = GetWorkdPoint(_actions.Touch.TapPosition.ReadValue<Vector2>());
            var currentInput = CalculateCurrentInput(playerPosition, touchScreenPosition);
            _inputMovingPublisher.Publish(new InputCommand<DirectionType>(currentInput));

            Vector3 GetPlayerPosition()
            {
                var forward = _mainCamera.transform.forward;
                var count = _mainCamera.transform.position.y / forward.y;
                var playerPosition = _mainCamera.transform.position - forward * count;
                return playerPosition;
            }

            Vector3 GetWorkdPoint(Vector3 screenPoint)
            {
                var ray = _mainCamera.ScreenPointToRay(screenPoint);
                var count = ray.origin.y / ray.direction.y;
                var worldPosition = ray.origin - ray.direction * count;
                return worldPosition;
            }

            DirectionType CalculateCurrentInput(Vector3 playerPosition, Vector3 touchScreenPosition)
            {
                var touchPosition = touchScreenPosition - playerPosition;
                var currentInput = DirectionType.None;
                if (Mathf.Abs(touchPosition.x) > Mathf.Abs(touchPosition.z))
                {
                    if (touchPosition.x > 0)
                    {
                        currentInput = DirectionType.Right;
                    }
                    else
                    {
                        currentInput = DirectionType.Left;
                    }
                }
                else
                {
                    if (touchPosition.z > 0)
                    {
                        currentInput = DirectionType.Forward;
                    }
                    else
                    {
                        currentInput = DirectionType.Back;
                    }
                }
                return currentInput;
            }
        }
    }
}
