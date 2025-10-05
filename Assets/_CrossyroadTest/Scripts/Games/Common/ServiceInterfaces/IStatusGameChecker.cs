using Assets._CrossyroadTest.Scripts.Services.GameService.Base;

namespace Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces
{
    public interface IStatusGameChecker
    {
        bool HasResult { get; }
        GameResult GameResult { get; }
    }
}
