using System;
using System.Collections;
using System.Collections.Generic;

namespace Symirror3.Core.Symmetry;

public record SymmetrySymbol(Fraction F0, Fraction F1, Fraction F2) : IReadOnlyList<Fraction>
{
    int IReadOnlyCollection<Fraction>.Count => 3;

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public override string ToString() => $"{F0.ToSymbolString()} {F1.ToSymbolString()} {F2.ToSymbolString()}";

    public IEnumerator<Fraction> GetEnumerator()
    {
        yield return F0;
        yield return F1;
        yield return F2;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Fraction this[int index] => index switch
    {
        0 => F0,
        1 => F1,
        2 => F2,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    static readonly Fraction F3_2 = new(3, 2);
    static readonly Fraction F4_3 = new(4, 3);
    static readonly Fraction F5_2 = new(5, 2);
    static readonly Fraction F5_3 = new(5, 3);
    static readonly Fraction F5_4 = new(5, 4);

    public static readonly SymmetrySymbol[] AllSymbols =
    {
            new(2, 3, 3),
            new(2, 3, F3_2),
            new(2, F3_2, F3_2),
            new(3, 3, F3_2),
            new(F3_2, F3_2, F3_2),

            new(2, 3, 4),
            new(2, 3, F4_3),
            new(2, F3_2, 4),
            new(2, F3_2, F4_3),
            new(3, 4, F4_3),
            new(F3_2, 4, 4),
            new(F3_2, F4_3, F4_3),

            // 2 3 5
            new(2,  3  ,  5  ),
            new(2,  3  , F5_4),
            new(2, F3_2,  5  ),
            new(2, F3_2, F5_4),
            new(2,  3  , F5_2),
            new(2,  3  , F5_3),
            new(2, F3_2, F5_2),
            new(2, F3_2, F5_3),

            // 2 5 5
            new(2,  5  , F5_2),
            new(2,  5  , F5_3),
            new(2, F5_4, F5_2),
            new(2, F5_4, F5_3),

            // 3 3 5
            new( 3  ,  3  , F5_4),
            new( 3  , F3_2,  5  ),
            new(F3_2, F3_2, F5_4),
            new( 3  ,  3  , F5_2),
            new( 3  , F3_2, F5_3),
            new(F3_2, F3_2, F5_2),

            // 3 5 5
            new( 3  ,  5  , F5_4),
            new(F3_2,  5  ,  5  ),
            new(F3_2, F5_4, F5_4),
            new( 3  ,  5  , F5_3),
            new( 3  , F5_4, F5_2),
            new(F3_2,  5  , F5_2),
            new(F3_2, F5_4, F5_3),
            new( 3,   F5_2, F5_3),
            new(F3_2, F5_2, F5_2),
            new(F3_2, F5_3, F5_3),

            // 5 5 5
            new( 5  ,  5  , F5_4),
            new(F5_4, F5_4, F5_4),
            new(F5_2, F5_2, F5_2),
            new(F5_2, F5_3, F5_3),

            new(2, 2, 2),
            new(2, 2, 3),
            new(2, 2, 4),
            new(2, 2, F4_3),
            new(2, 2, 5),
            new(2, 2, F5_4),
            new(2, 2, F5_2),
            new(2, 2, F5_3),
            new(2, 2, 6),
            new(2, 2, new(6, 5)),
            new(2, 2, 7),
            new(2, 2, new(7, 6)),
            new(2, 2, new(7, 2)),
            new(2, 2, new(7, 5)),
            new(2, 2, new(7, 3)),
            new(2, 2, new(7, 4)),
        };
}
