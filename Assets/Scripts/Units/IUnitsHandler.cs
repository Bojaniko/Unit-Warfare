using Type = System.Type;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public interface IUnitsHandler
    {
        public UnityEngine.Transform UnitContainer { get; }

        public IUnit Spawn(Territory territory, Type type);
        public void Despawn(IUnit unit);

        public IUnit[] GetUnits(IUnitOwner owner);
        public UnitData GetUnitDataByUnit(IUnitOwner owner, System.Type unit_type);
        public UnitData GetUnitData(IUnitOwner owner, System.Type data_type);

        public UnitInteractions InteractionsHandler { get; }

        public IUnit[] UnitsExecutingCommand { get; }
    }
}
