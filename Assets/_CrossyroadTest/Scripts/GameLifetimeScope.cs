using Assets._CrossyroadTest.GameServices.Extensions;
using Assets._CrossyroadTest.Scripts.Common.Extensions;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Extensions;
using Assets._CrossyroadTest.Scripts.TwoBosses.Extensions;
using Assets._CrossyroadTest.Scripts.View.Common.Extensions;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] Camera _camera;
    protected override void Configure(IContainerBuilder builder)
    {

        builder.RegisterComponent(_camera);
        builder.RegisterMessagePipe();

        builder.RegisterGameServices()
            .RegisterCommonService()
            .RegisterThreeRoadsGame()
            .RegisterTwoBossGame()
            .RegisterViewCommon();
    }
}
