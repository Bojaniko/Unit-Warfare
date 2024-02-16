using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
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
        private Player m_localPlayer;
        public Player LocalPlayer => m_localPlayer;

        private Player m_otherPlayer;
        public Player OtherPlayer => m_otherPlayer;

        private Player m_neutralPlayer;
        public Player NeutralPlayer => m_neutralPlayer;

        public Player GetPlayer(PlayerIdentification owner_type)
        {
            switch (owner_type)
            {
                case PlayerIdentification.PLAYER:
                    return LocalPlayer;

                case PlayerIdentification.OTHER_PLAYER:
                    return OtherPlayer;

                default:
                    return NeutralPlayer;
            }
        }

        public abstract event System.Action<Player> OnActivePlayerChanged;

        protected UnitsHandler _unitsHandler;
        protected MatchProgress ui_matchProgress;

        public record Config(MatchData Match, PlayerData PlayerOne, PlayerData PlayerTwo, PlayerData NeutralPlayer);

        private readonly Config _config;

        protected PlayersHandler(Config config, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _config = config;

            game_state_handler.OnPlayGameStateChanged += (state) =>
            {
                if (state.Equals(PlayingGameState.PLAYING))
                    StartMatch();
            };
        }

        protected abstract void StartMatch();
        protected abstract Player GeneratePlayerTwo();
        protected abstract void OnPlayerExplicitMoveEnd(Player player);

        protected override sealed void Initialize()
        {
            InputHandler input = gameStateHandler.GetHandler<InputHandler>();
            _unitsHandler = gameStateHandler.GetHandler<UnitsHandler>();
            ui_matchProgress = gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>();

            PlayerLocal.Config localConfig = new(input.TapInput,
                gameStateHandler.GetHandler<CameraHandler>().MainCamera,
                gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>(),
                gameStateHandler.GetHandler<UIHandler>().GetUIHandler<UnitDisplay>(),
                gameStateHandler.GetHandler<UnitsHandler>());
            m_localPlayer = new PlayerLocal(localConfig, _config.PlayerOne, PlayerIdentification.PLAYER, this);
            m_localPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);

            m_otherPlayer = GeneratePlayerTwo();
            m_otherPlayer.OnExplicitMoveEnd += (Player player) => OnPlayerExplicitMoveEnd(player);

            m_neutralPlayer = new PlayerNeutral(_config.NeutralPlayer, PlayerIdentification.NEUTRAL, this);
        }
    }
}