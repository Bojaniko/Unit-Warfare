using UnityEngine;

namespace UnitWarfare.Input
{
    [CreateAssetMenu(menuName = "Input/Move")]
    public class MoveProcessorData : ScriptableObject
    {
        [SerializeField] private float _minDistance = 1f;
        public float MinDistance => _minDistance;
    }
}