using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces
{
    public interface IPersonClass
    {
        public CharacterClass CharacterClass { get; }
        public int ReaverLevel { get; }
        public int WarriorLevel { get; }
        public int BarbarianLevel { get; }

    }
}
