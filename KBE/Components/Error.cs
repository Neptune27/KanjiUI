using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components
{
    [Serializable]
    public class IncorrectLengthError : Exception
    {
        public IncorrectLengthError() : base() { }
        public IncorrectLengthError(string message) : base(message) { }
        public IncorrectLengthError(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected IncorrectLengthError(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
