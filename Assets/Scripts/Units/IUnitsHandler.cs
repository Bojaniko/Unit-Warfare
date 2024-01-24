using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public interface IUnitsHandler
    {
        public IUnit[] GetUnitsForOwner(ITerritoryOwner owner);
        public IUnit[] GetUnitsForOwnerType(PlayerIdentification owner);
        public UnitInteractions InteractionsHandler { get; }

        public bool UnitExecutingCommand { get; }
    }
}
