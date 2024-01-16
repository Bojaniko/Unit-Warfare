using UnitWarfare.Core.Enums;

namespace UnitWarfare.Territories
{
    public interface ITerritoryOwner
    {
        public string Name { get; }
        public PlayerIdentification OwnerIdentification { get; }
    }
}