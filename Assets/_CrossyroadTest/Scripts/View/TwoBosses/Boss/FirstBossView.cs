using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    public class FirstBossView : BossBehaviorView<BossId>
    {
        [SerializeField] private float _obstaclePositionYForCreate = 0f;
        [SerializeField] private float _obstaclePositionYForPrewAttack = 0f;
        [SerializeField] private float _obstaclePositionYForAttack = 0f;
        private List<GameObject> _attackObstacles = new List<GameObject>();
        private GameObject _damageButton;

        protected override BossId Id => BossId.RedBoss;

        protected override void Init()
        {
            LMotion.Create(transform.localScale, Vector3.one, 1).BindToLocalScale(transform);
        }

        protected override async UniTask CreateDamageButton(BossBehaviorBase<BossId>.CreateDamageButtonCommand command, CancellationToken token)
        {
            _damageButton = _worldObjectService.GetGameObject(_assetsSettings.FirstBossButton, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(0, _obstaclePositionYForCreate, 0), Quaternion.identity);
            _damageButton.transform.position = _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.PositionForButton.x, _obstaclePositionYForCreate, command.PositionForButton.y);
            await LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForAttack, 0.5f)
               .BindToPositionY(_damageButton.transform);
            await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
            await LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForCreate, 0.5f)
               .BindToPositionY(_damageButton.transform);
            _worldObjectService.ReturnGameObject(_assetsSettings.FirstBossButton, _damageButton);
            _damageButton = null;
        }

        protected override async UniTask CreatingObstacle(BossBehaviorBase<BossId>.CreatingObstacleCommand command, CancellationToken token)
        {
            foreach (var position in command.CellsForObstacle)
            {
                var obctacle = _worldObjectService.GetGameObject(_assetsSettings.FirstBossObstacle, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(position.x, _obstaclePositionYForCreate, position.y), Quaternion.identity);
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
                _worldObjectService.ReturnGameObject(_assetsSettings.FirstBossObstacle, item);
            }
            _attackObstacles.Clear();
        }

        protected override async UniTask SetState(BossBehaviorBase<BossId>.SetStateCommand command, CancellationToken token)
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

        protected override void Dispose()
        {
            if (_damageButton != null)
            {
                _worldObjectService.ReturnGameObject(_assetsSettings.FirstBossButton, _damageButton);
                _damageButton = null;
            }
            foreach (var item in _attackObstacles)
            {
                _worldObjectService.ReturnGameObject(_assetsSettings.FirstBossObstacle, item);
            }
            _attackObstacles.Clear();
        }
    }
}
