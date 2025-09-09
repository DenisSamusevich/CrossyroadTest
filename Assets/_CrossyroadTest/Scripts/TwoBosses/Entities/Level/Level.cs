using Assets._CrossyroadTest.Scripts.Common.Creators;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level
{
    internal class Level : ILevelData, IEntityInit<Vector2Int>
    {
        private IReadOnlyList<ICellData> _cells;
        private int _width;
        private int _length;

        public int Width => _width;
        public int Length => _length;
        public IReadOnlyList<ICellData> GetAllCells => _cells;
        public ICellData GetCell(Vector2Int cellPosition) => _cells[cellPosition.x + cellPosition.y * Width];

        public async UniTask InitAsync(Vector2Int levelSettings, CancellationToken cancellationToken)
        {
            _width = levelSettings.x;
            _length = levelSettings.y;
            var cells = new List<ICellData>();
            for (int i = 0; i < levelSettings.y; i++)
            {
                for (int j = 0; j < levelSettings.x; j++)
                {
                    cells.Add(new Cell(new Vector2Int(j, i)));
                }
            }
            _cells = cells;
            await UniTask.CompletedTask;
        }
    }
}
