using Assets._CrossyroadTest.GameServices.Base;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.GameServices.Servises
{
    internal class GamesList
    {
        private readonly IEnumerable<GameBase> _allGames;
        private readonly Queue<GameBase> _games;
        public GamesList(IEnumerable<GameBase> baseGames)
        {
            _allGames = baseGames;
            _games = new Queue<GameBase>(baseGames);
        }

        public bool IsEmpty { get => _games.Count == 0; }
        public GameBase CurrentGame { get => _games.Peek(); }

        public void NextGame()
        {
            _games.Dequeue();
        }

        internal void Reset()
        {
            _games.Clear();
            foreach (var item in _allGames)
            {
                _games.Enqueue(item);
            }
        }
    }
}