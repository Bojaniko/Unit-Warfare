using System.Collections.Generic;

using UnitWarfare.Core;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Test
{
    public class TestGameStateHandler : IGameStateHandler
    {
        private List<GameHandler> _gameHandlers;

        public GameType TypeOfGame => GameType.TEST;
        public event IGameStateHandler.GameStateEventHandler OnGameStateChanged;
        public event IGameStateHandler.PlayGameStateEventHandler OnPlayGameStateChanged;
        public event IGameStateHandler.LoadGameStateEventHandler OnLoadGameStateChanged;

        public void SetGameState(GameState state)
        {
            OnGameStateChanged?.Invoke(state);
        }

        public Handler GetHandler<Handler>() where Handler : GameHandler
        {
            foreach (GameHandler h in _gameHandlers)
            {
                if (h.GetType().Equals(typeof(Handler)))
                    return h as Handler;
            }
            return null;
        }

        public void RegisterGameHandler(GameHandler handler)
        {
            _gameHandlers.Add(handler);
        }
        
        private TestGameStateHandler()
        {
            _gameHandlers = new();
        }

        private static TestGameStateHandler s_instance;
        public static TestGameStateHandler Instance => s_instance;

        public static TestGameStateHandler CreateInstance()
        {
            if (s_instance != null)
                return s_instance;
            s_instance = new();
            return s_instance;
        }
    }
}