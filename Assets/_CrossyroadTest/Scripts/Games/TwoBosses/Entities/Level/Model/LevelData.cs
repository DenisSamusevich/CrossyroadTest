using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Model
{
    internal class LevelData : IInitAsyncElement<ITB_LevelSettings>, ILevelData
    {
        private List<Vector2Int> _cellPositions;
        private int _width;
        private int _length;

        public int Width => _width;
        public int Length => _length;
        public IReadOnlyList<Vector2Int> AllCellPositions => _cellPositions;

        public UniTask InitAsyncElement(ITB_LevelSettings levelSettings)
        {
            _width = levelSettings.LevelSize.x;
            _length = levelSettings.LevelSize.y;
            var cells = new List<Vector2Int>();
            for (int y = 0; y < levelSettings.LevelSize.y; y++)
            {
                for (int x = 0; x < levelSettings.LevelSize.x; x++)
                {
                    cells.Add(new Vector2Int(x, y));
                }
            }
            _cellPositions = cells;
            return UniTask.CompletedTask;
        }
    }
}
