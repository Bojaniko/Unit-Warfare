using UnityEngine;

using Studio28.Utility;

namespace UnitWarfare.Units
{
    internal enum MoverState
    {
        MOVING,
        WAITING
    }

    internal class Mover
    {
        private readonly StateMachine<MoverState> _state;
        public MoverState CurrentState => _state.CurrentState;

        private System.Func<float> _speedMultiplier;

        private float _speed;
        public float Speed => _speed;
            
        private Vector3 _destination;
        private Vector3 _direction;

        private float _deltaDistance = 0f;

        private readonly Transform transform;

        public Mover(UnitEMB emb, float speed)
        {
            _state = new("Unit Mover", MoverState.WAITING);
            transform = emb.transform;
            emb.OnUpdate += Update;
            _speed = speed;
        }

        public void SetSpeed(float speed) =>
            _speed = speed;

        public void SetSpeedMultiplier(System.Func<float> multiplier) =>
            _speedMultiplier = multiplier;

        public void MoveToDestination(Vector3 destination)
        {
            _destination = destination;
            _deltaDistance = Vector3.Distance(transform.position, _destination);
            _direction = Vector3.Normalize(transform.position - _destination);
            _state.SetState(MoverState.MOVING);
        }

        private void Update()
        {
            if (!_state.CurrentState.Equals(MoverState.MOVING))
                return;
            float mult = 1f;
            if (_speedMultiplier != null)
                mult = _speedMultiplier.Invoke();
            transform.position += -_direction * Speed * mult * Time.deltaTime;

            float currentDistance = Vector3.Distance(transform.position, _destination);

            if (currentDistance <= Time.deltaTime || currentDistance > _deltaDistance)
            {
                transform.position = _destination;
                _state.SetState(MoverState.WAITING);
            }

            _deltaDistance = currentDistance;
        }
    }
}
