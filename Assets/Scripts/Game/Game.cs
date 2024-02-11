using System.Collections.Generic;

using UnityEngine;

using Studio28.Utility;

using UnitWarfare.AI;
using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Players;
using UnitWarfare.Core.Enums;
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
        private readonly Game m_game;
        public Game Game => m_game;

        public GameEMB(Game game, GameObject game_object) : base(game_object)
        {
            m_game = game;
        }
    }

    public class Game : IGameStateHandler
    {
        public record Config(GameData Data, MatchData Match, LevelData Level);
        public record PvEConfig(Config Config, AiBrainData AiBrain);

        private readonly Config _config;

        private readonly AiBrainData _aiData;

        private readonly GameEMB m_emb;
        public GameEMB EMB => m_emb;

        public Game(PvEConfig config)
        {
            _config = config.Config;
            _aiData = config.AiBrain;
            _typeOfGame = GameType.PLAYER_V_COMPUTER;

            m_emb = new(this, new("GAME"));

            InitStateControllers();
        }

        private void InitStateControllers()
        {
            m_gameStateController = new("Game State Controller", GameState.LOADING);
            m_gameStateController.OnStateChanged += (state) => { OnGameStateChanged?.Invoke(state); };

            m_loadStateController = new("Load Game State Controller", LoadingGameState.PRE);
            m_loadStateController.OnStateChanged += (state) => { OnLoadGameStateChanged?.Invoke(state); };

            m_playStateController = new("Play Game State Controller", PlayingGameState.LOADING);
            m_playStateController.OnStateChanged += (state) => { OnPlayGameStateChanged?.Invoke(state); };
        }

        private readonly GameType _typeOfGame;
        public GameType TypeOfGame => _typeOfGame;

        private StateMachine<LoadingGameState> m_loadStateController;
        private StateMachine<PlayingGameState> m_playStateController;
        private StateMachine<GameState> m_gameStateController;

        public event IGameStateHandler.GameStateEventHandler OnGameStateChanged;
        public event IGameStateHandler.PlayGameStateEventHandler OnPlayGameStateChanged;
        public event IGameStateHandler.LoadGameStateEventHandler OnLoadGameStateChanged;

        public void Load()
        {
            if (!m_loadStateController.CurrentState.Equals(LoadingGameState.PRE))
                return;

            InitGameHandlers();

            m_loadStateController.SetState(LoadingGameState.LOAD);

            m_loadStateController.SetState(LoadingGameState.POST);

            m_loadStateController.SetState(LoadingGameState.FINAL);

            m_playStateController.SetState(PlayingGameState.PLAYING);
        }

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
            if (playersHandler == null)
                throw new UnityException("Failed to generate players handler.");
            _gameHandlers.Add(playersHandler);

            TerritoryManager territoryHandler = new(_config.Data.MapData, this);
            _gameHandlers.Add(territoryHandler);

            UnitsHandler unitsHandler = new(_config.Data.Combinations, this);
            _gameHandlers.Add(unitsHandler);
        }

        // TODO: Get player name from Google Play Services
        // TODO: Network players
        // TODO: Random nation
        // TODO: Neutral nation
        private PlayersHandler GeneratePlayersHandler()
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
            return null;
        }

        public Handler GetHandler<Handler>() where Handler : GameHandler
        {
            foreach (GameHandler handler in _gameHandlers)
            {
                if (handler.GetType().Equals(typeof(Handler)))
                    return (Handler)handler;
            }
            return null;
        }
    }
}