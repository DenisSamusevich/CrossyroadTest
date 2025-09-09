using MessagePipe;

namespace Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera
{
    public interface ICameraService
    {
        void SetFocus(CameraTargetType cameraTarget);
    }

    public class CameraService : ICameraService
    {
        public struct SetCameraTargetCommand
        {
            public SetCameraTargetCommand(CameraTargetType target)
            {
                Target = target;
            }

            public CameraTargetType Target { get; }
        }

        private readonly IPublisher<SetCameraTargetCommand> _setCameraTargetPublisher;

        public CameraService(IPublisher<SetCameraTargetCommand> setCameraTargetPublisher)
        {
            _setCameraTargetPublisher = setCameraTargetPublisher;
        }

        public void SetFocus(CameraTargetType cameraTarget)
        {
            _setCameraTargetPublisher.Publish(new SetCameraTargetCommand(cameraTarget));
        }
    }
}
