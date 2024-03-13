using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace UnitWarfare.Input
{
    public class TapProcessor : InputPostProcessor<TapProcessor.Output>
    {
        public record Config(InputAction TapAction, InputAction TapPosition);
        private readonly Config _config;

        public TapProcessor(Config config)
        {
            _config = config;
            _config.TapAction.performed += (ctx) =>
            {
                if (ctx.interaction.GetType().Equals(typeof(TapInteraction)))
                {
                    SendInput(new Output(_config.TapPosition.ReadValue<Vector2>()));
                }
            };
        }

        public sealed class Output : IInputPostProcessorOutput
        {
            private readonly Vector2 _position;
            public Vector2 Position => _position;

            internal Output(Vector2 position)
            {
                _position = position;
            }
        }
    }
}
