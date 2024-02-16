using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Core.Global;
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

        private readonly PlayerData m_data;
        public PlayerData Data => m_data;

        public string Name => m_data.Name;

        private readonly PlayerIdentification m_identification;
        public PlayerIdentification Identification => m_identification;

        // ##### ? ##### \\

        private bool m_isActive;
        /// <summary>
        /// Is the current players turn this round?
        /// </summary>
        public bool IsActive => m_isActive;

        protected EncapsulatedMonoBehaviour emb;

        public delegate void PlayerEventHandler(Player player);

        public abstract event PlayerEventHandler OnExplicitMoveEnd;

        public event IUnitTeamManager.UnitOwnerEventHandler OnRoundStarted;
        public event IUnitTeamManager.UnitOwnerEventHandler OnRoundEnded;

        protected readonly IPlayersHandler handler;

        protected Player(PlayerData data, PlayerIdentification identification, IPlayersHandler handler)
        {
            m_data = data;
            m_isActive = false;

            m_identification = identification;

            this.handler = handler;

            handler.OnActivePlayerChanged += ActivePlayerHandler;

            emb = new(new(ToString()));
            emb.OnUpdate += OnUpdate;
        }

        private void ActivePlayerHandler(Player player)
        {
            if (player.Equals(this))
            {
                m_isActive = true;
                OnRoundStarted?.Invoke();
                OnActiveTurn();
                return;
            }
            if (!m_isActive)
                return;
            OnRoundEnded?.Invoke();
            OnInactiveTurn();
            m_isActive = false;
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