using UnityEngine;
using UnityEngine.InputSystem;

using UnitWarfare.Core;

namespace UnitWarfare.Input
{
    public class MoveProcessor : InputPostProcessor<MoveProcessor.Output>
    {
        public enum MoveAxis
        {
            HORIZONTAL,
            VERTICAL
        }

        public record Config(InputAction PressAction, InputAction PositionAction, MoveProcessorData Data);
        private readonly Config _config;

        private readonly EncapsulatedMonoBehaviour _emb;

        private bool _pressed;
        private Vector2 _previousPosition;

        public MoveProcessor(Config config)
        {
            _config = config;
            _pressed = false;
            BindInput(_config.PressAction);

            _emb = new(new("INPUT_PROCESSOR_MOVE"));
            _emb.OnUpdate += Update;
        }

        private void BindInput(InputAction hold_action)
        {
            hold_action.performed += (ctx) =>
            {
                _previousPosition = _config.PositionAction.ReadValue<Vector2>();
                _pressed = !_pressed;
            };
        }

        private void Update()
        {
            if (!_pressed)
                return;

            Vector2 position = _config.PositionAction.ReadValue<Vector2>();

            if (position.x > _previousPosition.x + _config.Data.MinDistance)
                SendInput(new Output(1, MoveAxis.HORIZONTAL));
            if (position.x < _previousPosition.x - _config.Data.MinDistance)
                SendInput(new Output(-1, MoveAxis.HORIZONTAL));

            if (position.y > _previousPosition.y + _config.Data.MinDistance)
                SendInput(new Output(1, MoveAxis.VERTICAL));
            if (position.y < _previousPosition.y - _config.Data.MinDistance)
                SendInput(new Output(-1, MoveAxis.VERTICAL));

            _previousPosition = position;
        }

        public sealed class Output : IInputPostProcessorOutput
        {
            private readonly MoveAxis _axis;
            public MoveAxis Axis => _axis;

            private readonly int _direction;
            public int Direction => _direction;

            internal Output(int direction, MoveAxis axis)
            {
                _direction = direction;
                _axis = axis;
            }
        }
    }
}