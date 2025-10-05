using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler
{
    public class AB_StatusGameChecker : IStatusGameChecker, IAB_GameService
    {

        private readonly IAB_GameSession _gameSession;
        private CancellationTokenSource _cancellationTokenSource;

        public AB_StatusGameChecker(
            IAB_GameSession gameSession)
        {
            _gameSession = gameSession;
        }

        public bool HasResult { get; private set; }
        public GameResult GameResult { get; private set; }

        public void EnableService()
        {
            GameResult = GameResult.None;
            HasResult = false;
            _cancellationTokenSource = new CancellationTokenSource();
            CheckEnemiesDefeated(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask CheckEnemiesDefeated(CancellationToken token)
        {
            int currentEnemyIndex = 0;
            while (token.IsCancellationRequested == false)
            {
                if (_gameSession.PlayerModel.EntityData.IsDefeated)
                {
                    GameResult = GameResult.Fail;
                    HasResult = true;
                    return;
                }
                else if (_gameSession.EnemyModels[currentEnemyIndex].EntityData.IsDefeated)
                {
                    currentEnemyIndex++;
                    if (currentEnemyIndex == _gameSession.EnemyModels.Count)
                    {
                        GameResult = GameResult.PlayerWin;
                        HasResult = true;
                        return;
                    }
                }
                await UniTask.WaitForSeconds(1f);
            }
        }

        public void DisableService()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
