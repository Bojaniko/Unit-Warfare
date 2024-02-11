using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Players;

namespace UnitWarfare.Game
{
    [CreateAssetMenu(menuName = "Game/Starter")]
    public class GameStarterData : ScriptableObject
    {
        [SerializeField] private GameData m_gameData;
        public GameData GameData => m_gameData;

        [SerializeField] private AIMatch[] m_aiMatches;
        public AIMatch[] AiMatches => m_aiMatches;

        [SerializeField] private LevelData[] m_levelData;
        public LevelData[] Levels => m_levelData;
    }
    
    [System.Serializable]
    public struct AIMatch
    {
        [SerializeField] public AiBrainData AiData;
        [SerializeField] public MatchData MatchData;
    }
}