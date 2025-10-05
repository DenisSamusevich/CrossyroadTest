using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model
{
    internal class FirstBossBehavior : BossBehaviorBase<BossId>
    {
        public FirstBossBehavior(IAsyncPublisher<BossId, SetStateCommand> setStateHandler,
            IAsyncPublisher<BossId, CreatingObstacleCommand> creatingObstacleHandler,
            IAsyncPublisher<BossId, CreateDamageButtonCommand> createDamageButtonHandler,
            ISubscriber<BossId, CollisionPlayerMessage> collisionMessageSubscriber)
            : base(setStateHandler, creatingObstacleHandler, createDamageButtonHandler, collisionMessageSubscriber)
        {
        }

        protected override UniTask InitAsync()
        {
            _bossData.BossPosition = new Vector2Int(-1, _levelData.Length);
            _bossStates = new List<StateBoss>()
            {
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreatingObstacle,
                StateBoss.CreateDamageButton,
            };
            _countObstacle = 6;
            return UniTask.CompletedTask;
        }
    }
}
