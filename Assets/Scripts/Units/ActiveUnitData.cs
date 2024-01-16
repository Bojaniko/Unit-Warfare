using UnityEngine;

namespace UnitWarfare.Units
{
    public abstract class ActiveUnitData : UnitData
    {
        [SerializeField] private float _speed = 2f;
        public float Speed => _speed;
    }
}
