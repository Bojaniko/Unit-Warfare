using UnitWarfare.Units;
using UnitWarfare.Core.Global;

using Photon.Pun;

namespace UnitWarfare.Players
{
    public class PlayerNetwork : Player
    {
        public override event PlayerEventHandler OnExplicitMoveEnd;

        private readonly PhotonView network_view;

        public PlayerNetwork(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {
            network_view = emb.gameObject.AddComponent<PhotonView>();
            //network_view.ViewID = GlobalValues.NETWORK_PLAYER_VIEW_ID;
        }

        public void SendUnitCommand()
        {
            // TODO: Sending command from local palyer
        }

        public void ReceiveUnitCommand()
        {
            // TODO: Receiving commands from network player
        }
    }
}
