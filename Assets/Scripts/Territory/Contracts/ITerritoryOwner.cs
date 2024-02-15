using UnitWarfare.Core.Global;

namespace UnitWarfare.Territories
{
    public interface ITerritoryOwner
    {
        public string Name { get; }
        public PlayerIdentification Identification { get; }
    }
}