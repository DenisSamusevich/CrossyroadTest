using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class Shield : IDefendAttackFeatures
    {
        public int ModifyProtection(int damage, IEntityModel<ICharacterData, IDefendBehavior> defender, IEntityModel<ICharacterData, IAttackBehavior> attacker, int countTurn)
        {
            return defender.EntityData.Strength > attacker.EntityData.Strength ? damage - 3 : damage;
        }
    }
}
