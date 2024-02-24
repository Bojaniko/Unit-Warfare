using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Units
{
    internal class AntennaeInitializer : UnitInitializer<Antennae, UnitCommand<AntennaeCommandOrder>>
    {
        public AntennaeInitializer(IUnitsHandler handler) : base(handler)
        {

        }

        private void HandleAntennaeReinforcements(Territory territory)
        {
            handler.Spawner.Spawn(territory, typeof(Recruit));
        }

        protected override void OnCommandEnd(Antennae unit, UnitCommand<AntennaeCommandOrder> command)
        {

        }

        protected override void OnCommandStart(Antennae unit, UnitCommand<AntennaeCommandOrder> command)
        {

        }

        protected override void OnDestroy(Antennae unit)
        {
            unit.OnReinforce -= HandleAntennaeReinforcements;
        }

        protected override void OnInit(Antennae unit)
        {
            unit.OnReinforce += HandleAntennaeReinforcements;
        }
    }
}