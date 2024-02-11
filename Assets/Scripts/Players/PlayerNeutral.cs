using UnitWarfare.Core.Enums;

namespace UnitWarfare.Players
{
    public class PlayerNeutral : Player
    {
        public PlayerNeutral(PlayerData data, PlayerIdentification identification, IPlayerHandler handler)
            : base(data, identification, handler)
        {

        }

        public override event PlayerEventHandler OnExplicitMoveEnd;
    }
}