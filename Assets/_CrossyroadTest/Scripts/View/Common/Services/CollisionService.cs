using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using MessagePipe;
using System;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services
{
    public class CollisionService : ITR_GameService, ITB_GameService
    {
        private IPublisher<CollisionPlayerMessage> _collisionPublisher;
        private IPublisher<BossId, CollisionPlayerMessage> _collisionButtonPublisher;
        private ISubscriber<Collider> _onTriggerEnterSubscriber;
        private IDisposable _disposable;

        public CollisionService(IPublisher<CollisionPlayerMessage> collisionPublisher,
             IPublisher<BossId, CollisionPlayerMessage> collisionButtonPublisher,
             ISubscriber<Collider> onTriggerEnterSubscriber)
        {
            _collisionPublisher = collisionPublisher;
            _collisionButtonPublisher = collisionButtonPublisher;
            _onTriggerEnterSubscriber = onTriggerEnterSubscriber;
        }

        void IGameService.EnableService()
        {
            _disposable = _onTriggerEnterSubscriber.Subscribe(OnTriggerEnter);
        }

        void IGameService.DisableService()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ObstaclesContainer"))
            {
                _collisionPublisher.Publish(new CollisionPlayerMessage(CollisionType.PlayerObstacle));
            }
            else if (other.CompareTag("FinalArea"))
            {
                _collisionPublisher.Publish(new CollisionPlayerMessage(CollisionType.PlayerFinalArea));
            }
            else if (other.CompareTag("BlueButton"))
            {
                _collisionButtonPublisher.Publish(BossId.BlueBoss, new CollisionPlayerMessage(CollisionType.PlayerButton));
            }
            else if (other.CompareTag("RedButton"))
            {
                _collisionButtonPublisher.Publish(BossId.RedBoss, new CollisionPlayerMessage(CollisionType.PlayerButton));
            }
        }
    }
}
