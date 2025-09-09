using Assets._CrossyroadTest.Scripts.Common.Player;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Player
{
    internal interface IPlayerData
    {
        public Vector2Int Position { get; }
        public PlayerState State { get; }
    }
}
