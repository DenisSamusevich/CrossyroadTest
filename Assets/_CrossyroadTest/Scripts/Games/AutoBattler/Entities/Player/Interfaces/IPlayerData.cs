using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces
{
    public interface IPlayerData : ICharacterData, IPersonClass
    {
        bool IsDefeated { get; }
        PlayerState State { get; }
    }
}
