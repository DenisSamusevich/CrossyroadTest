using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Model;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area
{
    using AEC_SimpleArea = AsyncEntityCharacteristic<(int areaPosition, int levelWidth, AreaType areaType), SimpleAreaData, IAreaData>;

    public class SimpleArea : AEC_SimpleArea.AsyncEntity
    {
        public SimpleArea(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
