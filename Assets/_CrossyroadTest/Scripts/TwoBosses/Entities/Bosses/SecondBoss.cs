using Assets._CrossyroadTest.Scripts.Common.Player;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses
{
    internal class SecondBoss : BossBase<SecondBoss>, ISecondBossData
    {
        private Vector2Int _bossPosition;

        public SecondBoss(IAsyncPublisher<SetStateCommand<SecondBoss>> setStateHandler,
            IAsyncPublisher<CreatingObstacleCommand<SecondBoss>> creatingObstacleHandler,
            IAsyncPublisher<CreateDamageButtonCommand<SecondBoss>> createDamageButtonHandler,
            ISubscriber<ButtonColor, CollisionPlayerMessage> collisionMessageSubscriber)
            : base(setStateHandler, creatingObstacleHandler, createDamageButtonHandler, collisionMessageSubscriber)
        {
        }
        public Vector2Int BossPosition => _bossPosition;
        public bool IsDefeated => _isDefeated;

        protected override UniTask InitAsync(CancellationToken cancellationToken)
        {
            _buttonColor = ButtonColor.Blue;
            _bossPosition = new Vector2Int(_cells.Width, _cells.Length);
            _bossStates = new List<StateBoss>()
            {
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreateDamageButton,
            };
            _health = 3;
            _countObstacle = 2;
            return UniTask.CompletedTask;
        }
    }
}
