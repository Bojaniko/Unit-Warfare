using UnityEngine;

using UnitWarfare.Game;

namespace UnitWarfare.Test
{
    public class TestAiGameStarter : MonoBehaviour
    {
        [SerializeField] private GameStarterData m_data;

        private Game.Game _game;

        private void Awake()
        {
            if (m_data == null)
            {
                Debug.LogError("Game starter data not set for tester.");
                return;
            }

            Game.Game.Config config = new(m_data.GameData, m_data.AiMatches[0].MatchData, m_data.Levels[0]);
            Game.Game.PvEConfig pveConfig = new(config, m_data.AiMatches[0].AIData);
            _game = new(pveConfig);

            _game.Load();
        }
    }
}