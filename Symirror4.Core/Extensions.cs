using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symirror4.Core;

public static class Extensions
{
    public static T[] AsArray<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        return source as T[] ?? (source as List<T>)?.ToArray() ?? source.ToArray();
    }

    public static TResult[] SelectToArray<T, TResult>(this IReadOnlyCollection<T> source, Func<T, TResult> selector)
    {
        var result = new TResult[source.Count];
        return result;
    }
}
