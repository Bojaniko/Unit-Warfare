using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Players;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Game
{
    public class GameIdentifier : MonoBehaviour
    {
        [SerializeField] private GameType m_typeOfGame = GameType.PLAYER_V_COMPUTER;
        public void SetGameType(GameType game_type) => m_typeOfGame = game_type;

        [SerializeField] private GameData m_gameData;
        public void SetGameData(GameData game_data) => m_gameData = game_data;

        [SerializeField] private AiBrainData m_aiData;
        public void SetAiBrainData(AiBrainData ai_data) => m_aiData = ai_data;

        [SerializeField] private MatchData m_matchData;
        public void SetMatchData(MatchData match_data) => m_matchData = match_data;
    }
}
