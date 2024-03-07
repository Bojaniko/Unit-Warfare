using Enum = System.Enum;

namespace UnitWarfare.Units
{
	public class UnitCommand<CommandOrders> : IUnitCommand
		where CommandOrders : Enum
	{
		private readonly CommandOrders m_order;
		public object Order => m_order;

		private readonly UnitTarget m_target;
		public UnitTarget Target => m_target;

		public object OrderRef => m_order;
		public byte OrderId => (byte)(object)m_order;

		public UnitCommand(CommandOrders order, UnitTarget target)
		{
			m_order = order;
			m_target = target;
		}

		public UnitCommand(byte order, UnitTarget target)
		{
			m_order = (CommandOrders)(object)order;
			m_target = target;
		}

		public override string ToString() =>
			$"Command order is {m_order}.";
	}
}