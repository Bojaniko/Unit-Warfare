using EventSystem = UnityEngine.EventSystems.EventSystem;
using UnityEngine.InputSystem;

namespace UnitWarfare.Input
{
    public class UserInterfaceInputTracker
    {
        private bool _inputed = false;

        public event System.Action<bool> OnUIInteraction;
        private readonly EventSystem _eventSystem;

        public UserInterfaceInputTracker(InputAction tap, EventSystem engine_events)
        {
            _eventSystem = engine_events;
            _eventSystem.IsPointerOverGameObject();

            tap.performed += HandleTap;
        }

        private void HandleTap(InputAction.CallbackContext context)
        {
            _inputed = !_inputed;
            if (_inputed && _eventSystem.IsPointerOverGameObject())
                OnUIInteraction?.Invoke(true);
            else if (_inputed)
                OnUIInteraction?.Invoke(false);
        }
    }
}