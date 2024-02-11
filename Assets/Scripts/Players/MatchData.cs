using UnityEngine;
using UnitWarfare.Units;

namespace UnitWarfare.Players
{
    [CreateAssetMenu(menuName = "Players/Match Data")]
    public class MatchData : ScriptableObject
    {
        [SerializeField] private float m_maxTurnDuration;
        public float MaxTurnDuration => m_maxTurnDuration;
    }
}