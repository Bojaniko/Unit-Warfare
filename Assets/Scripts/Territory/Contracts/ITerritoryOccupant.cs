using UnitWarfare.Core.Enums;

namespace UnitWarfare.Territories
{
    public interface ITerritoryOccupant
    {
        public PlayerIdentification Owner { get; }

        public Territory OccupiedTerritory { get; }

        public void SetOccupiedTerritory(Territory territory);
    }
}
