using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
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
    public class EnemyBehaviorView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private TextMeshPro _textMeshPro;
        [SerializeField] private TextMeshPro _name;
        private IAsyncSubscriber<(int, EnemyCommand), BehaviorCommand> _asyncBehaviorSubscriber;
        private IDisposable _disposable;
        private IEnemyData _enemyData;
        private IAB_LevelSettings _levelSettings;

        [Inject]
        public void Initialize(IAsyncSubscriber<(int, EnemyCommand), BehaviorCommand> asyncBehaviorSubscriber,
            IAB_LevelSettings levelSettings)
        {
            _asyncBehaviorSubscriber = asyncBehaviorSubscriber;
            _levelSettings = levelSettings;
        }

        internal void SetEnemyData(IEnemyData entityData)
        {
            _meshRenderer.enabled = false;
            _textMeshPro.gameObject.SetActive(false);
            _name.gameObject.SetActive(false);
            _enemyData = entityData;
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", _levelSettings.EnemyDictionary[_enemyData.EnemyType].EnemyColor);
            _meshRenderer.SetPropertyBlock(block);
            _name.text = _enemyData.Name;
            var bag = DisposableBag.CreateBuilder();
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.TakeDamage), TakeDamage).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.SetDamage), SetDamage).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.Defeated), Defeated).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.Miss), Miss).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.Win), Win).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.ActivateForBattle), ActivateForBattle).AddTo(bag);
            _asyncBehaviorSubscriber.Subscribe((_enemyData.Id, EnemyCommand.MissAttacking), MissAttacking).AddTo(bag);
            _disposable = bag.Build();
        }

        private async UniTask ActivateForBattle(BehaviorCommand command, CancellationToken token)
        {
            _meshRenderer.enabled = true;
            _textMeshPro.gameObject.SetActive(true);
            _name.gameObject.SetActive(true);
            _textMeshPro.text = _enemyData.Health.ToString();
            await LMotion.Create(_pivotTransform.position + new Vector3(0, 2, 0), _pivotTransform.position, 0.8f).WithEase(Ease.OutElastic).BindToPosition(_pivotTransform);
        }

        private async UniTask TakeDamage(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(-30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            await LMotion.Create(new Vector3(-30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private async UniTask SetDamage(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            _textMeshPro.text = _enemyData.Health.ToString();
            await LMotion.Create(new Vector3(30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private async UniTask Defeated(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.one, new Vector3(1f, 0.05f, 1f), 1f).BindToLocalScale(_pivotTransform);
            _meshRenderer.enabled = false;
            _textMeshPro.gameObject.SetActive(false);
            _name.gameObject.SetActive(false);
            _pivotTransform.localScale = Vector3.one;
        }

        private async UniTask Miss(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(Vector3.zero, new Vector3(-30, 0, 0), 0.25f).BindToLocalEulerAngles(_pivotTransform);
            await LMotion.Create(new Vector3(-30, 0, 0), Vector3.zero, 0.25f).BindToLocalEulerAngles(_pivotTransform);
        }

        private async UniTask MissAttacking(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + new Vector3(0, 0, 0.5f), 0.25f).BindToPosition(_pivotTransform);
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + new Vector3(0, 0, -0.5f), 0.25f).BindToPosition(_pivotTransform);
        }

        private async UniTask Win(BehaviorCommand command, CancellationToken token)
        {
            await LMotion.Create(_pivotTransform.position, _pivotTransform.position + Vector3.up, 0.4f)
                .WithEase(Ease.InCubic).WithLoops(6, LoopType.Yoyo).BindToPosition(_pivotTransform);
        }

        internal void DisposeEnemy()
        {
            _meshRenderer.enabled = false;
            _textMeshPro.gameObject.SetActive(false);
            _name.gameObject.SetActive(false);
            _disposable?.Dispose();
        }
    }
}