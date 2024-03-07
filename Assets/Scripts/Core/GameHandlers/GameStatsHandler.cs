using UnitWarfare.Units;
using UnitWarfare.Players;

namespace UnitWarfare.Core.Handlers
{
    public class GameStatsHandler : GameHandler, IGameStats
    {
        private readonly LevelData _level;
        public int MaxManpower => _level.MaxManpower;

        public GameStatsHandler(LevelData level, IGameStateHandler game_state_handler) : base(game_state_handler)
        {
            _level = level;
            m_playerLocalManpower = _level.MaxManpower;
            m_playerOtherManpower = _level.MaxManpower;
        }

        private int m_playerLocalManpower;
        public int PlayerLocalManpower => m_playerLocalManpower;

        private int m_playerOtherManpower;
        public int PlayerOtherManpower => m_playerOtherManpower;

        public event System.Action<int> OnPlayerLocalPointsChanged;
        public event System.Action<int> OnPlayerOtherPointsChanged;

        private PlayersHandler _players;

        protected override void Initialize()
        {
            _players = gameStateHandler.GetHandler<PlayersHandler>();

            gameStateHandler.GetHandler<UnitsHandler>().OnUnitSpawned += (unit) =>
                ReducePlayerManpower(unit.Owner as Player, unit.Data.Manpower);

            gameStateHandler.GetHandler<UnitsHandler>().OnUnitDespawned += (unit) =>
                ReducePlayerManpower(unit.Owner as Player, unit.Data.Manpower);
        }

        private void ReducePlayerManpower(Player player, int amount)
        {
            if (_players.LocalPlayer.Equals(player))
            {
                m_playerOtherManpower -= amount;
                if (m_playerOtherManpower < 0)
                    m_playerOtherManpower = 0;
                OnPlayerLocalPointsChanged?.Invoke(amount);
            }
            else if (_players.OtherPlayer.Equals(player))
            {
                m_playerLocalManpower -= amount;
                if (m_playerLocalManpower < 0)
                    m_playerLocalManpower = 0;
                OnPlayerOtherPointsChanged?.Invoke(amount);
            }
        }
    }
}