using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Units
{
    public delegate void UnitAttack(IActiveUnit attacking_unit, IUnit target_unit);

    public delegate void UnitMove(IActiveUnit moving_unit, Territory target_territory);

    public delegate void UnitJoin(IActiveUnit joining_unit, IActiveUnit target_unit);

    public interface IActiveUnit : IUnit
    {
        public UnitCommand<ActiveCommandOrder> CurrentCommand { get; }
        public void StartCommand(UnitCommand<ActiveCommandOrder> command);

        public event UnitAttack OnAttack;

        public event UnitMove OnMove;

        public event UnitJoin OnJoin;
    }
}