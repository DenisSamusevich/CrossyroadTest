using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces
{
    public interface IPlayerData
    {
        public Vector2 Position { get; }
        public PlayerState State { get; }
    }
}
