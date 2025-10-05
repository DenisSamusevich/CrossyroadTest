using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class HiddenAttack : IAttackDefendFeatures
    {
        public int ModifyAttack(int damage, IEntityModel<ICharacterData, IAttackBehavior> attacker, IEntityModel<ICharacterData, IDefendBehavior> defender, int countTurn)
        {
            return attacker.EntityData.Dexterity > defender.EntityData.Dexterity ? damage + 1 : damage;
        }
    }
}
