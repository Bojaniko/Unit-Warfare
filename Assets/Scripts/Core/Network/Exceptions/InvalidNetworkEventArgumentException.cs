using Exception = System.Exception;

namespace UnitWarfare.Network
{
    public class InvalidNetworkEventArgumentException : Exception
    {
        public InvalidNetworkEventArgumentException(string message) : base(message) { }
    }
}
