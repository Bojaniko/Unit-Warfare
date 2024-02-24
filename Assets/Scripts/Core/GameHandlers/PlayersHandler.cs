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
        private readonly Player m_localPlayer;
        public Player LocalPlayer => m_localPlayer;

        private readonly Player m_otherPlayer;
        public Player OtherPlayer => m_otherPlayer;

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

            m_localPlayer = GeneratePlayerOne();
            m_localPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);
            m_otherPlayer = GeneratePlayerTwo();
            m_otherPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);
            m_neutralPlayer = new PlayerNeutral(_config.NeutralPlayer, this);

            game_state_handler.OnPlayGameStateChanged += (state) =>
            {
                if (state.Equals(PlayingGameState.PLAYING))
                    StartMatch();
            };
        }

        protected abstract void StartMatch();
        protected abstract Player GeneratePlayerOne();
        protected abstract Player GeneratePlayerTwo();
        protected abstract void OnPlayerExplicitMoveEnd(Player player);

        protected override sealed void Initialize()
        {
            _unitsHandler = gameStateHandler.GetHandler<UnitsHandler>();
            ui_matchProgress = gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>();
            SubInitialize();
        }

        protected abstract void SubInitialize();
    }
}