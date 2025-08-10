using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Utils;

public static class EnumerableExtension
{
    public static IList<T> Shuffle<T>( this IList<T> source )
    {
        var rng = new Random();
        return [.. source.OrderBy(a => rng.Next())];
    }

    public static T PickRandom<T>(this IList<T> source)
    {
        var rng = new Random();
        var randomInt = rng.Next(source.Count);

        return source[randomInt];
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        var rng = new Random();

        var sourceList = source.ToList();
        var randomInt = rng.Next(sourceList.Count);

        return sourceList[randomInt];
    }


    public static T PickWithout<T>(this IList<T> source, T item)
    {

        if (item is null)
        {
            return source.PickRandom();
        }


        if (source.Count == 1)
        {
            return source.PickRandom();
        }

        var randomItem = item;
        while (item.Equals(randomItem))
        {
            randomItem = source.PickRandom();
        }

        return randomItem;
    }

    public static IList<T> PickWithoutRange<T>(this IList<T> source, int total, T item)
    {

        if (item is null)
        {
            return source.PickRandom(total);
        }


        if (source.Count < total)
        {
            throw new IncorrectLengthError($"Source only have {source.Count} but need {total}");
        }


        // If source totals is very small, then it will shuffle and take from the list, or
        // else it will pick by choosing a random number. This is to not take too long to 
        // get random amounts if the total is too low and not too ram intensive if it's too high
        if (source.Count < total * 3)
        {
            return source.PickRandom(total);
        }

        IList<T> randomItems = [];
        while (randomItems.Count < total)
        {
            var randomItem = source.PickRandom();

            if (randomItems.Contains(randomItem) || randomItem.Equals(item))
            {
                continue;
            }

            randomItems.Add(randomItem);
        }

        return randomItems;
    }

    public static IList<T> PickRandom<T>(this IList<T> source, int count)
    {
        return [.. source.Shuffle().Take(count)];
    }

    public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            source.Add(item);
        }
        ;
    }
}
