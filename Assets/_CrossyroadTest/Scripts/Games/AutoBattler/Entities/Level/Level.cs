using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level
{
    using AEC_Level = AsyncEntityCharacteristic<IAB_LevelSettings, LevelData, ILevelData>;
    internal class Level : AEC_Level.AsyncEntity
    {
        public Level(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
