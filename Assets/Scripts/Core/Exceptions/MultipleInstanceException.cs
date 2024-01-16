using UnityEngine;

namespace UnitWarfare.Core.Exceptions
{
    public class MultipleInstanceException : UnityException
    {
        public MultipleInstanceException() : base("An attempt was made to create two instances of a class that supports only one instance!") { }

        public MultipleInstanceException(string message) : base(message) { }

        public MultipleInstanceException(string message, System.Exception inner_exception) : base(message, inner_exception) { }
    }
}
