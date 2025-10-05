
using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases
{
    internal interface ITB_LevelSettings : IWorldSetting
    {
        public Vector2Int LevelSize { get; }
    }
}
