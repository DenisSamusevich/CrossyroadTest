using Assets._CrossyroadTest.Scripts.Common.Player;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses
{
    internal class FirstBoss : BossBase<FirstBoss>, IFirstBossData
    {
        private Vector2Int _bossPosition;

        public FirstBoss(IAsyncPublisher<SetStateCommand<FirstBoss>> setStateHandler,
            IAsyncPublisher<CreatingObstacleCommand<FirstBoss>> creatingObstacleHandler,
            IAsyncPublisher<CreateDamageButtonCommand<FirstBoss>> createDamageButtonHandler,
            ISubscriber<ButtonColor, CollisionPlayerMessage> collisionMessageSubscriber)
            : base(setStateHandler, creatingObstacleHandler, createDamageButtonHandler, collisionMessageSubscriber)
        {
        }

        public Vector2Int BossPosition => _bossPosition;
        public bool IsDefeated => _isDefeated;

        protected override UniTask InitAsync(CancellationToken cancellationToken)
        {
            _buttonColor = ButtonColor.Red;
            _bossPosition = new Vector2Int(-1, _cells.Length);
            _bossStates = new List<StateBoss>()
            {
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreateDamageButton,
            };
            _health = 3;
            _countObstacle = 6;
            return UniTask.CompletedTask;
        }
    }
}
