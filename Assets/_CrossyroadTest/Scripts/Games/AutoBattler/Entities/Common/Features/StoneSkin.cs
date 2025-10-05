using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class StoneSkin : IDefendFeatures
    {
        public int ModifyProtection(int damage, IEntityModel<ICharacterData, IDefendBehavior> defender, int countTurn)
        {
            return damage - defender.EntityData.Endurance;
        }
    }
}
