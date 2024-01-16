namespace UnitWarfare.Units
{
    public class UnitCommand<CommandOrders> : IUnitCommand
        where CommandOrders : System.Enum
    {
        private readonly CommandOrders _order;
        public CommandOrders Order => _order;

        private readonly UnitTarget _target;
        public UnitTarget Target => _target;

        private readonly int _orderValue;
        public int OrderValue => _orderValue;

        public UnitCommand(CommandOrders order, UnitTarget target)
        {
            _order = order;
            _target = target;
            _orderValue = (int)((object)_order);
        }
    }
}