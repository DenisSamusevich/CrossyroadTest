using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    internal class SecondBossView : BossView<SecondBoss, ISecondBossData>
    {
        [SerializeField] private GameObject _obstacle;
        [SerializeField] private GameObject _button;
        [SerializeField] private GameObject _wall;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Color _color;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private float _obstaclePositionYForCreate = 0f;
        [SerializeField] private float _obstaclePositionYForPrewAttack = 0f;
        [SerializeField] private float _obstaclePositionYForAttack = 0f;
        private GameObject _firstObstacle;
        private GameObject _secondObstacle;
        private GameObject _wallObstacle;
        private GameObject _damageButton;
        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock _defaultPropertyBlock;
        private Vector3 _levelStartPoint;
        protected override void Init()
        {
            _levelStartPoint = transform.position - new Vector3(_bossData.BossPosition.x, 0, _bossData.BossPosition.y);
            if (_damageButton == null)
            {
                _defaultPropertyBlock = new MaterialPropertyBlock();
                _defaultPropertyBlock.SetColor("_BaseColor", _defaultColor);
                _meshRenderer.SetPropertyBlock(_defaultPropertyBlock);
                _materialPropertyBlock = new MaterialPropertyBlock();
                _materialPropertyBlock.SetColor("_BaseColor", _color);
                _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
                _damageButton = Instantiate(_button, _levelStartPoint + new Vector3(0, _obstaclePositionYForCreate, 0), Quaternion.identity);
                _damageButton.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock);
                _damageButton.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                _damageButton.SetActive(false);
                _firstObstacle = Instantiate(_obstacle, _levelStartPoint + new Vector3(0, _obstaclePositionYForCreate, 0), Quaternion.identity);
                _firstObstacle.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock);
                _firstObstacle.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                _firstObstacle.SetActive(false);
                _secondObstacle = Instantiate(_obstacle, _levelStartPoint + new Vector3(0, _obstaclePositionYForCreate, 0), Quaternion.identity);
                _secondObstacle.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_defaultPropertyBlock);
                _secondObstacle.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                _secondObstacle.SetActive(false);
                _wallObstacle = Instantiate(_wall, _levelStartPoint + new Vector3(0, _obstaclePositionYForCreate, 0), Quaternion.identity);
                _wallObstacle.transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                _wallObstacle.SetActive(false);
            }
        }

        protected override async UniTask SetState(BossBase<SecondBoss>.SetStateCommand<SecondBoss> command, CancellationToken token)
        {
            switch (command.State)
            {
                case StateBoss.Damage:
                    await LSequence.Create().Append(LMotion.Punch.Create(0, 30, 2).BindToEulerAnglesX(transform))
                         .Join(LMotion.Punch.Create(0, 30, 2).BindToEulerAnglesY(transform))
                         .Join(LMotion.Punch.Create(0, 30, 2).BindToEulerAnglesZ(transform)).Run();
                    break;
                case StateBoss.Defeated:
                    await LSequence.Create().Append(LMotion.Shake.Create(0, 50, 4).BindToEulerAnglesX(transform))
                        .Join(LMotion.Shake.Create(0, 50, 4).BindToEulerAnglesY(transform))
                        .Join(LMotion.Shake.Create(0, 50, 4).BindToEulerAnglesZ(transform)).Run();
                    await LMotion.Create(transform.localScale, new Vector3(1, 0.1f, 1), 2).BindToLocalScale(transform);
                    break;
                default:
                    break;
            }
        }

        protected override async UniTask CreateDamageButton(BossBase<SecondBoss>.CreateDamageButtonCommand<SecondBoss> command, CancellationToken token)
        {
            _damageButton.SetActive(true);
            _damageButton.transform.position = _levelStartPoint + new Vector3(command.CellForButton.Position.x, _obstaclePositionYForCreate, command.CellForButton.Position.y);
            LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForAttack, 0.5f)
               .BindToPositionY(_damageButton.transform);
            await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
            LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForCreate, 0.5f)
               .BindToPositionY(_damageButton.transform);
        }

        protected override async UniTask CreatingObstacle(BossBase<SecondBoss>.CreatingObstacleCommand<SecondBoss> command, CancellationToken token)
        {
            var firstCell = command.CellsForObstacle.First();
            var secondCell = command.CellsForObstacle.Last();
            _firstObstacle.SetActive(true);
            _secondObstacle.SetActive(true);
            _wallObstacle.SetActive(true);
            _firstObstacle.transform.position = _levelStartPoint + new Vector3(firstCell.Position.x, _obstaclePositionYForCreate, firstCell.Position.y);
            _secondObstacle.transform.position = _levelStartPoint + new Vector3(secondCell.Position.x, _obstaclePositionYForCreate, secondCell.Position.y);
            _wallObstacle.transform.position = _firstObstacle.transform.position;
            _wallObstacle.transform.forward = _secondObstacle.transform.position - _firstObstacle.transform.position;
            _wallObstacle.transform.localScale = new Vector3(1, 1, Vector3.Distance(_firstObstacle.transform.position, _secondObstacle.transform.position));
            MoveObstacleByY(_obstaclePositionYForPrewAttack, 0.2f);
            await UniTask.WaitForSeconds(2);
            MoveObstacleByY(_obstaclePositionYForAttack, 0.5f);
            await UniTask.WaitForSeconds(2, cancellationToken: token).SuppressCancellationThrow();
            MoveObstacleByY(_obstaclePositionYForCreate, 2f);
            await UniTask.WaitForSeconds(2);
            _firstObstacle.SetActive(false);
            _secondObstacle.SetActive(false);
            _wallObstacle.SetActive(false);

            void MoveObstacleByY(float y, float duration)
            {
                LMotion.Create(_firstObstacle.transform.position.y, y, duration)
                    .BindToPositionY(_firstObstacle.transform);
                LMotion.Create(_secondObstacle.transform.position.y, y, duration)
                    .BindToPositionY(_secondObstacle.transform);
                LMotion.Create(_wallObstacle.transform.position.y, y, duration)
                    .BindToPositionY(_wallObstacle.transform);
            }
        }
    }
}
