using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings
{
    public interface ITB_AssetsSettings
    {
        public Color PlayerColor { get; }
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior { get; }
        public Color FirstBossColor { get; }
        public UniGameObjectPoolBase FirstBossObstacle { get; }
        public ObjectPoolBase<FirstBossView> FirstBoss { get; }
        public UniGameObjectPoolBase FirstBossButton { get; }
        public Color SecondBossColor { get; }
        public ObjectPoolBase<SecondBossView> SecondBoss { get; }
        public UniGameObjectPoolBase SecondBossObstacle { get; }
        public UniGameObjectPoolBase SecondBossWall { get; }
        public UniGameObjectPoolBase SecondBossButton { get; }
        public Color WallElementColor { get; }
        public ObjectPoolBase<MeshRenderer> WallElement { get; }
        public Color FreeElementColor { get; }
        public ObjectPoolBase<MeshRenderer> FreeLevelElement { get; }
    }





    [CreateAssetMenu(fileName = "TB_AssetsSettings", menuName = "GameSettings/TwoBossesSettings/TB_AssetsSettings")]
    public class TB_AssetsSettings : ScriptableObject, ITB_AssetsSettings, IStartable
    {
        private IObjectResolver _objectResolver;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _playerColor = Color.white;
        [SerializeField] private ComponentObjectPool<PlayerBehaviorView> _playerBehaviorPrefab;
        [SerializeField] private Color _firstBossColor = Color.white;
        [SerializeField] private UniGameObjectPool _firstBossObstacle;
        [SerializeField] private ComponentObjectPool<FirstBossView> _firstBoss;
        [SerializeField] private UniGameObjectPool _firstBossButton;
        [SerializeField] private Color _secondBossColor = Color.white;
        [SerializeField] private ComponentObjectPool<SecondBossView> _secondBoss;
        [SerializeField] private UniGameObjectPool _secondBossObstacle;
        [SerializeField] private UniGameObjectPool _secondBossWall;
        [SerializeField] private UniGameObjectPool _secondBossButton;
        [SerializeField] private Color _wallElementColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _wallElement;
        [SerializeField] private Color _freeElementColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _freeLevelElement;
        private MaterialPropertyBlock _defaultPropertyBlock;
        private MaterialPropertyBlock _playerPropertyBlock;
        private MaterialPropertyBlock _firstBossPropertyBlock;
        private MaterialPropertyBlock _secondBossPropertyBlock;

        [Inject]
        public void Initialize(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public Color DefaultColor { get => _defaultColor; }
        public Color PlayerColor { get => _playerColor; }
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior { get => _playerBehaviorPrefab; }
        public Color FirstBossColor { get => _firstBossColor; }
        public ObjectPoolBase<FirstBossView> FirstBoss { get => _firstBoss; }
        public UniGameObjectPoolBase FirstBossObstacle { get => _firstBossObstacle; }
        public UniGameObjectPoolBase FirstBossButton { get => _firstBossButton; }
        public Color SecondBossColor { get => _secondBossColor; }
        public ObjectPoolBase<SecondBossView> SecondBoss { get => _secondBoss; }
        public UniGameObjectPoolBase SecondBossObstacle { get => _secondBossObstacle; }
        public UniGameObjectPoolBase SecondBossWall { get => _secondBossWall; }
        public UniGameObjectPoolBase SecondBossButton { get => _secondBossButton; }
        public Color WallElementColor { get => _wallElementColor; }
        public ObjectPoolBase<MeshRenderer> WallElement { get => _wallElement; }
        public Color FreeElementColor { get => _freeElementColor; }
        public ObjectPoolBase<MeshRenderer> FreeLevelElement { get => _freeLevelElement; }

        public void Start()
        {
            _defaultPropertyBlock = GetMaterialPropertyBlock(DefaultColor);
            _playerPropertyBlock = GetMaterialPropertyBlock(PlayerColor);
            _firstBossPropertyBlock = GetMaterialPropertyBlock(FirstBossColor);
            _secondBossPropertyBlock = GetMaterialPropertyBlock(SecondBossColor);

            _playerBehaviorPrefab.SetOnCreateAction((p) =>
            {
                p.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_playerPropertyBlock);
                _objectResolver.Inject(p);
            });
            _firstBossObstacle.SetOnCreateAction((g) => g.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_firstBossPropertyBlock));
            _firstBoss.SetOnCreateAction((g) =>
            {
                g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_firstBossPropertyBlock);
                _objectResolver.Inject(g);
            });
            _firstBossButton.SetOnCreateAction((g) => g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock));
            _firstBossButton.SetOnCreateAction((g) => g.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_firstBossPropertyBlock));
            _secondBoss.SetOnCreateAction((g) =>
            {
                g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_secondBossPropertyBlock);
                _objectResolver.Inject(g);
            });
            _secondBossObstacle.SetOnCreateAction((g) => g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock));
            _secondBossObstacle.SetOnCreateAction((g) => g.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(_secondBossPropertyBlock));
            _secondBossWall.SetOnCreateAction((g) => g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_secondBossPropertyBlock));
            _secondBossButton.SetOnCreateAction((g) => g.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock));
            _secondBossButton.SetOnCreateAction((g) => g.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_secondBossPropertyBlock));
            _wallElement.SetOnCreateAction((g) => g.SetPropertyBlock(GetMaterialPropertyBlock(WallElementColor)));
            _freeLevelElement.SetOnCreateAction((g) => g.SetPropertyBlock(GetMaterialPropertyBlock(FreeElementColor)));

            MaterialPropertyBlock GetMaterialPropertyBlock(Color color)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", color);
                return block;
            }
        }
    }
}
