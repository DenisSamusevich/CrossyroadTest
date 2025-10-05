using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Extensions;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services
{
    public class WorldCreator : IWorldCreator
    {
        private readonly Dictionary<IWorldSetting, IWorldData> _hasWorlds = new Dictionary<IWorldSetting, IWorldData>();
        private readonly WorldObjectService _worldObjectService;
        private IWorldSetting _currentWorldSetting;
        private Vector3 _zeroPosition;

        public WorldCreator(WorldObjectService worldObjectService)
        {
            _worldObjectService = worldObjectService;
        }

        public void CreateWorld(IWorldSetting worldSetting)
        {
            _currentWorldSetting = worldSetting;
            if (_hasWorlds.ContainsKey(worldSetting))
            {
                _worldObjectService.SetCurrentWorld(_hasWorlds[worldSetting]);
            }
            else
            {
                _worldObjectService.CreateNewWorld(_zeroPosition, _currentWorldSetting.IsDestroyAfterGame, _currentWorldSetting.IsUsedRepeatedly);
                if (_currentWorldSetting.IsDestroyAfterGame == false && _currentWorldSetting.IsUsedRepeatedly)
                {
                    _hasWorlds.Add(_currentWorldSetting, _worldObjectService.CurrentWorld);
                }
            }
        }

        public void DestroyWorld()
        {
            if (_currentWorldSetting.IsDestroyAfterGame == false)
            {
                _zeroPosition = _worldObjectService.CurrentWorld.ZeroPosition.AddZ(_currentWorldSetting.WorldSize.y);
            }
            else
            {
                _worldObjectService.DestroyCurrentWorld();
            }
        }
    }
}
