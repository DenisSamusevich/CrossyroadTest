using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces
{
    public interface IPlayerBehavior : IAttackBehavior, IDefendBehavior
    {
        UniTask LevelUp();
        UniTask OfferWeapon(Weapon weapon);
        UniTask RestoreHealth();
        UniTask WinBattle();
    }
}
