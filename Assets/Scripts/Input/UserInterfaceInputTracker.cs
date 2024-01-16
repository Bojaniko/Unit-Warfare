using UnityEngine;
using UnityEngine.InputSystem;

using UnitWarfare.Core;

namespace UnitWarfare.Input
{
    public delegate bool UserInterfaceInputEventHandler(Vector2 position);

    public delegate void UserInterfaceInputTrackerEventHandler(bool ui_interaction);

    public class UserInterfaceInputTracker
    {
        private bool _inputing;

        private readonly InputAction _position;

        public event UserInterfaceInputEventHandler OnInput;
        public event UserInterfaceInputTrackerEventHandler OnUIInteractionChanged;

        public UserInterfaceInputTracker(InputAction tap, InputAction position)
        {
            EncapsulatedMonoBehaviour monoBehaviour = new(new GameObject("USER_INTERFACE_INPUT_TRACKER"));
            monoBehaviour.OnUpdate += OnUpdate;

            _inputing = false;

            _position = position;

            tap.performed += (ctx) =>
            {
                _inputing = !_inputing;
            };
        }

        private void OnUpdate()
        {
            if (_inputing)
            {
                bool? inputed = OnInput?.Invoke(_position.ReadValue<Vector2>());
                if (!inputed.HasValue)
                {
                    OnUIInteractionChanged?.Invoke(false);
                    return;
                }
                OnUIInteractionChanged?.Invoke(inputed.Value);
            }
        }
    }
}