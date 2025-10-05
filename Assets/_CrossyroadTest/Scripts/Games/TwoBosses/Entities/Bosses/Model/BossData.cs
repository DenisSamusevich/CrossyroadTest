using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model
{
    public class BossData : IInitAsyncElement<ILevelData>, IBossData
    {
        public bool IsDefeated { get; set; }
        public int Health { get; set; }
        public Vector2Int BossPosition { get; set; }

        public UniTask InitAsyncElement(ILevelData data1)
        {
            IsDefeated = false;
            Health = 3;
            return UniTask.CompletedTask;
        }
    }
}
