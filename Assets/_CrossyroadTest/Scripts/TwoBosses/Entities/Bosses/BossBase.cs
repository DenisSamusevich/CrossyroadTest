using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Interface;
using Assets._CrossyroadTest.Scripts.Common.Player;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.TwoBosses.Extensions;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses
{
    internal abstract class BossBase<TBoss> : IEntityInit<ILevelData>, IUpdate where TBoss : BossBase<TBoss>
    {
        internal struct SetStateCommand<T> where T : BossBase<TBoss>
        {
            public SetStateCommand(StateBoss state)
            {
                State = state;
            }
            public StateBoss State { get; }
        }
        internal struct CreatingObstacleCommand<T> where T : BossBase<TBoss>
        {
            public CreatingObstacleCommand(IEnumerable<ICellData> cellsForObstacle)
            {
                CellsForObstacle = cellsForObstacle;
            }
            public IEnumerable<ICellData> CellsForObstacle { get; }
        }
        internal struct CreateDamageButtonCommand<T> where T : BossBase<TBoss>
        {
            public CreateDamageButtonCommand(ICellData cellForButton)
            {
                CellForButton = cellForButton;
            }
            public ICellData CellForButton { get; }
        }

        private readonly IAsyncPublisher<SetStateCommand<TBoss>> _setStateHandler;
        private readonly IAsyncPublisher<CreatingObstacleCommand<TBoss>> _creatingObstacleHandler;
        private readonly IAsyncPublisher<CreateDamageButtonCommand<TBoss>> _createDamageButtonHandler;
        private readonly ISubscriber<ButtonColor, CollisionPlayerMessage> _collisionMessageSubscriber;
        private CancellationTokenSource _cancellationTokenSource;
        private IDisposable _disposable;
        protected StateBoss _nextState;
        protected ILevelData _cells;
        protected int _health;
        protected int _currentState;
        protected IReadOnlyList<StateBoss> _bossStates;
        protected int _countObstacle;
        protected bool _isChangeState;
        protected bool _isDefeated = true;
        protected ButtonColor _buttonColor;

        public BossBase(IAsyncPublisher<SetStateCommand<TBoss>> setStateHandler,
            IAsyncPublisher<CreatingObstacleCommand<TBoss>> creatingObstacleHandler,
            IAsyncPublisher<CreateDamageButtonCommand<TBoss>> createDamageButtonHandler,
            ISubscriber<ButtonColor, CollisionPlayerMessage> collisionMessageSubscriber)
        {
            _setStateHandler = setStateHandler;
            _creatingObstacleHandler = creatingObstacleHandler;
            _createDamageButtonHandler = createDamageButtonHandler;
            _collisionMessageSubscriber = collisionMessageSubscriber;
        }

        public async UniTask InitAsync(ILevelData levelData, CancellationToken cancellationToken)
        {
            _cells = levelData;
            await InitAsync(cancellationToken);
            _cancellationTokenSource = new CancellationTokenSource();
            _disposable = _collisionMessageSubscriber.Subscribe(_buttonColor, OnPlayerCollision);
            _currentState = 0;
            _nextState = _bossStates[_currentState];
            _isChangeState = true;
            _isDefeated = false;
        }

        protected abstract UniTask InitAsync(CancellationToken cancellationToken);

        internal void Destroy()
        {
            _disposable?.Dispose();
        }

        public void Update()
        {
            if (_isDefeated == false && _isChangeState)
            {
                _isChangeState = false;
                if (_nextState == StateBoss.CreatingObstacle)
                {
                    var cellForObstacle = _cells.GetAllCells.GetRandom(_countObstacle);
                    SetStateCreatingObstacle(cellForObstacle).Forget();
                }
                else if (_nextState == StateBoss.CreateDamageButton)
                {
                    var cellForDamageButton = _cells.GetAllCells.GetRandom();
                    SetStateCreateDamageButton(cellForDamageButton).Forget();
                }
                else
                {
                    SetSimpleState().Forget();
                }
            }

            async UniTask SetSimpleState()
            {
                _isDefeated = _nextState == StateBoss.Defeated;
                await _setStateHandler.PublishAsync(new SetStateCommand<TBoss>(_nextState), _cancellationTokenSource.Token).SuppressCancellationThrow();
                SetNextState();
            }

            async UniTask SetStateCreatingObstacle(List<ICellData> cellForObstacle)
            {
                await _creatingObstacleHandler.PublishAsync(new CreatingObstacleCommand<TBoss>(cellForObstacle), _cancellationTokenSource.Token);
                SetNextState();
            }

            async UniTask SetStateCreateDamageButton(ICellData cellForDamageButton)
            {
                var token = _cancellationTokenSource.Token;
                await _createDamageButtonHandler.PublishAsync(new CreateDamageButtonCommand<TBoss>(cellForDamageButton), token).SuppressCancellationThrow();
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
            if (message.TypeCollision == TypeCollision.PlayerButton)
            {
                _health -= 1;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                if (_health <= 0)
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
