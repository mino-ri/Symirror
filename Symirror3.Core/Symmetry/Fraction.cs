using System;

namespace Symirror3.Core.Symmetry;

/// <summary> ワイソフ記号用の分数を表します。 </summary>
public readonly struct Fraction : IEquatable<Fraction>
{
    /// <summary> 分子 </summary>
    public readonly int Numerator;

    /// <summary> 分母 </summary>
    public readonly int Denominator;

    /// <summary>Fraction 構造体の新しいインスタンスを生成します。</summary>
    /// <param name="numerator">分子。</param>
    /// <param name="denominator">分母。</param>
    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0) throw new ArgumentException("分母を0にすることはできません。", nameof(denominator));
        if (numerator == 0)
        {
            Numerator = 0;
            Denominator = 1;
        }
        else
        {
            var sign = Math.Sign(numerator) * Math.Sign(denominator);
            var gcd = Gcd(Math.Abs(numerator), Math.Abs(denominator));
            Numerator = numerator / gcd * sign;
            Denominator = denominator / gcd;
        }
    }

    /// <summary>このオブジェクトのハッシュコードを取得します。</summary>
    /// <returns>ハッシュコードを表す<see cref="int"/>。</returns>
    public override int GetHashCode() => Numerator ^ Denominator;

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";


    /// <summary>このオブジェクトを、三角形記号に使用する文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public string ToSymbolString() => (Numerator, Denominator) switch
    {
        (var n, 1) => n.ToString(),
        (5, 2) => "$",
        (5, 3) => "$'",
        (var n, var d) when d == n - 1 => $"{n}'",
        (var n, var d) when d > n / 2 => $"{n}/{n - d}'",
        (var n, var d) => $"{n}/{d}",
    };

    /// <summary>このオブジェクトと指定したオブジェクトが等しいか判断します。</summary>
    /// <param name="obj">比較対象のオブジェクト。</param>
    /// <returns>オブジェクトが等しいかを表す<see cref="bool"/>。</returns>
    public bool Equals(Fraction other) => this == other;

    /// <summary>このオブジェクトと指定したオブジェクトが等しいか判断します。</summary>
    /// <param name="obj">比較対象のオブジェクト。</param>
    /// <returns>オブジェクトが等しいかを表す<see cref="bool"/>。</returns>
    public override bool Equals(object? obj) => obj switch
    {
        int @int => this == @int,
        Fraction frac => this == frac,
        _ => false
    };

    /// <summary>このオブジェクトを分解します。</summary>
    /// <param name="numerator">分子</param>
    /// <param name="denominator">分母</param>
    public void Deconstruct(out int numerator, out int denominator) => (numerator, denominator) = (Numerator, Denominator);

    public static implicit operator Fraction(int x) => new(x, 1);
    public static implicit operator Fraction((int numerator, int denominator) x) => new(x.numerator, x.denominator);
    public static implicit operator float(Fraction x) => (float)x.Numerator / x.Denominator;
    public static implicit operator double(Fraction x) => (double)x.Numerator / x.Denominator;

    public static bool operator ==(Fraction a, Fraction b) => a.Numerator == b.Numerator && a.Denominator == b.Denominator;
    public static bool operator !=(Fraction a, Fraction b) => a.Numerator != b.Numerator || a.Denominator != b.Denominator;

    public static bool operator ==(Fraction f, int s) => f.Denominator == 1 && f.Numerator == s;
    public static bool operator !=(Fraction f, int s) => f.Denominator != 1 || f.Numerator != s;

    private static int Gcd(int x, int y)
    {
        if (x == 0) return y;
        if (y == 0) return x;

        while (true)
        {
            if (x >= y)
            {
                x %= y;
                if (x == 0) return y;
            }
            else
            {
                y %= x;
                if (y == 0) return x;
            }
        }
    }
}
