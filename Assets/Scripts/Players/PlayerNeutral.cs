using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitWarfare.Players
{
    public class PlayerNeutral : Player
    {
        public PlayerNeutral(PlayerData data, ref ActivePlayerEventHandler active_player_handler)
            : base(data, ref active_player_handler)
        {

        }

        public override event PlayerEventHandler OnExplicitMoveEnd;
    }
}