using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level
{
    using AEC_Level = AsyncEntityCharacteristic<ITB_LevelSettings, LevelData, ILevelData>;

    internal class Level : AEC_Level.AsyncEntity
    {
        public Level(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
