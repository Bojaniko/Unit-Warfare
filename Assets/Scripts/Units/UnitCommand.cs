using Enum = System.Enum;
using Type = System.Type;

using UnitWarfare.Territories;
using InvalidNetworkEventRagumentException = UnitWarfare.Network.InvalidNetworkEventArgumentException;

namespace UnitWarfare.Units
{
	public class NetworkUnitCommand
	{
		public byte Order { get; set; }
		public byte TargetTerritoryID { get; set; }
		public byte CallerTerritoryID { get; set; }
		public string CommandType { get; set; }

		public NetworkUnitCommand(IUnit caller, IUnitCommand command)
		{
			Order = command.OrderId;
			TargetTerritoryID = command.Target.Territory.ID;
			CallerTerritoryID = caller.OccupiedTerritory.ID;
			CommandType = command.GetType().Name;
		}
		
		/// <exception cref="InvalidNetworkEventRagumentException">if the arguments from the network command are invalid.</exception>
		public static IUnitCommand GenerateUnitCommand(NetworkUnitCommand network_command, ITerritoryHandler territory_handler)
		{
			Territory territory = null;
			foreach (Territory t in territory_handler.Territories)
			{
				if (t.ID == network_command.TargetTerritoryID)
				{
					territory = t;
					break;
				}
			}
			if (territory == null)
				throw new InvalidNetworkEventRagumentException($"Territory with specified ID ({network_command.TargetTerritoryID}) from network command not found.");

			Type commandType = Type.GetType(network_command.CommandType);
			if (commandType == null)
				throw new InvalidNetworkEventRagumentException($"There is no command of type {network_command.CommandType}.");

			return (IUnitCommand)System.Activator.CreateInstance(commandType, network_command.Order, territory);
		}
	}

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