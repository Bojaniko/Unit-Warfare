using System.Collections.Generic;

using Studio28.Utility;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Players;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;
using UnitWarfare.Core.Handlers;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}


namespace UnitWarfare.Game
{
    public abstract class GameBase : IGame
    {
        public record Config(GameData Data, LevelData Level);
        private readonly Config m_config;

        public LevelData Level => m_config.Level;

        private readonly GameEncapsulatedMonoBehaviour m_emb;
        public GameEncapsulatedMonoBehaviour EMB => m_emb;
        EncapsulatedMonoBehaviour IGame.EMB => m_emb;

        public abstract GameType GameType { get; }

        protected GameBase(Config config)
        {
            m_config = config;
            m_emb = new(this, new(GlobalValues.GAME_HANDLER_NAME));
            InitStateControllers();
        }

        public event System.Action<PlayingGameState> OnPlayGameStateChanged;
        public event System.Action<LoadingGameState> OnLoadGameStateChanged;

        private void InitStateControllers()
        {
            stateMachine_load = new("Load Game State Controller", LoadingGameState.PRE);
            stateMachine_load.OnStateChanged += (state) => { OnLoadGameStateChanged?.Invoke(state); };

            stateMachine_play = new("Play Game State Controller", PlayingGameState.LOADING);
            stateMachine_play.OnStateChanged += (state) => { OnPlayGameStateChanged?.Invoke(state); };
        }

        private StateMachine<LoadingGameState> stateMachine_load;
        public LoadingGameState LoadingState => stateMachine_load.CurrentState;

        protected StateMachine<PlayingGameState> stateMachine_play;
        public PlayingGameState PlayingState => stateMachine_play.CurrentState;

        public void Load()
        {
            if (!stateMachine_load.CurrentState.Equals(LoadingGameState.PRE))
                return;

            InitGameHandlers();

            stateMachine_load.SetState(LoadingGameState.LOAD);

            stateMachine_load.SetState(LoadingGameState.POST);

            stateMachine_load.SetState(LoadingGameState.FINAL);

            OnLoadFinished();
        }

        protected abstract void OnLoadFinished();

        private List<GameHandler> m_gameHandlers;

        private void InitGameHandlers()
        {
            m_gameHandlers = new();

            GameStatsHandler statsHandler = new(m_config.Level, this);
            statsHandler.OnPlayerLocalPointsChanged += (amount) =>
                stateMachine_play.SetState(PlayingGameState.ENDED);
            statsHandler.OnPlayerOtherPointsChanged += (amount) =>
                stateMachine_play.SetState(PlayingGameState.ENDED);
            m_gameHandlers.Add(statsHandler);

            UIHandler uiHandler = new(m_config.Data.UIData, this);
            m_gameHandlers.Add(uiHandler);

            CameraHandler camHandler = new(m_config.Data.CameraData, this);
            m_gameHandlers.Add(camHandler);

            InputHandler inputHandler = new(m_config.Data.InputData, this);
            m_gameHandlers.Add(inputHandler);

            TerritoryManager territoryHandler = new(m_config.Data.MapData, this);
            m_gameHandlers.Add(territoryHandler);

            UnitsHandler unitsHandler = new(m_config.Data.Combinations, this);
            m_gameHandlers.Add(unitsHandler);

            PlayersHandler playersHandler = GeneratePlayersHandler();
            m_gameHandlers.Add(playersHandler);
        }

        protected abstract PlayersHandler GeneratePlayersHandler();

        // TODO: Get player name from Google Play Services
        // TODO: Random nation
        // TODO: Neutral nation

        public Handler GetHandler<Handler>() where Handler : GameHandler
        {
            foreach (GameHandler handler in m_gameHandlers)
            {
                if (handler.GetType().Equals(typeof(Handler))
                    || handler.GetType().IsSubclassOf(typeof(Handler))
                    || handler.GetType().IsAssignableFrom(typeof(Handler))
                    || handler.GetType().GetInterface(typeof(Handler).ToString()) != null)
                    return (Handler)handler;
            }
            return null;
        }
    }
}