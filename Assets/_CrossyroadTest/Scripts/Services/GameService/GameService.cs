using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.Services.GameService

{
    internal class GameService : IStartable
    {
        private readonly GamesList _gamesList;

        public GameService(GamesList gamesList)
        {
            _gamesList = gamesList;
        }

        public void Start()
        {
            StartGame(CancellationToken.None).Forget();
        }

        public async UniTask StartGame(CancellationToken cancellationToken)
        {
            await UniTask.Yield();
            while (_gamesList.IsEmpty == false)
            {
                await _gamesList.CurrentGame.PrepareGameAsync(cancellationToken);
                var gameStatus = await _gamesList.CurrentGame.GameAsync(cancellationToken);
                _gamesList.CurrentGame.Dispose();
                switch (gameStatus)
                {
                    case GameResult.Fail:
                        _gamesList.Reset();
                        break;
                    case GameResult.PlayerWin:
                        _gamesList.NextGame();
                        break;
                    case GameResult.Cancel:
                        _gamesList.Reset();
                        break;
                    default:
                        break;
                }
                if (_gamesList.IsEmpty)
                {
                    _gamesList.Reset();
                }
            }
        }
    }
}
