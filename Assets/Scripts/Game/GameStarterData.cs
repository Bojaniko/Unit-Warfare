using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Core;

namespace UnitWarfare.Game
{
    [CreateAssetMenu(menuName = "Game/Starter")]
    public class GameStarterData : ScriptableObject
    {
        [SerializeField] private GameData m_gameData;
        public GameData GameData => m_gameData;

        [SerializeField] private AiBrainData[] m_aiBrains;
        public AiBrainData[] AiBrains => m_aiBrains;

        [SerializeField] private LevelData[] m_levelData;
        public LevelData[] Levels => m_levelData;
    }
}