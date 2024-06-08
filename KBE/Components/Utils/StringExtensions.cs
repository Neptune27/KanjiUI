using KBE.Components.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Utils
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ToChunks(this string input, int maxChunkSize) {
            if (maxChunkSize <= 0)
            {
                throw new IncorrectLengthError($"Chunk size cannot be lower than 1!");
            }
            for (int i = 0; i < input.Length; i += maxChunkSize)
                yield return input.Substring(i, Math.Min(maxChunkSize, input.Length - i));

        }
    }
}
