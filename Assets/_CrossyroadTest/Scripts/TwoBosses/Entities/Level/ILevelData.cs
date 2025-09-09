using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level
{
    internal interface ILevelData
    {
        int Width { get; }
        int Length { get; }
        IReadOnlyList<ICellData> GetAllCells { get; }
        ICellData GetCell(Vector2Int cellPosition);
    }
}
