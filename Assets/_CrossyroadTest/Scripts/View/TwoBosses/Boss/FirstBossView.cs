using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    internal class FirstBossView : BossView<FirstBoss, IFirstBossData>
    {
        [SerializeField] private GameObject _obstacle;
        [SerializeField] private GameObject _button;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Color _color;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private float _obstaclePositionYForCreate = 0f;
        [SerializeField] private float _obstaclePositionYForPrewAttack = 0f;
        [SerializeField] private float _obstaclePositionYForAttack = 0f;
        private List<GameObject> _reserveObstacles = new List<GameObject>();
        private List<GameObject> _attackObstacles = new List<GameObject>();
        private GameObject _damageButton;
        private Vector3 _levelStartPoint;
        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock _defaultPropertyBlock;

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
            }
            _damageButton.SetActive(false);
            LMotion.Create(transform.localScale, new Vector3(1, 1, 1), 1).BindToLocalScale(transform);
        }

        protected override async UniTask SetState(BossBase<FirstBoss>.SetStateCommand<FirstBoss> command, CancellationToken token)
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

        protected override async UniTask CreateDamageButton(BossBase<FirstBoss>.CreateDamageButtonCommand<FirstBoss> command, CancellationToken token)
        {
            _damageButton.SetActive(true);
            _damageButton.transform.position = _levelStartPoint + new Vector3(command.CellForButton.Position.x, _obstaclePositionYForCreate, command.CellForButton.Position.y);
            await LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForAttack, 0.5f)
               .BindToPositionY(_damageButton.transform);
            await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
            await LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForCreate, 0.5f)
               .BindToPositionY(_damageButton.transform);
        }

        protected override async UniTask CreatingObstacle(BossBase<FirstBoss>.CreatingObstacleCommand<FirstBoss> command, CancellationToken token)
        {
            foreach (var cell in command.CellsForObstacle)
            {
                GameObject obctacle;
                if (_reserveObstacles.Count == 0)
                {
                    obctacle = Instantiate(_obstacle, _levelStartPoint + new Vector3(cell.Position.x, _obstaclePositionYForCreate, cell.Position.y), Quaternion.identity);
                    obctacle.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);

                }
                else
                {
                    obctacle = _reserveObstacles[^1];
                    _reserveObstacles.RemoveAt(_reserveObstacles.Count - 1);
                    obctacle.SetActive(true);
                    obctacle.transform.position = _levelStartPoint + new Vector3(cell.Position.x, _obstaclePositionYForCreate, cell.Position.y);
                }
                _attackObstacles.Add(obctacle);
                await LMotion.Create(obctacle.transform.position.y, _obstaclePositionYForPrewAttack, 0.2f)
                    .BindToPositionY(obctacle.transform);
            }
            await UniTask.WaitForSeconds(3);
            foreach (var item in _attackObstacles)
            {
                LMotion.Create(item.transform.position.y, _obstaclePositionYForAttack, 0.5f)
                    .BindToPositionY(item.transform);
            }
            await UniTask.WaitForSeconds(1, cancellationToken: token).SuppressCancellationThrow();
            foreach (var item in _attackObstacles)
            {
                LMotion.Create(item.transform.position.y, _obstaclePositionYForCreate, 2)
                    .BindToPositionY(item.transform);
            }
            await UniTask.WaitForSeconds(3);
            foreach (var item in _attackObstacles)
            {
                item.SetActive(false);
                _reserveObstacles.Add(item);
            }
            _attackObstacles.Clear();
        }
    }
}
