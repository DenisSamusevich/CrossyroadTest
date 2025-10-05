using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level
{
    using AEC_Level = AsyncEntityCharacteristic<ITR_LevelSettings, LevelData, ILevelData, LevelBehavior, ILevelBehavior>;

    public class Level : AEC_Level.AsyncEntity, IEntityModel<ILevelData, ILevelBehavior>
    {
        public Level(AsyncEntityFactory entityFactor) : base(entityFactor)
        {

        }
    }
}
