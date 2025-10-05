using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings
{
    public interface ITR_AssetsSettings
    {
        public Color PlayerColor { get; }
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior { get; }
        public Color ObstacleColor { get; }
        public UniGameObjectPoolBase Obstacle { get; }
        public ObjectPoolBase<BoxCollider> TriggerCollider { get; }
        public Color FinalAreaColor { get; }
        public ObjectPoolBase<MeshRenderer> FinalAreaElement { get; }
        public Color FreeAreaColor { get; }
        public ObjectPoolBase<MeshRenderer> FreeAreaElement { get; }
        public Color RoadAreaColor { get; }
        public ObjectPoolBase<MeshRenderer> RoadAreaElement { get; }
        public Color StartAreaColor { get; }
        public ObjectPoolBase<MeshRenderer> StartAreaElement { get; }
    }

    [CreateAssetMenu(fileName = "TR_AssetsSettings", menuName = "GameSettings/ThreeRoadsSettings/TR_AssetsSettings")]
    public class TR_AssetsSettings : ScriptableObject, ITR_AssetsSettings, IStartable
    {
        private IObjectResolver _objectResolver;
        [SerializeField] private Color _playerColor = Color.white;
        [SerializeField] private ComponentObjectPool<PlayerBehaviorView> _playerBehavior;
        [SerializeField] private Color _obstacleColor = Color.white;
        [SerializeField] private UniGameObjectPool _obstacle;
        [SerializeField] private ComponentObjectPool<BoxCollider> _triggerCollider;
        [SerializeField] private Color _finalAreaColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _finalAreaElement;
        [SerializeField] private Color _freeAreaColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _freeAreaElement;
        [SerializeField] private Color _roadAreaColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _roadAreaElement;
        [SerializeField] private Color _startAreaColor = Color.white;
        [SerializeField] private ComponentObjectPool<MeshRenderer> _startAreaElement;
        private MaterialPropertyBlock _playerBlock;
        private MaterialPropertyBlock _obstacleBlock;
        private MaterialPropertyBlock _finalAreaBlock;
        private MaterialPropertyBlock _freeAreaBlock;
        private MaterialPropertyBlock _roadAreaBlock;
        private MaterialPropertyBlock _startAreaBlock;

        [Inject]
        public void Initialize(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public Color PlayerColor { get => _playerColor; }
        public ObjectPoolBase<PlayerBehaviorView> PlayerBehavior { get => _playerBehavior; }
        public Color ObstacleColor { get => _obstacleColor; }
        public UniGameObjectPoolBase Obstacle { get => _obstacle; }
        public ObjectPoolBase<BoxCollider> TriggerCollider { get => _triggerCollider; }
        public Color FinalAreaColor { get => _finalAreaColor; }
        public ObjectPoolBase<MeshRenderer> FinalAreaElement { get => _finalAreaElement; }
        public Color FreeAreaColor { get => _freeAreaColor; }
        public ObjectPoolBase<MeshRenderer> FreeAreaElement { get => _freeAreaElement; }
        public Color RoadAreaColor { get => _roadAreaColor; }
        public ObjectPoolBase<MeshRenderer> RoadAreaElement { get => _roadAreaElement; }
        public Color StartAreaColor { get => _startAreaColor; }
        public ObjectPoolBase<MeshRenderer> StartAreaElement { get => _startAreaElement; }

        public void Start()
        {
            _playerBlock = GetMaterialPropertyBlock(PlayerColor);
            _obstacleBlock = GetMaterialPropertyBlock(ObstacleColor);
            _finalAreaBlock = GetMaterialPropertyBlock(FinalAreaColor);
            _freeAreaBlock = GetMaterialPropertyBlock(FreeAreaColor);
            _roadAreaBlock = GetMaterialPropertyBlock(RoadAreaColor);
            _startAreaBlock = GetMaterialPropertyBlock(StartAreaColor);

            _playerBehavior.SetOnCreateAction((p) =>
            {
                p.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_playerBlock);
                _objectResolver.Inject(p);
            });
            _obstacle.SetOnCreateAction((p) => p.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_obstacleBlock));
            _finalAreaElement.SetOnCreateAction((p) => p.SetPropertyBlock(_finalAreaBlock));
            _freeAreaElement.SetOnCreateAction((p) => p.SetPropertyBlock(_freeAreaBlock));
            _roadAreaElement.SetOnCreateAction((p) => p.SetPropertyBlock(_roadAreaBlock));
            _startAreaElement.SetOnCreateAction((p) => p.SetPropertyBlock(_startAreaBlock));

            MaterialPropertyBlock GetMaterialPropertyBlock(Color color)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", color);
                return block;
            }
        }
    }
}
