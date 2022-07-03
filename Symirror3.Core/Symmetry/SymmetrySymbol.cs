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
}
