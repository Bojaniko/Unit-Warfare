using UnitWarfare.Core.Global;

namespace UnitWarfare.Core
{
    public interface IGameStateHandler
    {
        public event System.Action<PlayingGameState> OnPlayGameStateChanged;
        public event System.Action<LoadingGameState> OnLoadGameStateChanged;

        public Handler GetHandler<Handler>() where Handler : GameHandler;

        public GameType GameType { get; }
    }
}