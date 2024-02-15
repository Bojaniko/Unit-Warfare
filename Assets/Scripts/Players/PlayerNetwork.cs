using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayerNetwork : Player
    {
        public override event PlayerEventHandler OnExplicitMoveEnd;

        public PlayerNetwork(PlayerData data, PlayerIdentification identification, IPlayerHandler handler)
            : base(data, identification, handler)
        {

        }
    }
}
