using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Extensions;
using Assets._CrossyroadTest.Scripts.Services.Extensions;
using Assets._CrossyroadTest.Scripts.View.AutoBattler;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Extensions;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Handlers;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Assets._CrossyroadTest.Scripts.View.Common.Extensions;
using Assets._CrossyroadTest.Scripts.View.Common.Services;
using Assets._CrossyroadTest.Scripts.View.ThreeRoads;
using Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings;
using Assets._CrossyroadTest.Scripts.View.TwoBosses;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraService _cameraService;

        [Header("ThreeRoadsGame")]
        [SerializeField] private TR_AssetsSettings _assetsSettings;
        [SerializeField] private TR_LevelSettings _levelSettings;
        [SerializeField] private View.ThreeRoads.LevelView _levelView;
        [SerializeField] private ObstaclesView _obstaclesView;
        [SerializeField] private View.ThreeRoads.PlayerView _playerView;

        [Header("TwoBossesGame")]
        [SerializeField] private TB_AssetsSettings assetsSettings;
        [SerializeField] private TB_LevelSettings levelSettings;
        [SerializeField] private BossView bossView;
        [SerializeField] private View.TwoBosses.LevelView levelView;
        [SerializeField] private View.TwoBosses.PlayerView playerView;

        [Header("AutoBattlerGame")]
        [SerializeField] private AB_AssetsSettings ab_AssetsSettings;
        [SerializeField] private AB_LevelSettings ab_LevelSettings;
        [SerializeField] private View.AutoBattler.LevelView ab_LevelView;
        [SerializeField] private View.AutoBattler.PlayerView ab_PlayerView;
        [SerializeField] private EnemyView ab_EnemyView;
        [SerializeField] private GameMessageView ab_GameMessageView;
        [SerializeField] private LevelUpHandler ab_LevelUpHandler;
        [SerializeField] private OfferWeapomHandler ab_OfferWeapomHandler;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterCommonService().RegisterAutoBattlerGame();
            //.RegisterThreeRoadsGame()
            //.RegisterTwoBossGame();
            builder.RegisterViewCommon(_cameraService, _camera);
            //.RegisterViewThreeRoads(_assetsSettings, _levelSettings, _levelView, _obstaclesView, _playerView)
            // .RegisterViewTwoBosses(assetsSettings, levelSettings, bossView, levelView, playerView);
            builder.RegisterViewAutoBattler(ab_AssetsSettings, ab_LevelSettings, ab_LevelView,
                ab_EnemyView, ab_PlayerView, ab_LevelUpHandler, ab_OfferWeapomHandler, ab_GameMessageView);
        }
    }
}
