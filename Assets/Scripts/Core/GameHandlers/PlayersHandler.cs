using UnityException = UnityEngine.UnityException;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Core.Global;

using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Players
{
    public abstract class PlayersHandler : GameHandler, IPlayersHandler
    {
        public abstract Player LocalPlayer { get; }
        public abstract Player OtherPlayer { get; }

        private readonly Player m_neutralPlayer;
        public Player NeutralPlayer => m_neutralPlayer;

        public abstract event System.Action<Player> OnActivePlayerChanged;

        protected UnitsHandler _unitsHandler;
        protected MatchProgress ui_matchProgress;

        public record Config(LevelData Level, PlayerData PlayerLocal, PlayerData PlayerOther, PlayerData NeutralPlayer);
        private readonly Config _config;

        protected PlayersHandler(Config config, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _config = config;
            m_neutralPlayer = new PlayerNeutral(_config.NeutralPlayer, this);
            game_state_handler.OnPlayGameStateChanged += (state) =>
            {
                if (state.Equals(PlayingGameState.PLAYING))
                    StartMatch();
            };
        }

        protected abstract void StartMatch();
        protected abstract void OnPlayerExplicitMoveEnd(Player player);

        protected override sealed void Initialize()
        {
            if (LocalPlayer == null)
                throw new UnityException("LocalPlayer can't be null.");
            if (OtherPlayer == null)
                throw new UnityException("OtherPlayer can't be null.");
            LocalPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);
            OtherPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);
            _unitsHandler = gameStateHandler.GetHandler<UnitsHandler>();
            ui_matchProgress = gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>();
            SubInitialize();
        }

        protected abstract void SubInitialize();
    }
}