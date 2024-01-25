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

        protected EncapsulatedMonoBehaviour emb;

        public delegate void PlayerEventHandler(Player player);

        public abstract event PlayerEventHandler OnExplicitMoveEnd;

        public event IUnitTeamManager.UnitOwnerEventHandler OnRoundStarted;

        protected readonly IPlayerHandler handler;

        protected Player(PlayerData data, IPlayerHandler handler)
        {
            _data = data;
            _isActive = false;

            this.handler = handler;

            handler.OnActivePlayerChanged += ActivePlayerHandler;

            emb = new(new(ToString()));
            emb.OnUpdate += OnUpdate;
        }

        private void ActivePlayerHandler(Player player)
        {
            if (player.Equals(this))
            {
                _isActive = true;
                OnRoundStarted?.Invoke();
                OnActiveTurn();
                return;
            }
            if (!_isActive)
                return;
            OnInactiveTurn();
            _isActive = false;
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