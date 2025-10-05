using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class BoneForm : IDefendAttackFeatures
    {
        public int ModifyProtection(int damage,
            IEntityModel<ICharacterData, IDefendBehavior> defender,
            IEntityModel<ICharacterData, IAttackBehavior> attacker,
            int countTurn)
        {
            if (attacker.EntityData.Weapon.DamageType == DamageType.Crushing)
            {
                damage += attacker.EntityData.Weapon.Damage;
            }
            return damage;
        }
    }
}
