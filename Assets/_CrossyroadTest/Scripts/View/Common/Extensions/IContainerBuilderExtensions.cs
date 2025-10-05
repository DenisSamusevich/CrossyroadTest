using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.View.Common.Services;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Data;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.Common.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterViewCommon(this IContainerBuilder builder, CameraService cameraService, Camera camera)
        {
            builder.Register<InputService>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<CollisionService>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<WorldCreator>(Lifetime.Singleton).As<IWorldCreator>();
            builder.Register<WorldObjectService>(Lifetime.Singleton).As<IWorldObjectService>().AsSelf();
            builder.Register<WorldData>(Lifetime.Transient);
            builder.RegisterComponent(cameraService).As<ICameraService>();
            builder.RegisterComponent(camera);
            return builder;
        }
    }
}
