namespace Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces
{
    public interface IWorldCreator
    {
        void CreateWorld(IWorldSetting worldSetting);
        void DestroyWorld();
    }
}
