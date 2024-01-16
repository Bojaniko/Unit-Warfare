using UnityEngine;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.Core
{
    public interface IGameStateHandler
    {
        public delegate void PlayGameStateEventHandler(PlayingGameState state);
        public event PlayGameStateEventHandler OnPlayGameStateChanged;

        public delegate void LoadGameStateEventHandler(LoadingGameState state);
        public event LoadGameStateEventHandler OnLoadGameStateChanged;

        public delegate void GameStateEventHandler(GameState state);
        public event GameStateEventHandler OnGameStateChanged;

        public GameType TypeOfGame { get; }

        public Handler GetHandler<Handler>() where Handler : GameHandler;
    }
}