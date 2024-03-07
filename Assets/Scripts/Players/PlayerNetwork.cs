using UnitWarfare.Units;
using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

using Photon.Realtime;
using ExitGames.Client.Photon;
using NetworkPlayer = Photon.Realtime.Player;

namespace UnitWarfare.Players
{
    public class PlayerNetwork : Player, IOnEventCallback
    {
        public override event PlayerEventHandler OnExplicitMoveEnd;

        public PlayerNetwork(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {
            
        }

        public record Config(IUnitsHandler Units, ITerritoryHandler Territories, NetworkPlayer NetworkPlayer);
        private Config _config;
        public void Configure(Config config)
        {
            if (_config != null)
                return;
            _config = config;
        }

        protected override void OnActiveTurn()
        {
            if (_config == null)
                throw new UnityEngine.UnityException("Configuration not setup for network player.");
        }

        public void OnEvent(EventData photonEvent)
        {
            if (!photonEvent.Code.Equals(GlobalValues.NETWORK_UNIT_COMMAND_CODE))
                return;
            if (!photonEvent.Sender.Equals(_config.NetworkPlayer.ActorNumber))
                return;
            NetworkUnitCommand nuc = (NetworkUnitCommand)photonEvent.CustomData;
            if (nuc == null)
                return;
            IUnit unit = null;
            foreach (IUnit u in _config.Units.GetUnits(this))
            {
                if (u.OccupiedTerritory.ID == nuc.CallerTerritoryID)
                {
                    unit = u;
                    break;
                }
            }
            if (unit == null)
                return;
            IUnitCommand command = null;
            try
            {
                command = NetworkUnitCommand.GenerateUnitCommand(nuc, _config.Territories);
            }
            catch
            {
                // TODO: Handle this.
                //throw;
            }
            if (command == null)
                return;
            unit.StartCommand(command);
        }
    }
}
