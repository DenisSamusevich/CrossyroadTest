using Assets._CrossyroadTest.Scripts.Common.Player;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.Common
{
    internal class CollisionPlayerDetector : MonoBehaviour
    {
        private IPublisher<CollisionPlayerMessage> _collisionPublisher;
        private IPublisher<ButtonColor, CollisionPlayerMessage> _collisionButtonPublisher;

        [Inject]
        public void Initialize(IPublisher<CollisionPlayerMessage> collisionPublisher,
             IPublisher<ButtonColor, CollisionPlayerMessage> collisionButtonPublisher)
        {
            _collisionPublisher = collisionPublisher;
            _collisionButtonPublisher = collisionButtonPublisher;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                _collisionPublisher.Publish(new CollisionPlayerMessage(TypeCollision.PlayerObstacle));
            }
            else if (other.CompareTag("FinalArea"))
            {
                _collisionPublisher.Publish(new CollisionPlayerMessage(TypeCollision.PlayerFinalArea));
            }
            else if (other.CompareTag("BlueButton"))
            {
                _collisionButtonPublisher.Publish(ButtonColor.Blue, new CollisionPlayerMessage(TypeCollision.PlayerButton));
            }
            else if (other.CompareTag("RedButton"))
            {
                _collisionButtonPublisher.Publish(ButtonColor.Red, new CollisionPlayerMessage(TypeCollision.PlayerButton));
            }
        }
    }
}
