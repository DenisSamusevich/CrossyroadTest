using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Model
{
    public class LevelData : ILevelData, IInitAsyncElement<IAB_LevelSettings>
    {
        public List<EnemyCharacteristics> Enemies { get; private set; } = new List<EnemyCharacteristics>();
        public IReadOnlyList<EnemyCharacteristics> LevelEnemies { get => Enemies; }

        public UniTask InitAsyncElement(IAB_LevelSettings settings)
        {
            for (int i = 0; i < settings.CountEnemy; i++)
            {
                Enemies.Add(settings.EnemyDictionary[settings.EnemyDictionary.Keys.ElementAt(Random.Range(0, settings.EnemyDictionary.Count))]);
            }
            return UniTask.CompletedTask;
        }
    }
}
