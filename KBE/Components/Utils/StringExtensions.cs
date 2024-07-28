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

		public static IEnumerable<string> ToChunks(this string input, int maxChunkSize, IEnumerable<string> filterLeft)
		{
			if (maxChunkSize <= 0)
			{
				throw new IncorrectLengthError($"Chunk size cannot be lower than 1!");
			}
			for (int i = 0; i < input.Length; i += maxChunkSize)
			{
				var currentChunkSize = Math.Min(maxChunkSize, input.Length - i);

				string currentStr = input.Substring(i, currentChunkSize);

				if (currentChunkSize < maxChunkSize)
				{
					yield return currentStr;
					continue;
				}

				if (!filterLeft.Any())
				{
					yield return currentStr;
					continue;
				}
				
				foreach (var filter in filterLeft)
				{
					var pos = currentStr.LastIndexOf(filter);
					if (pos != -1)
					{
						currentChunkSize = pos;
						break;
					}
				}

				if (currentChunkSize != maxChunkSize)
				{
					var newS = input.Substring(i, currentChunkSize + 1);
					i = i + currentChunkSize - maxChunkSize + 1;
					yield return newS;

				}
				else
				{
					yield return currentStr;
				}

			}

		}
	}
}
