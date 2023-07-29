using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Utils
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> Shuffle<T>( this IEnumerable<T> source )
        {
            var rng = new Random();
            return source.OrderBy(a => rng.Next());
        }

        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }
    }
}
