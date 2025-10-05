using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class Poison : IDefendFeatures
    {
        private readonly int _poisonDamage;
        public Poison(int poisonDamage)
        {
            _poisonDamage = poisonDamage;
        }

        public int ModifyProtection(int damage, IEntityModel<ICharacterData, IDefendBehavior> defender, int countTurn)
        {
            return damage + _poisonDamage;
        }
    }
}
