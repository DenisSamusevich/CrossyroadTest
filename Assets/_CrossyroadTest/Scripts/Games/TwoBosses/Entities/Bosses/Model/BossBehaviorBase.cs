using Assets._CrossyroadTest.Scripts.Games.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Extensions;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model
{
    public abstract class BossBehaviorBase<TBossId> : IInitAsyncElement<ILevelData, BossData>, IBossBehavior
    {
        public struct SetStateCommand
        {
            public SetStateCommand(StateBoss state)
            {
                State = state;
            }
            public StateBoss State { get; }
        }
        public struct CreatingObstacleCommand
        {
            public CreatingObstacleCommand(IEnumerable<Vector2Int> cellsForObstacle)
            {
                CellsForObstacle = cellsForObstacle;
            }
            public IEnumerable<Vector2Int> CellsForObstacle { get; }
        }
        public struct CreateDamageButtonCommand
        {
            public CreateDamageButtonCommand(Vector2Int cellForButton)
            {
                PositionForButton = cellForButton;
            }
            public Vector2Int PositionForButton { get; }
        }

        private readonly IAsyncPublisher<TBossId, SetStateCommand> _setStateHandler;
        private readonly IAsyncPublisher<TBossId, CreatingObstacleCommand> _creatingObstacleHandler;
        private readonly IAsyncPublisher<TBossId, CreateDamageButtonCommand> _createDamageButtonHandler;
        private readonly ISubscriber<TBossId, CollisionPlayerMessage> _collisionMessageSubscriber;
        private IDisposable _disposable;
        private CancellationTokenSource _cancellationTokenSource;
        protected TBossId bossId;
        protected ILevelData _levelData;
        protected BossData _bossData;
        protected IReadOnlyList<StateBoss> _bossStates;
        protected StateBoss _nextState;
        protected int _currentState;
        protected int _countObstacle;
        protected bool _isChangeState;


        public BossBehaviorBase(IAsyncPublisher<TBossId, SetStateCommand> setStateHandler,
            IAsyncPublisher<TBossId, CreatingObstacleCommand> creatingObstacleHandler,
            IAsyncPublisher<TBossId, CreateDamageButtonCommand> createDamageButtonHandler,
            ISubscriber<TBossId, CollisionPlayerMessage> collisionMessageSubscriber)
        {
            _setStateHandler = setStateHandler;
            _creatingObstacleHandler = creatingObstacleHandler;
            _createDamageButtonHandler = createDamageButtonHandler;
            _collisionMessageSubscriber = collisionMessageSubscriber;
        }

        public async UniTask InitAsyncElement(ILevelData levelData, BossData bossData)
        {
            _levelData = levelData;
            _bossData = bossData;
            await InitAsync();
            _cancellationTokenSource = new CancellationTokenSource();
            _disposable = _collisionMessageSubscriber.Subscribe(bossId, OnPlayerCollision);
            _nextState = _bossStates[_currentState];
            _currentState = 0;
            _isChangeState = true;
        }

        protected abstract UniTask InitAsync();

        internal void Destroy()
        {
            _disposable?.Dispose();
        }

        public void Update()
        {
            if (_bossData.IsDefeated == false && _isChangeState)
            {
                _isChangeState = false;
                if (_nextState == StateBoss.CreatingObstacle)
                {
                    var cellForObstacle = _levelData.AllCellPositions.GetRandom(_countObstacle);
                    SetStateCreatingObstacle(cellForObstacle).Forget();
                }
                else if (_nextState == StateBoss.CreateDamageButton)
                {
                    var cellForDamageButton = _levelData.AllCellPositions.GetRandom();
                    SetStateCreateDamageButton(cellForDamageButton).Forget();
                }
                else
                {
                    SetSimpleState().Forget();
                }
            }

            async UniTask SetSimpleState()
            {
                _bossData.IsDefeated = _nextState == StateBoss.Defeated;
                await _setStateHandler.PublishAsync(bossId, new SetStateCommand(_nextState), _cancellationTokenSource.Token).SuppressCancellationThrow();
                SetNextState();
            }

            async UniTask SetStateCreatingObstacle(List<Vector2Int> cellForObstacle)
            {
                await _creatingObstacleHandler.PublishAsync(bossId, new CreatingObstacleCommand(cellForObstacle), _cancellationTokenSource.Token);
                SetNextState();
            }

            async UniTask SetStateCreateDamageButton(Vector2Int cellForDamageButton)
            {
                var token = _cancellationTokenSource.Token;
                await _createDamageButtonHandler.PublishAsync(bossId, new CreateDamageButtonCommand(cellForDamageButton), token).SuppressCancellationThrow();
                if (token.IsCancellationRequested == false)
                {
                    SetNextState();
                }
            }

            void SetNextState()
            {
                _currentState = _currentState == _bossStates.Count - 1 ? 0 : ++_currentState;
                _nextState = _bossStates[_currentState];
                _isChangeState = true;
            }
        }

        private void OnPlayerCollision(CollisionPlayerMessage message)
        {
            if (message.TypeCollision == CollisionType.PlayerButton)
            {
                _bossData.Health -= 1;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                if (_bossData.Health <= 0)
                {
                    _nextState = StateBoss.Defeated;
                }
                else
                {
                    _nextState = StateBoss.Damage;
                }
                _isChangeState = true;
            }
        }
    }
}
