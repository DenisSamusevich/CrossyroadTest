using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level
{
    internal class Cell : ICellData
    {
        public Cell(Vector2Int position)
        {
            Position = position;
        }

        public Vector2Int Position { get; private set; }
    }
}
