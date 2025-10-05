using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class ImpulsesToAction : IAttackFeatures
    {
        public int ModifyAttack(int damage, IEntityModel<ICharacterData, IAttackBehavior> attacker, int countTurn)
        {
            return countTurn == 0 ? damage += attacker.EntityData.Weapon.Damage : damage;
        }
    }
}
