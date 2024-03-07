using UnitWarfare.Territories;

using UnitWarfare.Core.Global;

using Type = System.Type;
using Serializable = System.SerializableAttribute;
using Encoding = System.Text.Encoding;
using InvalidNetworkEventArgumentException = UnitWarfare.Network.InvalidNetworkEventArgumentException;

namespace UnitWarfare.Units
{
	[Serializable]
	public class NetworkUnitCommand
	{
		public byte Order { get; set; }
		public byte TargetTerritoryID { get; set; }
		public byte CallerTerritoryID { get; set; }
		public string CommandType { get; set; }

		public NetworkUnitCommand() { }

		public NetworkUnitCommand(IUnit caller, IUnitCommand command)
		{
			Order = command.OrderId;
			TargetTerritoryID = command.Target.Territory.ID;
			CallerTerritoryID = caller.OccupiedTerritory.ID;
			CommandType = command.GetType().GetGenericArguments()[0].AssemblyQualifiedName;
		}

		/// <exception cref="InvalidNetworkEventArgumentException">if the arguments from the network command are invalid.</exception>
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
				throw new InvalidNetworkEventArgumentException($"Territory with specified ID ({network_command.TargetTerritoryID}) from network command not found.");

			Type orderType = Type.GetType(network_command.CommandType);
			Type commandType = typeof(UnitCommand<>).MakeGenericType(orderType);
			if (commandType == null)
				throw new InvalidNetworkEventArgumentException($"There is no command of type {network_command.CommandType}.");
			UnitTarget target = new(territory);
			return (IUnitCommand)System.Activator.CreateInstance(commandType, network_command.Order, target);
		}

		public static byte[] Serialize(object obj)
		{
			NetworkUnitCommand ncu = (NetworkUnitCommand)obj;
			byte[] commandType = Encoding.ASCII.GetBytes(ncu.CommandType);
			byte[] data = new byte[3 + commandType.Length];
			data[0] = ncu.CallerTerritoryID;
			data[1] = ncu.TargetTerritoryID;
			data[2] = ncu.Order;
			System.Array.Copy(commandType, 0, data, 3, commandType.Length);
			return data;
		}

		public static object Deserialize(byte[] data)
        {
			NetworkUnitCommand ncu = new();
			ncu.CallerTerritoryID = data[0];
			ncu.TargetTerritoryID = data[1];
			ncu.Order = data[2];
			byte[] typeBytes = new byte[data.Length - 3];
			for (int i = 3; i < data.Length; i++)
            {
				typeBytes[i - 3] = data[i];
            }
			ncu.CommandType = Encoding.ASCII.GetString(typeBytes);
			return ncu;
        }
	}
}