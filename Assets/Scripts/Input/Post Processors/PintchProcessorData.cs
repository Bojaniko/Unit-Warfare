using UnityEngine;

namespace UnitWarfare.Input
{
    [CreateAssetMenu(menuName = "Input/Pintch")]
    public class PintchProcessorData : ScriptableObject
    {
        [SerializeField, Tooltip("The minimum change in distance between two pintching fingers.")] private float _minPintchTreshold = 0.4f;
        public float MinPintchTreshold => _minPintchTreshold;
    }
}