using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Territories;

namespace UnitWarfare.Cameras
{
    public class SelectionTarget
    {
        public IUnit Unit => _target.Occupant as IUnit;
        public IActiveUnit ActiveUnit => _target.Occupant as IActiveUnit;

        private readonly Territory _target;
        public Territory Territory => _target;

        private readonly Vector2 _position;
        public Vector2 Position => _position;

        public SelectionTarget(Territory target, Vector2 position)
        {
            _target = target;
            _position = position;
        }
    }
}