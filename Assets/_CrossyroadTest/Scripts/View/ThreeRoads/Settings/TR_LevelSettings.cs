using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings
{
    [CreateAssetMenu(fileName = "TR_LevelSettings", menuName = "GameSettings/ThreeRoadsSettings/TR_LevelSettings")]
    internal class TR_LevelSettings : ScriptableObject, ITR_LevelSettings
    {
        public Vector2 WorldSize => new Vector2(LevelWidth, LevelAreas.Count);
        [field: SerializeField] public bool IsDestroyAfterGame { get; private set; }
        [field: SerializeField] public bool IsUsedRepeatedly { get; private set; }

        [SerializeField]
        private List<AreaType> _levelAreas = new List<AreaType>() { AreaType.Start, AreaType.Free, AreaType.Road, AreaType.Free,
            AreaType.Road, AreaType.Free, AreaType.Free, AreaType.Road, AreaType.Final, };
        public IReadOnlyList<AreaType> LevelAreas { get => _levelAreas; }

        [field: SerializeField] public int LevelWidth { get; private set; }
    }
}
