using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces
{
    public interface IWeapon
    {
        public DamageType DamageType { get; }
        public int Damage { get; }
    }
}
