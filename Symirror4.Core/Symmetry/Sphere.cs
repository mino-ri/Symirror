using System;
using System.Linq;

namespace Symirror4.Core.Symmetry;

/// <summary>球面幾何学に関するユーティリティを提供します。</summary>
public static class Sphere
{
    public static bool CanMakeTriangle(Fraction a, Fraction b, Fraction c)
    {
        var target = new[] { a, b, c }.OrderBy(x => (double)x).ToArray();

        return ValidSymbols.Any(target.SequenceEqual);
    }

    public static int GetSymmetryOrder(Fraction a, Fraction b, Fraction c)
    {
        var target = new[] { a, b, c }.OrderBy(x => x.Numerator).ToArray();

        if (target[0] == 2 && target[1] == 2)
            return target[2].Numerator * 4;

        if (target.Any(x => x.Numerator == 5))
            return 120;

        if (target.Any(x => x.Numerator == 4))
            return 48;

        return 24;
    }

    private static readonly Fraction[][] ValidSymbols = new Fraction[][]
    {
            new Fraction[] {2, 2, 2},
            new Fraction[] {2, 2, 3},
            new Fraction[] {2, 2, new Fraction(3, 2)},
            new Fraction[] {2, 2, 4},
            new Fraction[] {2, 2, new Fraction(4, 3)},
            new Fraction[] {2, 2, 5},
            new Fraction[] {2, 2, new Fraction(5, 2)},
            new Fraction[] {2, 2, new Fraction(5, 3)},
            new Fraction[] {2, 2, new Fraction(5, 4)},
            new Fraction[] {2, 2, 6},
            new Fraction[] {2, 2, 7},
            new Fraction[] {2, 2, new Fraction(7, 2)},
            new Fraction[] {2, 2, new Fraction(7, 3)},
            new Fraction[] {2, 2, new Fraction(7, 4)},
            new Fraction[] {2, 2, new Fraction(7, 5)},
            new Fraction[] {2, 2, new Fraction(7, 6)},

            new Fraction[] {2, 3, 3},
            new Fraction[] {2, 3, new Fraction(3, 2)},
            new Fraction[] {2, new Fraction(3, 2), new Fraction(3, 2)},
            new Fraction[] {3, 3, new Fraction(3, 2)},
            new Fraction[] {new Fraction(3, 2), new Fraction(3, 2), new Fraction(3, 2)},

            new Fraction[] {2, 3, 4},
            new Fraction[] {2, 3, new Fraction(4, 3)},
            new Fraction[] {2, new Fraction(3, 2), 4},
            new Fraction[] {2, new Fraction(3, 2), new Fraction(4, 3)},
            new Fraction[] {3, 4, new Fraction(4, 3)},
            new Fraction[] {new Fraction(3, 2), 4, 4},
            new Fraction[] {new Fraction(3, 2), new Fraction(4, 3), new Fraction(4, 3)},

            new Fraction[] {2, 3, 5},
            new Fraction[] {2, 3, new Fraction(5, 2)},
            new Fraction[] {2, 3, new Fraction(5, 3)},
            new Fraction[] {2, 3, new Fraction(5, 4)},
            new Fraction[] {2, new Fraction(3, 2), 5},
            new Fraction[] {2, new Fraction(3, 2), new Fraction(5, 2)},
            new Fraction[] {2, new Fraction(3, 2), new Fraction(5, 3)},
            new Fraction[] {2, new Fraction(3, 2), new Fraction(5, 4)},
            new Fraction[] {2, 5, new Fraction(5, 2)},
            new Fraction[] {2, 5, new Fraction(5, 3)},
            new Fraction[] {2, new Fraction(5, 2), new Fraction(5, 4)},
            new Fraction[] {2, new Fraction(5, 3), new Fraction(5, 4)},
            new Fraction[] {3, 3, new Fraction(5, 2)},
            new Fraction[] {3, 3, new Fraction(5, 4)},
            new Fraction[] {3, new Fraction(3, 2), 5},
            new Fraction[] {3, new Fraction(3, 2), new Fraction(5, 3)},
            new Fraction[] {new Fraction(3, 2), new Fraction(3, 2), new Fraction(5, 2)},
            new Fraction[] {new Fraction(3, 2), new Fraction(3, 2), new Fraction(5, 4)},
            new Fraction[] {3, 5, new Fraction(5, 3)},
            new Fraction[] {3, 5, new Fraction(5, 4)},
            new Fraction[] {3, new Fraction(5, 2), new Fraction(5, 3)},
            new Fraction[] {3, new Fraction(5, 2), new Fraction(5, 4)},
            new Fraction[] {new Fraction(3, 2), 5, 5},
            new Fraction[] {new Fraction(3, 2), 5, new Fraction(5, 2)},
            new Fraction[] {new Fraction(3, 2), new Fraction(5, 2), new Fraction(5, 2)},
            new Fraction[] {new Fraction(3, 2), new Fraction(5, 3), new Fraction(5, 3)},
            new Fraction[] {new Fraction(3, 2), new Fraction(5, 3), new Fraction(5, 4)},
            new Fraction[] {new Fraction(3, 2), new Fraction(5, 4), new Fraction(5, 4)},
            new Fraction[] {5, 5, new Fraction(5, 4)},
            new Fraction[] {new Fraction(5, 2), new Fraction(5, 2), new Fraction(5, 2)},
            new Fraction[] {new Fraction(5, 2), new Fraction(5, 3), new Fraction(5, 3)},
            new Fraction[] {new Fraction(5, 4), new Fraction(5, 4), new Fraction(5, 4)}
    }
    .Select(s => s.OrderBy(f => (double)f).ToArray())
    .ToArray();
}
