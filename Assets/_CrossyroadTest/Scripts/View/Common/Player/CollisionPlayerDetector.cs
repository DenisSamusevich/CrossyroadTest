using MessagePipe;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.Common.Player
{
    internal class CollisionPlayerDetector : MonoBehaviour
    {
        private IPublisher<Collider> _onTriggerEnterPublisher;

        [Inject]
        public void Initialize(IPublisher<Collider> onTriggerEnterPublisher)
        {
            _onTriggerEnterPublisher = onTriggerEnterPublisher;
        }

        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEnterPublisher.Publish(other);
        }
    }
}
