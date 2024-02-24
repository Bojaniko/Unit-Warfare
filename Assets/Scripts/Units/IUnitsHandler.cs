namespace UnitWarfare.Units
{
    public interface IUnitsHandler
    {
        public UnitSpawner Spawner { get; }

        public UnityEngine.Transform UnitContainer { get; }

        public IUnit[] GetUnits(IUnitOwner owner);
        public UnitData GetUnitDataByUnit(IUnitOwner owner, System.Type unit_type);
        public UnitData GetUnitData(IUnitOwner owner, System.Type data_type);

        public UnitInteractions InteractionsHandler { get; }

        public bool UnitExecutingCommand { get; }
    }
}
