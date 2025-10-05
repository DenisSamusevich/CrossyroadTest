using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces
{
    public interface IDefendFeatures : IFeatures
    {
        int ModifyProtection(int damage, IEntityModel<ICharacterData, IDefendBehavior> defender, int countTurn);
    }
}
