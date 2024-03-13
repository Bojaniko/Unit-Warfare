using UnityEngine;

namespace UnitWarfare.Core
{
    public class GameEncapsulatedMonoBehaviour : EncapsulatedMonoBehaviour
    {
        private readonly IGame m_game;
        public IGame Game => m_game;

        public GameEncapsulatedMonoBehaviour(IGame game, GameObject game_object) : base(game_object)
        {
            m_game = game;
        }
    }
}
