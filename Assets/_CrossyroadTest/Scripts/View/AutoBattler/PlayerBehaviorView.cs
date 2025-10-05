using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler
{
    public class PlayerBehaviorView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private TextMeshPro _textMeshPro;
        private IAsyncSubscriber<PlayerCommand, BehaviorCommand> _asyncBehaviorSubscriber;
        private IDisposable _disposable;
        private IPlayerData _playerData;
        private IAB_LevelSettings _levelSettings;
        private IAB_AssetsSettings _assetsSettings;

        [Inject]
        public void Initialize(IAsyncSubscriber<PlayerCommand, BehaviorCommand> asyncBehaviorSubscriber,
            IAB_LevelSettings levelSettings, IAB_AssetsSettings assetsSettings)
        {
            _asyncBehaviorSubscriber = asyncBehaviorSubscriber;
            _levelSettings = levelSettings;
            _assetsSettings = assetsSettings;
        }

        public void SetPlayerData(IPlayerData playerData)
        {
            _playerData = playerData;
            var bag = DisposableBag.CreateBuilder();
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.TakeDamage, TakeDamage).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.SetDamage, SetDamage).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Defeated, Defeated).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Miss, Miss).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.SelectClass, SelectClass).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.LevelUp, LevelUp).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.Win, Win).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.MissAttacking, MissAttacking).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe(PlayerCommand.RestoreHealth, RestoreHealth).AddTo(bag);
            _disposable = bag.Build();
            LMotion.Create(_pivotTransform.transform.localScale, Vector3.one, 1)
                    .WithEase(Ease.InElastic)
                    .BindToLocalScale(_pivotTransform.transform);
        }

        private UniTask RestoreHealth(BehaviorCommand command, CancellationToken token)
        {
            _textMeshPro.text = _playerData.Health.ToString();
            return UniTask.CompletedTask;
        }

        private async UniTask TakeDamage(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            await LMotion.Create(new Vector3(30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private UniTask SelectClass(BehaviorCommand command, CancellationToken token)
        {
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", _levelSettings.ClassDictionary[_playerData.CharacterClass].CharacterColor);
            _meshRenderer.SetPropertyBlock(block);
            return UniTask.CompletedTask;
        }

        private async UniTask Miss(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            await LMotion.Create(new Vector3(30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private async UniTask Defeated(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.one, new Vector3(1f, 0.05f, 1f), 1f).BindToLocalScale(_pivotTransform);
        }

        private async UniTask LevelUp(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(_pivotTransform.localScale, _pivotTransform.localScale * 1.08f, 0.4f)
                .WithEase(Ease.OutElastic).BindToLocalScale(_pivotTransform);
        }

        private async UniTask SetDamage(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(-30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            _textMeshPro.text = _playerData.Health.ToString();
            await LMotion.Create(new Vector3(-30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private async UniTask MissAttacking(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + new Vector3(0, 0, -0.5f), 0.25f).BindToPosition(_pivotTransform);
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + new Vector3(0, 0, 0.5f), 0.25f).BindToPosition(_pivotTransform);
        }

        private async UniTask Win(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + Vector3.up, 0.4f)
                .WithEase(Ease.InCubic).WithLoops(6, LoopType.Yoyo).BindToPosition(_pivotTransform);
        }

        public void DisposePlayer()
        {
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", _assetsSettings.PlayerColor);
            _meshRenderer?.SetPropertyBlock(block);
            _disposable?.Dispose();
        }
    }
}
