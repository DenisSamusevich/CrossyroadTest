using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features
{
    public class FeaturesFactory
    {

        public FeaturesFactory() { }

        public IFeatures GetFeatures(Ability ability)
        {
            switch (ability)
            {
                case Ability.BoneForm:
                    return new BoneForm();
                case Ability.GelForm:
                    return new GelForm();
                case Ability.DragonAttack:
                    return new DragonAttack();
                case Ability.HidenAttack:
                    return new HiddenAttack();
                case Ability.ImpulsesToAction:
                    return new ImpulsesToAction();
                case Ability.PoisonAttack:
                    return new PoisonAttack();
                case Ability.Rage:
                    return new Rage();
                case Ability.Shield:
                    return new Shield();
                case Ability.StoneSkin:
                    return new StoneSkin();
                default:
                    return null;
            }
        }
    }
}
