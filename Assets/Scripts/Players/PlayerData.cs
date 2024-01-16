using UnityEngine;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.Players
{
    public class PlayerData
    {
        private readonly string _name;
        public string Name => _name;

        private readonly Color _flagColor;
        public Color FlagColor => _flagColor;

        private readonly PlayerIdentification _identification;
        public PlayerIdentification Identification => _identification;

        // TODO: ADD TERRITORYOWNERTYPE

        public PlayerData(string name, Color flag_color, PlayerIdentification identification)
        {
            _name = name;
            _flagColor = flag_color;
            _identification = identification;
        }

        /// <summary>
        /// The default player
        /// </summary>
        public static PlayerData DefaultPlayer
        {
            get
            {
                return new("Player", Color.green * 1.5f, PlayerIdentification.PLAYER);
            }
        }

        /// <summary>
        /// The default AI controlled enemy
        /// </summary>
        public static PlayerData DefaultEnemy
        {
            get
            {
                return new("Enemy", Color.red * 1.5f, PlayerIdentification.OTHER_PLAYER);
            }
        }

        /// <summary>
        /// The default neutral player
        /// </summary>
        public static PlayerData NeutralPlayer
        {
            get
            {
                return new("Neutral", Color.white * 1.5f, PlayerIdentification.NEUTRAL);
            }
        }
    }
}
