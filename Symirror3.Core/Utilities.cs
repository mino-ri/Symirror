using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Symirror3.Core;

internal static class Utilities
{
    public static IEnumerable<T> Pairwise<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        while (e1.MoveNext() && e2.MoveNext())
        {
            yield return e1.Current;
            yield return e2.Current;
        }
    }
}

public static class PolyhedronUtilities
{
    // キラル多面体のアポロニウス点は決め打ち。自動計算できるようにしたいが存在しない構成もあるので微妙
    private static readonly Dictionary<SymmetrySymbol, (double x, double y, double z)> SnubPoints = new()
    {
        [new(2, 3, 3)] = (0.525731112119134, 0.601500955007546, 0.601500955007546),
        [new(2, (3, 2), (3, 2))] = (0.85065080835204, 0.371748034460184, 0.371748034460184),
        [new(2, 3, 4)] = (0.250971645, 0.275597400, 0.927934647),
        [new(2, 3, 5)] = (0.154168844, 0.174078420, 0.972590500),
        [new(2, 3, (5, 2))] = (0.411726729, -0.455836066, 0.789109993),
        [new(2, 3, (5, 3))] = (0.29295921361776295, -0.7186510734697211, 0.6306469168697171),
        [new(2, (3, 2), (5, 3))] = (0.8063525037986461, -0.3080935421343984, 0.5048504817396278),
        [new(2, 5, (5, 2))] = (0.305307329, 0.243639067, 0.920562000),
        [new(2, 5, (5, 3))] = (0.556042500, 0.185382977, 0.810220361),
        [new(3, 3, (5, 2))] = (0.3434435568433021, 0.3960066862844772, 0.8516015662742817),
        [new(3, 5, (5, 3))] = (0.6159354575903924, -0.391091023905623, 0.6838649889439915),
        [new(3, (5, 2), (5, 3))] = (0.9451044717221586, 0.20088420665992301, 0.2577267410366895),
        [new((3, 2), (3, 2), (5, 2))] = (0.8613633818444203, 0.4623559851773993, -0.2104282951225438),

        [new(2, 2, 3)] = (0.407704830, 0.577367900, 0.707410600),
        [new(2, 2, 4)] = (0.330173900, 0.511891400, 0.793072200),
        [new(2, 2, 5)] = (0.275600284, 0.447595954, 0.850711400),
        [new(2, 2, (5, 2))] = (0.471330822, 0.599416256, 0.646955600),
        [new(2, 2, (5, 3))] = (0.723797739, 0.447400779, 0.525316100),
        [new(2, 2, 6)] = (0.238152936, 0.393012315, 0.888159700),
        [new(2, 2, 7)] = (0.208528623, 0.348602800, 0.913784266),
        [new(2, 2, (7, 2))] = (0.363304049, 0.546076953, 0.754859200),
        [new(2, 2, (7, 3))] = (0.499450862, 0.600404143, 0.624553200),
        [new(2, 2, (7, 4))] = (0.674680300, 0.504504740, 0.538780570),
    };

    private static readonly HashSet<SymmetrySymbol> DirhombicSymbols = new()
    {
        new(2, 3, (5, 2)),
        new(2, 3, (5, 3)),
        new(2, (3, 2), (5, 2)),
        new(2, (3, 2), (5, 3)),
    };

    public static bool TryGetSnubPoint(this SymmetrySymbol symbol, [NotNullWhen(true)] out SphericalPoint result)
    {
        if (SnubPoints.TryGetValue(symbol, out var p))
        {
            result = new(p.x, p.y, p.z)!;
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }

    public static bool TryGetDirhombicPoint(this SymmetrySymbol symbol, [NotNullWhen(true)] out SphericalPoint result)
    {
        if (DirhombicSymbols.Contains(symbol))
        {
            result = new(0.786151378, -0.618033989, 0)!;
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }
}
