using System;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.View.Common
{
    internal class PlayerViewContainer
    {
        private Dictionary<Type, object> players = new Dictionary<Type, object>();

        public void AddPlayerView<T>(T playerView)
        {
            players.Add(typeof(T), playerView);
        }

        public T GetPlayerView<T>()
        {
            return (T)players[typeof(T)];
        }
    }
}
