using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings
{

    [CreateAssetMenu(fileName = "TB_LevelSettings", menuName = "GameSettings/TwoBossesSettings/TB_LevelSettings")]
    internal class TB_LevelSettings : ScriptableObject, ITB_LevelSettings
    {
        public Vector2 WorldSize => LevelSize;
        [field: SerializeField] public bool IsDestroyAfterGame { get; private set; }
        [field: SerializeField] public bool IsUsedRepeatedly { get; private set; }
        [field: SerializeField] public Vector2Int LevelSize { get; private set; } = new Vector2Int(12, 11);
    }
}
