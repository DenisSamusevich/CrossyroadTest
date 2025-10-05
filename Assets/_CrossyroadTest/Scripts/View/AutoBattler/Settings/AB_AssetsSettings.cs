using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings
{
    public interface IAB_AssetsSettings
    {
        public Color LevelElementColor { get; }
        public ObjectPoolBase<MeshRenderer> LevelElement { get; }
        public Color PlayerColor { get; }
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior { get; }
        public ObjectPoolBase<EnemyBehaviorView> EnemyBehavior { get; }
    }

    [CreateAssetMenu(fileName = "AB_AssetsSettings", menuName = "GameSettings/AutoBattlerSettings/AB_AssetsSettings")]
    internal class AB_AssetsSettings : ScriptableObject, IAB_AssetsSettings, IStartable
    {
        private IObjectResolver _objectResolver;
        [SerializeField] private Color _levelElementColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _levelElement;
        [SerializeField] private Color _playerColor = Color.white;
        [SerializeField] private ComponentObjectPool<PlayerBehaviorView> _playerBehavior;
        [SerializeField] private ComponentObjectPool<EnemyBehaviorView> _enemyBehavior;
        private MaterialPropertyBlock _levelElementBlock;
        private MaterialPropertyBlock _playerBlock;

        [Inject]
        public void Initialize(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Start()
        {
            _playerBlock = GetMaterialPropertyBlock(PlayerColor);
            _levelElementBlock = GetMaterialPropertyBlock(LevelElementColor);
            _playerBehavior.SetOnCreateAction((p) =>
            {
                _objectResolver.Inject(p);
            });
            _levelElement.SetOnCreateAction((p) => p.SetPropertyBlock(_levelElementBlock));
            _enemyBehavior.SetOnCreateAction((p) =>
            {
                _objectResolver.Inject(p);
            });

            MaterialPropertyBlock GetMaterialPropertyBlock(Color color)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", color);
                return block;
            }
        }

        public Color LevelElementColor => _levelElementColor;
        public ObjectPoolBase<MeshRenderer> LevelElement => _levelElement;
        public Color PlayerColor => _playerColor;
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior => _playerBehavior;
        public ObjectPoolBase<EnemyBehaviorView> EnemyBehavior => _enemyBehavior;
    }
}
