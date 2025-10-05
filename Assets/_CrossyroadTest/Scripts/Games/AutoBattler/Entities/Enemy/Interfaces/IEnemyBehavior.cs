using Cysharp.Threading.Tasks;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces
{
    public interface IEnemyBehavior : IAttackBehavior, IDefendBehavior
    {
        UniTask ActivateForBattle();
    }
}
