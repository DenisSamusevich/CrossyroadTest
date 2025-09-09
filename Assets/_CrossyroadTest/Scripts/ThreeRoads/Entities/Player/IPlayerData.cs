using Assets._CrossyroadTest.Scripts.Common.Player;
using System;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player
{
    internal interface IPlayerData : IDisposable
    {
        public Vector2 Position { get; }
        public PlayerState State { get; }
    }
}
