﻿namespace UnitWarfare.Territories
{
    public interface ITerritoryOccupant
    {
        public ITerritoryOwner Owner { get; }

        public Territory OccupiedTerritory { get; }

        public void SetOccupiedTerritory(Territory territory);
    }
}
