using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Units
{
    internal class ActiveUnitInitializer : UnitInitializer<IActiveUnit, UnitCommand<ActiveCommandOrder>>
    {
        public ActiveUnitInitializer(IUnitsHandler handler) : base(handler)
        {

        }

        protected override void OnCommandEnd(IActiveUnit unit, UnitCommand<ActiveCommandOrder> command)
        {
            if (command.Order.Equals(ActiveCommandOrder.MOVE))
                command.Target.Territory.SetInteractable(true);
        }

        protected override void OnCommandStart(IActiveUnit unit, UnitCommand<ActiveCommandOrder> command)
        {
            if (command.Order.Equals(ActiveCommandOrder.MOVE))
                command.Target.Territory.SetInteractable(false);
        }

        protected override void OnDestroy(IActiveUnit unit)
        {
            unit.OnAttack -= HandleActiveUnitAttack;
            unit.OnMove -= HandleActiveUnitMove;
            unit.OnJoin -= HandleActiveUnitJoin;
        }

        protected override void OnInit(IActiveUnit unit)
        {
            unit.OnAttack += HandleActiveUnitAttack;
            unit.OnMove += HandleActiveUnitMove;
            unit.OnJoin += HandleActiveUnitJoin;
        }

        private void HandleActiveUnitAttack(IActiveUnit unit, UnitTarget target)
        {
            target.Unit.Damage(unit.Attack);
        }

        private void HandleActiveUnitMove(IActiveUnit unit, UnitTarget target)
        {
            unit.OccupiedTerritory.Deocuppy();
            unit.SetOccupiedTerritory(target.Territory);
            target.Territory.Occupy(unit);
        }

        private void HandleActiveUnitJoin(IActiveUnit unit, UnitTarget target)
        {
            System.Type result = handler.InteractionsHandler.CombinationsManager.GetResult(unit.GetType(), target.Unit.GetType());
            Territory territory = target.Territory;
            handler.Spawner.Despawn(unit);
            handler.Spawner.Despawn(target.Unit);
            handler.Spawner.Spawn(territory, result);
        }
    }
}