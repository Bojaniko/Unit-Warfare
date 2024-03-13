using UnitWarfare.Core.Global;

namespace UnitWarfare.Core
{
    public interface IGame : IGameStateHandler
    {
        public LoadingGameState LoadingState { get; }
        public PlayingGameState PlayingState { get; }
        public EncapsulatedMonoBehaviour EMB { get; }
        public LevelData Level { get; }
        public void Load();
    }
}
