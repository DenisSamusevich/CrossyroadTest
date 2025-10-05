using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    internal class GelForm : IDefendAttackFeatures
    {
        public int ModifyProtection(int damage,
            IEntityModel<ICharacterData, IDefendBehavior> defender,
            IEntityModel<ICharacterData, IAttackBehavior> attacker,
            int countTurn)
        {
            if (attacker.EntityData.Weapon.DamageType == DamageType.Slashing)
            {
                damage -= attacker.EntityData.Weapon.Damage;
            }
            return damage;
        }
    }
}
