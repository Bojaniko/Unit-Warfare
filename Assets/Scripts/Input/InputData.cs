using UnityEngine;

namespace UnitWarfare.Input
{
    [CreateAssetMenu(menuName = "Input/Handler")]
    public class InputData : ScriptableObject
    {
        [SerializeField] private PintchProcessorData _pintchData;
        public PintchProcessorData PintchData => _pintchData;

        [SerializeField] private MoveProcessorData _moveData;
        public MoveProcessorData MoveData => _moveData;
    }
}