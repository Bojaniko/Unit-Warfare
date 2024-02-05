namespace UnitWarfare.Units
{
    public class UnitCommand<CommandOrders> : IUnitCommand
        where CommandOrders : System.Enum
    {
        private readonly CommandOrders _order;
        public object Order => _order;

        private readonly UnitTarget _target;
        public UnitTarget Target => _target;

        public object OrderRef => _order;

        public UnitCommand(CommandOrders order, UnitTarget target)
        {
            _order = order;
            _target = target;
        }

        public override string ToString() =>
            $"Command order is {_order}.";
    }
}