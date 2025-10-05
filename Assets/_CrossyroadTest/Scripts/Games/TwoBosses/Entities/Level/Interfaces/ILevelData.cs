using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces
{
    public interface ILevelData
    {
        int Width { get; }
        int Length { get; }
        IReadOnlyList<Vector2Int> AllCellPositions { get; }
    }
}
