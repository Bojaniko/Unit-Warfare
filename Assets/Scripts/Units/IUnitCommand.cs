using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public readonly struct UnitTarget
    {
        private readonly Territory _territory;
        public Territory Territory => _territory;

        private readonly IUnit _unit;
        public IUnit Unit => _unit;

        public UnitTarget(IUnit unit)
        {
            _unit = unit;
            _territory = unit.OccupiedTerritory;
        }

        public UnitTarget(Territory territory)
        {
            _territory = territory;
            _unit = _territory.Occupant as IUnit;
        }
    }

    public interface IUnitCommand
    {
        public object OrderRef { get; }
        public UnitTarget Target { get; }
    }
}