namespace UnitWarfare.Units
{
    internal interface IUnitInitializer
    {
        public bool InitUnit(IUnit unit);
    }

    internal abstract class UnitInitializer<T, Command> : IUnitInitializer
        where T : IUnit
        where Command : IUnitCommand
    {
        protected readonly IUnitsHandler handler;

        protected UnitInitializer(IUnitsHandler handler)
        {
            this.handler = handler;
        }

        public bool InitUnit(IUnit unit)
        {
            if (!(unit.GetType() is T)
                && !unit.GetType().IsAssignableFrom(typeof(T))
                && unit.GetType().GetInterface(typeof(T).Name) == null)
                return false;
            unit.OnDestroy += HandleUnitDestroy;
            unit.OnCommandStart += HandleUnitCommandStart;
            unit.OnCommandEnd += HandleUnitCommandEnd;

            OnInit((T)unit);

            return true;
        }

        private void HandleUnitDestroy(IUnit unit)
        {
            unit.OnDestroy -= HandleUnitDestroy;
            unit.OnCommandStart -= HandleUnitCommandStart;
            unit.OnCommandEnd -= HandleUnitCommandEnd;

            unit.OccupiedTerritory.SetInteractable(true);
            unit.OccupiedTerritory.Deocuppy();

            OnDestroy((T)unit);
        }

        private void HandleUnitCommandStart(IUnit unit, IUnitCommand command)
        {
            Command com = (Command)command;
            if (com == null)
                return;
            OnCommandStart((T)unit, com);
        }

        private void HandleUnitCommandEnd(IUnit unit, IUnitCommand command)
        {
            Command com = (Command)command;
            if (com == null)
                return;
            OnCommandEnd((T)unit, com);
        }

        protected abstract void OnInit(T unit);
        protected abstract void OnDestroy(T unit);
        protected abstract void OnCommandStart(T unit, Command command);
        protected abstract void OnCommandEnd(T unit, Command command);
    }
}
