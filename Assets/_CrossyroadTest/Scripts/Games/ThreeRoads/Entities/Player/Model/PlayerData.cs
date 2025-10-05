using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model
{
    public class PlayerData : IInitAsyncElement<ITR_LevelSettings>, IPlayerData
    {
        public Vector2 Position { get; set; }
        public PlayerState State { get; set; }

        UniTask IInitAsyncElement<ITR_LevelSettings>.InitAsyncElement(ITR_LevelSettings threeRoadsSettings)
        {
            var startPosition = -1;
            foreach (var area in threeRoadsSettings.LevelAreas)
            {
                startPosition++;
                if (area == AreaType.Start) break;
            }
            Position = new Vector2(Random.Range(0, 10), startPosition);
            State = PlayerState.WaitInput;
            return UniTask.CompletedTask;
        }
    }
}
