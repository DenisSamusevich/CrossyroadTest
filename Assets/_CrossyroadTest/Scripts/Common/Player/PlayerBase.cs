using MessagePipe;

namespace Assets._CrossyroadTest.Scripts.Common.Player
{
    internal class PlayerBase<TPlayer> where TPlayer : PlayerBase<TPlayer>
    {
        internal struct MovePlayerCommand<T> where T : TPlayer { }
        internal struct PushPlayerCommand<T> where T : TPlayer { }
        internal struct DancePlayerCommand<T> where T : TPlayer { }
        internal struct PlayerWinMessage<T> where T : TPlayer { }

        protected readonly ISubscriber<CollisionPlayerMessage> _collisionMessageSubscriber;
        protected readonly IAsyncPublisher<MovePlayerCommand<TPlayer>> _movePlayerPublisher;
        protected readonly IAsyncPublisher<DancePlayerCommand<TPlayer>> _dancePlayerPublisher;
        protected readonly IAsyncPublisher<PushPlayerCommand<TPlayer>> _pushPlayerPublisher;
        protected readonly ISubscriber<PlayerWinMessage<TPlayer>> _playerWinSubscriber;

        public PlayerBase(
            ISubscriber<CollisionPlayerMessage> collisionMessageSubscriber,
            IAsyncPublisher<MovePlayerCommand<TPlayer>> movePlayerPublisher,
            IAsyncPublisher<DancePlayerCommand<TPlayer>> dancePlayerPublisher,
            IAsyncPublisher<PushPlayerCommand<TPlayer>> pushPlayerPublisher,
            ISubscriber<PlayerWinMessage<TPlayer>> playerWinSubscriber)
        {
            _collisionMessageSubscriber = collisionMessageSubscriber;
            _movePlayerPublisher = movePlayerPublisher;
            _dancePlayerPublisher = dancePlayerPublisher;
            _pushPlayerPublisher = pushPlayerPublisher;
            _playerWinSubscriber = playerWinSubscriber;
        }
    }
}
