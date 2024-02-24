using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public interface IUnitOwner : ITerritoryOwner
    {
        public bool IsActive { get; }

        public UnitsData UnitsData { get; }

        public delegate void UnitOwnerEventHandler();
        public event UnitOwnerEventHandler OnRoundStarted;
        public event UnitOwnerEventHandler OnRoundEnded;
    }
}