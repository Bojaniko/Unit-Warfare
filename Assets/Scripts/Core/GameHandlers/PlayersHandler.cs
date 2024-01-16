using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Studio28.Utility;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Players
{
    public sealed class PlayersGameData
    {
        private readonly MatchData _matchData;
        public MatchData Match => _matchData;

        private readonly PlayerData _playerOneData;
        public PlayerData PlayerOneData => _playerOneData;

        private readonly PlayerData _playerTwoData;
        public PlayerData PlayerTwoData => _playerTwoData;

        private readonly PlayerData _neutralPlayerData;
        public PlayerData NeutralPlayerData => _neutralPlayerData;

        public PlayersGameData(MatchData match_data, PlayerData player_one_data, PlayerData player_two_data)
        {
            _matchData = match_data;
            _playerOneData = player_one_data;
            _playerTwoData = player_two_data;
            _neutralPlayerData = PlayerData.NeutralPlayer;
        }
    }

    public enum PlayerHandlerState
    {
        LOADING,
        PAUSED,
        PLAYER_ONE_TURN,
        PLAYER_TWO_TURN
    }

    public class PlayersHandler : GameHandler
    {
        // ##### PLAYERS DATA ##### \\

        private readonly PlayersGameData _playersData;

        private Player _playerOne;
        public Player PlayerOne => _playerOne;

        private Player _playerTwo;
        public Player PlayerTwo => _playerTwo;

        private Player _neutralPlayer;
        public Player NeutralPlayer => _neutralPlayer;

        public Player GetPlayer(PlayerIdentification owner_type)
        {
            switch (owner_type)
            {
                case PlayerIdentification.PLAYER:
                    return PlayerOne;

                case PlayerIdentification.OTHER_PLAYER:
                    return PlayerTwo;

                default:
                    return NeutralPlayer;
            }
        }

        // ##### ??? ##### \\

        private event ActivePlayerEventHandler OnActivePlayerChanged;

        private StateMachine<PlayerHandlerState> _stateController;

        private InputHandler _input;

        protected override void Initialize()
        {
            _input = gameStateHandler.GetHandler<InputHandler>();

            PlayerLocal.Config localConfig = new(_input.TapInput,
                gameStateHandler.GetHandler<CameraHandler>().MainCamera,
                gameStateHandler.GetHandler<UIHandler>().GetUIHandler<UnitDisplay>(),
                gameStateHandler.GetHandler<UnitsHandler>());
            _playerOne = new PlayerLocal(localConfig, _playersData.PlayerOneData, ref OnActivePlayerChanged);

            _playerTwo = new PlayerComputer(gameStateHandler.GetHandler<UnitsHandler>(), _playersData.Match.AiData, _playersData.PlayerTwoData, ref OnActivePlayerChanged);

            _neutralPlayer = new PlayerNeutral(_playersData.NeutralPlayerData, ref OnActivePlayerChanged);

            _stateController = new("Players Handler", PlayerHandlerState.LOADING);
        }

        private void Pause()
        {

        }

        private void Resume()
        {
            OnActivePlayerChanged?.Invoke(_playerOne);
            //coroutine_mainLoop = StartCoroutine(MainPlayersLoop());
        }

        // ##### COROUTINES ##### \\

        private Coroutine coroutine_mainLoop;

        private IEnumerator MainPlayersLoop()
        {
            OnActivePlayerChanged?.Invoke(_playerOne);

            yield return new WaitForSeconds(_playersData.Match.MaxTurnDuration);

            OnActivePlayerChanged?.Invoke(_playerTwo);

            yield return new WaitForSeconds(_playersData.Match.MaxTurnDuration);

            yield return MainPlayersLoop();
        }
        
        public PlayersHandler(PlayersGameData players_data, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _playersData = players_data;

            game_state_handler.OnPlayGameStateChanged += (state) =>
            {
                if (state.Equals(PlayingGameState.PLAYING))
                    Resume();
            };
        }
    }
}