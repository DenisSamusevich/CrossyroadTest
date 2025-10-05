using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces
{
    public interface IAttackFeatures : IFeatures
    {
        int ModifyAttack(int damage, IEntityModel<ICharacterData, IAttackBehavior> attacker, int countTurn);
    }
}
