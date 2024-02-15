using UnityEngine;

using UnitWarfare.Game;

namespace UnitWarfare.Test
{
    public class TestAiGameStarter : MonoBehaviour
    {
        [SerializeField] private GameStarterData m_data;

        private GameLocal _game;

        private void Awake()
        {
            if (m_data == null)
            {
                Debug.LogError("Game starter data not set for tester.");
                return;
            }

            GameBase.Config configuration = new(m_data.GameData, m_data.AiMatches[0].MatchData, m_data.Levels[0]);
            GameLocal.Config config = new(configuration, m_data.AiMatches[0].AIData);
            _game = new(config);

            _game.Load();
        }
    }
}