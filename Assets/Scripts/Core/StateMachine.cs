namespace Studio28.Utility
{
    public class StateMachine<StateType> where StateType : System.Enum
    {
        public delegate void StateMachineEventHandler(StateType state);
        public event StateMachineEventHandler OnStateChanged;

        public string Name => _name;
        private readonly string _name;

        public float StateStartTime => _stateStartTime;
        private float _stateStartTime;

        public StateType DefaultState => _defaultState;
        private readonly StateType _defaultState;

        public StateType CurrentState => _currentState;
        private StateType _currentState;

        public StateType PreviousState => _previousState;
        private StateType _previousState;

        public StateMachine(string state_machine_name, StateType default_state)
        {
            _name = state_machine_name;

            _defaultState = default_state;

            _currentState = default_state;
            _previousState = default_state;

            _stateStartTime = UnityEngine.Time.time;

            //Debug.Log("Initialized State Machine " + _name);
        }

        public void SetState(StateType movement_state)
        {
            if (!CurrentState.Equals(movement_state))
            {
                _previousState = _currentState;
                _currentState = movement_state;

                _stateStartTime = UnityEngine.Time.time;

                OnStateChanged?.Invoke(movement_state);

                //Debug.Log(_name + " state set to " + CurrentState);
            }
        }

        public override string ToString()
        {
            return _name + ", default state" + DefaultState.ToString(); 
        }
    }
}