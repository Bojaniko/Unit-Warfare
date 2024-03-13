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
        public event System.Action<Output> OnInput;

        protected void SendInput(Output output)
        {
            if (_enabled)
                OnInput?.Invoke(output);
        }

        private bool _enabled = false;
        public void Enable() => _enabled = true;
        public void Disable() => _enabled = false;
    }

    public interface IInputPostProcessorOutput { }
}