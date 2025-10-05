using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model
{
    public class PlayerData : IInitAsyncElement<ILevelData>, IPlayerData
    {
        public Vector2Int Position { get; set; }
        public PlayerState State { get; set; }

        UniTask IInitAsyncElement<ILevelData>.InitAsyncElement(ILevelData levelData)
        {
            Position = new Vector2Int(levelData.Width / 2, levelData.Length / 2);
            State = PlayerState.WaitInput;
            return UniTask.CompletedTask;
        }
    }
}
