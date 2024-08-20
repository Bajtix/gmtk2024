using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions {
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
        return source.Shuffle(new Random());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (rng == null) throw new ArgumentNullException(nameof(rng));

        return source.ShuffleIterator(rng);
    }

    private static IEnumerable<T> ShuffleIterator<T>(
        this IEnumerable<T> source, Random rng) {
        var buffer = source.ToList();
        for (int i = 0; i < buffer.Count; i++) {
            int j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) {
        return source.MinBy(selector, null);
    }

    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> selector, IComparer<TKey> comparer) {
        if (source == null) throw new ArgumentNullException("source");
        if (selector == null) throw new ArgumentNullException("selector");
        comparer ??= Comparer<TKey>.Default;

        using (var sourceIterator = source.GetEnumerator()) {
            if (!sourceIterator.MoveNext()) {
                throw new InvalidOperationException("Sequence contains no elements");
            }
            var min = sourceIterator.Current;
            var minKey = selector(min);
            while (sourceIterator.MoveNext()) {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0) {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }
            return min;
        }
    }
}