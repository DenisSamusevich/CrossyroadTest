using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces
{
    public interface IPlayerData
    {
        public Vector2Int Position { get; }
        public PlayerState State { get; }
    }
}
