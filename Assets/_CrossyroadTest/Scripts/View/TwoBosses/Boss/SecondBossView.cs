using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Assets._CrossyroadTest.Scripts.View.Common.ObjectPool;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    public class SecondBossView : BossBehaviorView<BossId>
    {
        [SerializeField] private float _obstaclePositionYForCreate = 0f;
        [SerializeField] private float _obstaclePositionYForPrewAttack = 0f;
        [SerializeField] private float _obstaclePositionYForAttack = 0f;
        private GameObject _firstObstacle;
        private GameObject _secondObstacle;
        private GameObject _wallObstacle;
        private GameObject _damageButton;

        protected override BossId Id => BossId.BlueBoss;

        protected override void Init() { }

        protected override async UniTask CreateDamageButton(BossBehaviorBase<BossId>.CreateDamageButtonCommand command, CancellationToken token)
        {
            _damageButton = _worldObjectService.GetGameObject(_assetsSettings.SecondBossButton, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(command.PositionForButton.x, _obstaclePositionYForCreate, command.PositionForButton.y), Quaternion.identity);
            LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForAttack, 0.5f)
               .BindToPositionY(_damageButton.transform);
            await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
            LMotion.Create(_damageButton.transform.position.y, _obstaclePositionYForCreate, 0.5f)
               .BindToPositionY(_damageButton.transform);
            _worldObjectService.ReturnGameObject(_assetsSettings.SecondBossButton, _damageButton);
            _damageButton = null;
        }

        protected override async UniTask CreatingObstacle(BossBehaviorBase<BossId>.CreatingObstacleCommand command, CancellationToken token)
        {
            var firstCell = command.CellsForObstacle.First();
            var secondCell = command.CellsForObstacle.Last();
            _firstObstacle = _worldObjectService.GetGameObject(_assetsSettings.SecondBossObstacle, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(firstCell.x, _obstaclePositionYForCreate, firstCell.y), Quaternion.identity);
            _secondObstacle = _worldObjectService.GetGameObject(_assetsSettings.SecondBossObstacle, _worldObjectService.CurrentWorld.ZeroPosition + new Vector3(secondCell.x, _obstaclePositionYForCreate, secondCell.y), Quaternion.identity);
            _wallObstacle = _worldObjectService.GetGameObject(_assetsSettings.SecondBossWall, _firstObstacle.transform.position, Quaternion.LookRotation(_secondObstacle.transform.position - _firstObstacle.transform.position));
            _wallObstacle.transform.localScale = new Vector3(1, 1, Vector3.Distance(_firstObstacle.transform.position, _secondObstacle.transform.position));
            MoveObstacleByY(_obstaclePositionYForPrewAttack, 0.2f);
            await UniTask.WaitForSeconds(2);
            MoveObstacleByY(_obstaclePositionYForAttack, 0.5f);
            await UniTask.WaitForSeconds(2, cancellationToken: token).SuppressCancellationThrow();
            MoveObstacleByY(_obstaclePositionYForCreate, 2f);
            await UniTask.WaitForSeconds(2);
            _worldObjectService.ReturnGameObject(_assetsSettings.SecondBossObstacle, _firstObstacle);
            _worldObjectService.ReturnGameObject(_assetsSettings.SecondBossObstacle, _secondObstacle);
            _worldObjectService.ReturnGameObject(_assetsSettings.SecondBossObstacle, _wallObstacle);
            _firstObstacle = null;
            _secondObstacle = null;
            _wallObstacle = null;

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
            ReturnGameObject(_assetsSettings.SecondBossButton, ref _damageButton);
            ReturnGameObject(_assetsSettings.SecondBossObstacle, ref _firstObstacle);
            ReturnGameObject(_assetsSettings.SecondBossObstacle, ref _secondObstacle);
            ReturnGameObject(_assetsSettings.SecondBossWall, ref _wallObstacle);
        }

        private void ReturnGameObject(UniGameObjectPoolBase uniGameObjectPoolBase, ref GameObject gameObject)
        {
            if (gameObject != null)
            {
                _worldObjectService.ReturnGameObject(uniGameObjectPoolBase, gameObject);
                gameObject = null;
            }
        }
    }
}
