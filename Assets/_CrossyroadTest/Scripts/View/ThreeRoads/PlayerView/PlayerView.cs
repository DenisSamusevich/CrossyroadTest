using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.View.Common;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Threading;
using UnityEngine;
using PL = Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player.Player;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.PlayerView
{
    using PB = PlayerBase<PL>;

    internal class PlayerView : PlayerView<PL, IPlayerData>
    {
        protected Vector3 _levelStartPoint;

        protected override async UniTask PushPlayer(PB.PushPlayerCommand<PL> command, CancellationToken token)
        {
            await LMotion.Create(transform.position, new Vector3(transform.position.x, 0, transform.position.z), 0.3f).BindToPosition(transform);
            await LMotion.Create(Vector3.one, new Vector3(1f, 0.05f, 1f), 1f).BindToLocalScale(transform);
        }

        protected override async UniTask DancePlayer(PB.DancePlayerCommand<PL> command, CancellationToken token)
        {
            await LMotion.Create(transform.position, transform.position + Vector3.up, 0.4f)
                .WithEase(Ease.InCubic).WithLoops(6, LoopType.Yoyo).BindToPosition(transform);
        }

        protected override async UniTask MovePlayer(PB.MovePlayerCommand<PL> command, CancellationToken token)
        {
            await LSequence.Create()
                .Append(LMotion.Create(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(_levelStartPoint.x + _playerData.Position.x, _levelStartPoint.z + _playerData.Position.y),
                    0.5f).BindToPositionXZ(transform))
                .Join(LMotion.Create(transform.position.y, 1, 0.25f).WithEase(Ease.InOutCubic).WithLoops(2, LoopType.Yoyo).BindToPositionY(transform))
                .Run().ToUniTask(token).SuppressCancellationThrow();
        }

        protected override void Init()
        {
            _levelStartPoint = transform.position - new Vector3(_playerData.Position.x, 0, _playerData.Position.y);

        }
    }
}
