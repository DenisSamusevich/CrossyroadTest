using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces
{
    public interface IHaveFeatures
    {
        IReadOnlyList<IFeatures> ReadOnlyFeatures { get; }

    }
}
