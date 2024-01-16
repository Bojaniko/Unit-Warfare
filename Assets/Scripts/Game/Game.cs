using System.Collections.Generic;

using UnityEngine;

using Studio28.Utility;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Players;
using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Game
{
    public class Game : MonoBehaviour, IGameStateHandler
    {
        [SerializeField] private GameData _gameData;

        public GameType TypeOfGame => GameType.PLAYER_V_COMPUTER;

        private StateMachine<LoadingGameState> _loadStateController;
        private StateMachine<PlayingGameState> _playStateController;
        private StateMachine<GameState> _gameStateController;

        public event IGameStateHandler.GameStateEventHandler OnGameStateChanged;
        public event IGameStateHandler.PlayGameStateEventHandler OnPlayGameStateChanged;
        public event IGameStateHandler.LoadGameStateEventHandler OnLoadGameStateChanged;

        private void Awake()
        {
            _gameStateController = new("Game State Controller", GameState.LOADING);
            _gameStateController.OnStateChanged += (state) => { OnGameStateChanged?.Invoke(state); };

            _loadStateController = new("Load Game State Controller", LoadingGameState.PRE);
            _loadStateController.OnStateChanged += (state) => { OnLoadGameStateChanged?.Invoke(state); };

            _playStateController = new("Play Game State Controller", PlayingGameState.LOADING);
            _playStateController.OnStateChanged += (state) => { OnPlayGameStateChanged?.Invoke(state); };

            InitGameHandlers();

            _loadStateController.SetState(LoadingGameState.LOAD);

            _loadStateController.SetState(LoadingGameState.POST);

            _loadStateController.SetState(LoadingGameState.FINAL);

            _playStateController.SetState(PlayingGameState.PLAYING);
        }

        private List<GameHandler> _gameHandlers;

        private void InitGameHandlers()
        {
            _gameHandlers = new();

            UIHandler uiHandler = new(this);
            _gameHandlers.Add(uiHandler);

            CameraHandler camHandler = new(_gameData.CameraData, this);
            _gameHandlers.Add(camHandler);

            InputHandler inputHandler = new(_gameData.InputData, this);
            _gameHandlers.Add(inputHandler);

            // TODO : Get player data
            PlayersGameData playersData = new(_gameData.MatchData, PlayerData.DefaultPlayer, PlayerData.DefaultEnemy);
            PlayersHandler playersHandler = new(playersData, this);
            _gameHandlers.Add(playersHandler);

            TerritoryManager territoryHandler = new(_gameData.MapData, this);
            _gameHandlers.Add(territoryHandler);

            UnitsHandler unitsHandler = new(_gameData.UnitsData, this);
            _gameHandlers.Add(unitsHandler);
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