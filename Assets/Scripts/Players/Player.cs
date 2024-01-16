using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Players
{
    public delegate void ActivePlayerEventHandler(Player player);

    public abstract class Player : ITerritoryOwner, IUnitTeamManager
    {
        // ##### DATA ##### \\

        private readonly PlayerData _data;
        public PlayerData Data => _data;

        public string Name => _data.Name;
        public Color FlagColor => _data.FlagColor;

        public PlayerIdentification OwnerIdentification => _data.Identification;

        // ##### ? ##### \\

        private bool _isActive;
        /// <summary>
        /// Is the current players turn this round?
        /// </summary>
        public bool IsActive => _isActive;

        public UnitInteractions InteractionsHandler => throw new System.NotImplementedException();

        protected EncapsulatedMonoBehaviour emb;

        public event IUnitTeamManager.UnitManagerEventHandler OnRoundStarted;
        public event IUnitTeamManager.UnitManagerEventHandler OnRoundEnded;

        protected Player(PlayerData data, ref ActivePlayerEventHandler active_player_event)
        {
            _data = data;
            _isActive = false;

            active_player_event += (player) =>
            {
                if (player.Equals(this))
                {
                    _isActive = true;
                    OnActiveTurn();
                    OnRoundStarted?.Invoke();
                    return;
                }
                if (!_isActive)
                    return;
                OnInactiveTurn();
                OnRoundEnded?.Invoke();
                _isActive = false;
            };

            emb = new(new(ToString()));
            emb.OnUpdate += OnUpdate;
        }

        /// <summary>
        /// Called when the turn switches to the current player.
        /// </summary>
        protected virtual void OnActiveTurn() { }

        /// <summary>
        /// Called when the player's turn ends.
        /// </summary>
        protected virtual void OnInactiveTurn() { }

        protected virtual void OnUpdate() { }

        public override string ToString()
        {
            return $"PLAYER {Data.Name}";
        }
    }
}