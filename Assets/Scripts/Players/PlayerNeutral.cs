using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayerNeutral : Player
    {
        public override bool IsNeutral => true;

        public PlayerNeutral(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {

        }

        public override event PlayerEventHandler OnExplicitMoveEnd;
    }
}