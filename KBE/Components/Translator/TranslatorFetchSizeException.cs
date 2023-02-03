using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Translator
{
    [Serializable]
    public class TranslatorFetchSizeException : Exception
    {
        public TranslatorFetchSizeException()
        {
        }

        public TranslatorFetchSizeException(string? message) : base(message)
        {
        }

        public TranslatorFetchSizeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TranslatorFetchSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
