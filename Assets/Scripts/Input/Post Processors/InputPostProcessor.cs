using System.Collections;
using UnityEngine;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Input
{
    public abstract class InputPostProcessor<Output>
        where Output : IInputPostProcessorOutput
    {
        public delegate void InputPostProcessorEventHandler(Output output);
        public event InputPostProcessorEventHandler OnInput;

        private bool _enabled = true;

        protected InputPostProcessor(ref UserInterfaceInputTrackerEventHandler ui_interaction)
        {
            ui_interaction += (interacting) =>
            {
                _enabled = !interacting;
            };
        }

        protected void SendInput(Output output)
        {
            if (_enabled)
                OnInput?.Invoke(output);
        }
    }

    public interface IInputPostProcessorOutput { }
}