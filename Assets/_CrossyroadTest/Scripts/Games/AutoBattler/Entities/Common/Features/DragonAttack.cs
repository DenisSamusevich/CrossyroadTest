using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    internal class DragonAttack : IAttackFeatures
    {
        public int ModifyAttack(int damage, Services.EntityService.Interfaces.IEntityModel<ICharacterData, IAttackBehavior> attacker, int countTurn)
        {
            return damage + 3;
        }
    }
}
