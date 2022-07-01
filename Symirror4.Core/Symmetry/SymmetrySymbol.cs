using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Symirror4.Core.Symmetry;

/// <summary>コクセター記号の直列表現を表します。</summary>
/// <remarks>
/// 記号は6つの分数の組で、次の順序で並んでいることを想定しています。
///        ┏━━━━━ 5 ━━━━━┓
/// ① -2- ④ -3- ② -4- ③ -1-
/// ┗━━━━━ 0 ━━━━━┛
/// </remarks>
public record SymmetrySymbol(Fraction F0, Fraction F1, Fraction F2, Fraction F3, Fraction F4, Fraction F5) : IReadOnlyList<Fraction>
{
    public SymmetrySymbol(Fraction c, Fraction d, Fraction e)
        : this(2, 2, c, d, e, 2)
    { }

    public Fraction this[int index] => index switch
    {
        0 => F0,
        1 => F1,
        2 => F2,
        3 => F3,
        4 => F4,
        5 => F5,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public int Count => 6;

    public IEnumerator<Fraction> GetEnumerator()
    {
        yield return F0;
        yield return F1;
        yield return F2;
        yield return F3;
        yield return F4;
        yield return F5;
    }

    /// <summary>このシンボルから玲胞四面体を作れるか判定します。</summary>
    public bool CanMakeTetrahedron
    {
        get
        {
            bool IsRight(int a, int b, int c) => Sphere.CanMakeTriangle(this[a], this[b], this[c]);

            return IsRight(3, 4, 5) &&
                   IsRight(1, 2, 5) &&
                   IsRight(0, 2, 3) &&
                   IsRight(0, 1, 4);
        }
    }

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表すSystem.String。</returns>
    public override string ToString()
    {
        var sym = this.Select(s =>
              s == 2 ? "_"
            : s.Denominator == 1 ? s.Numerator.ToString()
            : s.Denominator == s.Numerator - 1 ? s.Numerator + "'"
            : s == (5, 2) ? "$"
            : s == (5, 3) ? "$'"
            : s.ToString()
            ).ToArray();

        return $"{OpenBrace(sym[1])}{OpenParent(sym[0])}{sym[2]} {OpenParent(sym[5])}{sym[3]}{CloseParent(sym[0])} {sym[4]}{CloseParent(sym[5])}{CloseBrace(sym[1])}";

        string OpenParent(string value) => value == "_" ? "" : "(";
        string CloseParent(string value) => value == "_" ? "" : $" {value})";
        string OpenBrace(string value) => value == "_" ? "" : "[";
        string CloseBrace(string value) => value == "_" ? "" : $" {value}]";
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
