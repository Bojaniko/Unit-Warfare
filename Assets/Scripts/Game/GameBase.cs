using System.Collections.Generic;

using UnityEngine;

using Studio28.Utility;

using Photon.Realtime;

using UnitWarfare.AI;
using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Players;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}


namespace UnitWarfare.Game
{
    public class GameEMB : EncapsulatedMonoBehaviour
    {
        private readonly GameBase m_game;
        public GameBase Game => m_game;

        public GameEMB(GameBase game, GameObject game_object) : base(game_object)
        {
            m_game = game;
        }
    }

    public abstract class GameBase : IGameStateHandler
    {
        public record Config(GameData Data, MatchData Match, LevelData Level);

        private readonly Config _config;

        private readonly GameEMB m_emb;
        public GameEMB EMB => m_emb;

        protected GameBase(Config config)
        {
            _config = config;
            m_emb = new(this, new("GAME"));
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

        public abstract GameType TypeOfGame { get; }

        private StateMachine<LoadingGameState> stateMachine_load;

        protected StateMachine<PlayingGameState> stateMachine_play;

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

        private List<GameHandler> _gameHandlers;

        private void InitGameHandlers()
        {
            _gameHandlers = new();

            UIHandler uiHandler = new(_config.Data.UIData, this);
            _gameHandlers.Add(uiHandler);

            CameraHandler camHandler = new(_config.Data.CameraData, this);
            _gameHandlers.Add(camHandler);

            InputHandler inputHandler = new(_config.Data.InputData, this);
            _gameHandlers.Add(inputHandler);

            PlayersHandler playersHandler = GeneratePlayersHandler();
            _gameHandlers.Add(playersHandler);

            TerritoryManager territoryHandler = new(_config.Data.MapData, this);
            _gameHandlers.Add(territoryHandler);

            UnitsHandler unitsHandler = new(_config.Data.Combinations, this);
            _gameHandlers.Add(unitsHandler);
        }

        protected abstract PlayersHandler GeneratePlayersHandler();

        // TODO: Get player name from Google Play Services
        // TODO: Network players
        // TODO: Random nation
        // TODO: Neutral nation
        /*private PlayersHandler GeneratePlayersHandler()
        {
            PlayerData neutral = new("Neutral", _config.Data.AllyNation);
            if (TypeOfGame.Equals(GameType.PLAYER_V_COMPUTER))
            {
                PlayerData playerOne = new("Player", _config.Data.AllyNation);
                PlayerData playeTwo = new("Bot", _config.Data.AxisNation);
                PlayersHandler.Config config = new(_config.Match, playerOne, playeTwo, neutral);
                PlayersHandler.PvEConfig pveConfig = new PlayersHandler.PvEConfig(config, _aiData);
                PlayersHandler playersHandler = new(pveConfig, this);
                return playersHandler;
            }
            if (TypeOfGame.Equals(GameType.PLAYER_V_PLAYER))
            {
                PlayerData playerOne = new("Player", _config.Data.AllyNation);
                PlayerData playeTwo = new(_networkPlayer.NickName, _config.Data.AxisNation);
                PlayersHandler.Config config = new(_config.Match, playerOne, playeTwo, neutral);
                PlayersHandler.PvEConfig pveConfig = new PlayersHandler.PvEConfig(config, _aiData);
                PlayersHandler playersHandler = new(pveConfig, this);
                return playersHandler;
            }
            throw new UnityException("Invalid type of game.");
        }*/

        public Handler GetHandler<Handler>() where Handler : GameHandler
        {
            foreach (GameHandler handler in _gameHandlers)
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