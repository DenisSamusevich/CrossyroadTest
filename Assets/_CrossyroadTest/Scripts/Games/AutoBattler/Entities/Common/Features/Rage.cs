using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class Rage : IAttackFeatures
    {
        public int ModifyAttack(int damage, IEntityModel<ICharacterData, IAttackBehavior> attacker, int countTurn)
        {
            return countTurn < 3 ? damage + 2 : damage - 1;

        }
    }
}
