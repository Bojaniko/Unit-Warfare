using UnityEngine;
using UnitWarfare.AI;

namespace UnitWarfare.Players
{
    [CreateAssetMenu(menuName = "Players/Match Data")]
    public class MatchData : ScriptableObject
    {
        [SerializeField] private float _maxTurnDuration;
        public float MaxTurnDuration => _maxTurnDuration;

        [SerializeField] private AiBrainData _aiData;
        public AiBrainData AiData => _aiData;
    }
}