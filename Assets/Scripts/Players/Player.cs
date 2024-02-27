using UnitWarfare.Core;
using UnitWarfare.Units;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Players
{
    public abstract class Player : IUnitOwner
    {
        // ##### DATA ##### \\

        private readonly PlayerData m_data;
        public PlayerData Data => m_data;
        public string Name => m_data.Name;

        // ##### ? ##### \\

        public int Points { get; }

        private bool m_isActive;
        /// <summary>
        /// Is the current players turn this round?
        /// </summary>
        public bool IsActive => m_isActive;

        public UnitsData UnitsData => Data.Nation.Units;

        public virtual bool IsNeutral => false;

        protected EncapsulatedMonoBehaviour emb;

        public delegate void PlayerEventHandler(Player player);

        public abstract event PlayerEventHandler OnExplicitMoveEnd;

        public event IUnitOwner.UnitOwnerEventHandler OnRoundStarted;
        public event IUnitOwner.UnitOwnerEventHandler OnRoundEnded;

        protected readonly IPlayersHandler handler;

        /// <summary>
        /// Player classes must have only the PlayerData and IPlayersHandler argumetns (in order).
        /// </summary>
        protected Player(PlayerData data, IPlayersHandler handler)
        {
            m_data = data;
            m_isActive = false;

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