using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitWarfare.Players
{
    public class PlayerNeutral : Player
    {
        public PlayerNeutral(PlayerData data, IPlayerHandler handler)
            : base(data, handler)
        {

        }

        public override event PlayerEventHandler OnExplicitMoveEnd;
    }
}