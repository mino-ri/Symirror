using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Symirror3.Core.Symmetry;

public class SymmetryTriangle : ISymmetryElement, IReadOnlyList<SymmetryVertex>
{
    private readonly SymmetryVertex[] _vertices;

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    public int ElementType { get; }

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    public int Index { get; }

    /// <summary><see cref="SymmetryElementCategory.Face"/>を返します。</summary>
    public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Face;

    /// <summary>このオブジェクト自身を返します。</summary>
    public ISymmetryElement SymmetryElement => this;

    /// <summary>多角形を構成する頂点を取得します。</summary>
    public SymmetryVertex this[int index] => _vertices[index];

    int IReadOnlyCollection<SymmetryVertex>.Count => 3;

    internal SymmetryTriangle(int elementType, int index, SymmetryVertex vertex0, SymmetryVertex vertex1, SymmetryVertex vertex2)
    {
        ElementType = elementType;
        Index = index;
        _vertices = new[] { vertex0, vertex1, vertex2 };
    }

    private SymmetryTriangle((double x, double y, double z) vertex1, (double x, double y, double z) vertex2, (double x, double y , double z) vertex3)
        : this (0, 0,
        new SymmetryVertex(new SphericalPoint(vertex1.x, vertex1.y, vertex1.z), 0, 0),
        new SymmetryVertex(new SphericalPoint(vertex2.x, vertex2.y, vertex2.z), 1, 1),
        new SymmetryVertex(new SphericalPoint(vertex3.x, vertex3.y, vertex3.z), 2, 2))
    {
        ValidatePoint(vertex1, nameof(vertex1));
        ValidatePoint(vertex2, nameof(vertex2));
        ValidatePoint(vertex3, nameof(vertex3));

        static void ValidatePoint((double x, double y, double z) v, string parameterName)
        {
            var length = v.x * v.x + v.y * v.y + v.z * v.z;
            if (length is < (1.0 - SphericalPoint.DefaultError) or > (1.0 + SphericalPoint.DefaultError))
            {
                throw new ArgumentException($"Invalid Point: {v.x: #.0000} {v.y: #.0000} {v.z: #.0000}", parameterName);
            }
        }
    }

    public IEnumerator<SymmetryVertex> GetEnumerator() => ((IEnumerable<SymmetryVertex>)_vertices).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public SphericalRing Edge(int index) =>
        new(_vertices[(index + 1) % 3].Point, _vertices[(index + 2) % 3].Point);

    public SphericalPoint GetBisectorCross(int index)
    {
        var a = _vertices[index].Point;
        var b = _vertices[(index + 1) % 3].Point;
        var c = _vertices[(index + 2) % 3].Point;
        return SphericalPoint.Normalize(
            b * SphericalPoint.OppositeSin(a, c) +
            c * SphericalPoint.OppositeSin(a, b));
    }

    public SphericalPoint GetIncenter()
    {
        var a = _vertices[0].Point;
        var b = _vertices[1].Point;
        var c = _vertices[2].Point;
        return SphericalPoint.Normalize(
            a * SphericalPoint.OppositeSin(b, c) +
            b * SphericalPoint.OppositeSin(a, c) +
            c * SphericalPoint.OppositeSin(a, b));
    }

    /// <summary>この球面三角形と指定した球面三角形が共有している頂点の数を取得します。</summary>
    /// <param name="other">調査する球面三角形。</param>
    /// <returns>共有している頂点の数。</returns>
    internal int CountSharedVertices(SymmetryTriangle other)
    {
        return this.Zip(other, (x, y) => x == y).Count(r => r);
    }

    /// <summary>この球面三角形と指定した球面三角形が共有していない頂点のインデックスを取得します。</summary>
    /// <param name="other">調査する球面三角形。</param>
    /// <returns>共有していない頂点のインデックスの最小値。</returns>
    internal int GetUnsharedVertexIndex(SymmetryTriangle other)
    {
        for (var i = 0; i < 3; i++)
            if (this[i] != other[i]) return i;
        return 0;
    }

    /// <summary>この球面三角形と指定した球面三角形が辺で隣接しているかどうか調査します。</summary>
    /// <param name="other">調査する球面三角形。</param>
    /// <returns>2つの球面三角形が隣接している場合 true。それ以外の場合 false。</returns>
    internal bool IsNext(SymmetryTriangle other)
    {
        if (this == other) return false;

        for (var i = 0; i < 3; i++)
            if (this[i] == other[i] && this[(i + 1) % 3] == other[(i + 1) % 3])
                return true;

        return false;
    }

    /// <summary>指定された分数のワイソフ記号から、球面充填を構成する1枚目の球面三角形を取得します。</summary>
    /// <param name="a">ワイソフ記号の1要素を表す分数。</param>
    /// <param name="b">ワイソフ記号の1要素を表す分数。</param>
    /// <param name="c">ワイソフ記号の1要素を表す分数。</param>
    /// <param name="createVector">ベクトルのインスタンスを生成する関数。</param>
    public static SymmetryTriangle Create(SymmetrySymbol symbol)
    {
        if (AllTriangles.TryGetValue(symbol, out var triangle))
            return triangle;

        var angleA = PI / symbol.F0;
        var angleB = PI / symbol.F1;
        var angleC = PI / symbol.F2;

        var cosB = OppositeCos(angleB, angleA, angleC);
        var cosC = OppositeCos(angleC, angleA, angleB);
        var sinB = Sqrt(1d - cosB * cosB);
        var sinC = Sqrt(1d - cosC * cosC);

        return new SymmetryTriangle(
            (0.0, 0.0, 1.0),
            (sinC, 0.0, cosC),
            (sinB * Cos(angleA), sinB * Sin(angleA), cosB));

        // A
        // |\
        //b| \c
        // |__\ 
        //C    B
    }

    /// <summary>直角球面三角形の3つの角の大きさから、角aの対辺AのCos値を求めます。</summary>
    /// <param name="a">球面三角形の角の大きさ(ラジアン)。</param>
    /// <param name="b">球面三角形の角の大きさ(ラジアン)。</param>
    /// <param name="c">球面三角形の角の大きさ(ラジアン)。</param>
    /// <returns>角aの対辺AのCos値。</returns>
    private static double OppositeCos(double a, double b, double c) => (Cos(b) * Cos(c) + Cos(a)) / (Sin(b) * Sin(c));

    public static readonly SymmetrySymbol[] AllSymbols;
    public static readonly Dictionary<SymmetrySymbol, SymmetryTriangle> AllTriangles;

    static SymmetryTriangle()
    {
        var f3_2 = new Fraction(3, 2);
        var f4_3 = new Fraction(4, 3);
        var f5_2 = new Fraction(5, 2);
        var f5_3 = new Fraction(5, 3);
        var f5_4 = new Fraction(5, 4);

        var s = Sqrt(2d) / 2d;
        var t = Sqrt(3d) / 3d;
        var u = Sqrt(6d) / 3d;
        var d = Sqrt((5d + Sqrt(5d)) / 10d);
        var e = Sqrt((5d - Sqrt(5d)) / 10d);
        var f = (Sqrt(5d) + 1d) * t / 2d;
        var g = (Sqrt(5d) - 1d) * t / 2d;

        var sets = new (SymmetrySymbol Symbol, SymmetryTriangle? Triangle)[]
        {
            (new( 2  ,  3  ,  3  ), new ((0, s, s), (t, 0, u), (t, u, 0))),
            (new( 2  ,  3  , f3_2), new ((0, s, s), (t, -u, 0), (t, u, 0))),
            (new( 2  , f3_2, f3_2), new ((0, s, s), (t, -u, 0), (t, 0, -u))),
            (new( 3  ,  3  , f3_2), new ((t, 0, u), (t, -u, 0), (t, u, 0))),
            (new(f3_2, f3_2, f3_2), new ((t, 0, u), (t, 0, -u), (-t, u, 0))),

            (new( 2  ,  3  ,  4  ), new ((0, 0, 1), (t, 0, u), (0, s, s))),
            (new( 2  ,  3  , f4_3), new ((0, 0, 1), (t, 0, -u), (0, s, s))),
            (new( 2  , f3_2,  4  ), new ((0, 0, 1), (t, 0, u), (0, s, -s))),
            (new( 2  , f3_2, f4_3), new ((0, 0, 1), (t, 0, -u), (0, s, -s))),
            (new( 3  ,  4  , f4_3), new ((t, 0, u), (0, -s, -s), (1, 0, 0))),
            (new(f3_2,  4  ,  4  ), new ((t, 0, u), (1, 0, 0), (0, s, s))),
            (new(f3_2, f4_3, f4_3), new ((t, 0, u), (0, -s, -s), (0, s, -s))),

            // 2 3 5
            (new(2,  3  ,  5  ), new((0, 0, 1), (g, 0, f), (0, e, d))),
            (new(2,  3  , f5_4), new((0, 0, 1), (g, 0, -f), (0, e, d))),
            (new(2, f3_2,  5  ), new((0, 0, 1), (g, 0, f), (0, e, -d))),
            (new(2, f3_2, f5_4), new((0, 0, 1), (g, 0, -f), (0, e, -d))),
            (new(2,  3  , f5_2), new((0, 0, 1), (0, -f, g), (d, 0, e))),
            (new(2,  3  , f5_3), new((0, 0, 1), (0, -f, -g), (d, 0, e))),
            (new(2, f3_2, f5_2), new((0, 0, 1), (0, -f, g), (d, 0, -e))),
            (new(2, f3_2, f5_3), new((0, 0, 1), (0, -f, -g), (d, 0, -e))),

            // 2 5 5
            (new(2,  5  , f5_2), new((0, 0, 1), (d, 0, e), (0, e, d))),
            (new(2,  5  , f5_3), new((0, 0, 1), (d, 0, -e), (0, e, d))),
            (new(2, f5_4, f5_2), new((0, 0, 1), (d, 0, e), (0, e, -d))),
            (new(2, f5_4, f5_3), new((0, 0, 1), (d, 0, -e), (0, e, -d))),

            // 3 3 5
            (new( 3  ,  3  , f5_4), new((g, 0, f), (g, 0, -f), (e, d, 0))),
            (new( 3  , f3_2,  5  ), new((g, 0, f), (t, -t, t), (d, 0, -e))),
            (new(f3_2, f3_2, f5_4), new((g, 0, f), (g, 0, -f), (-e, d, 0))),
            (new( 3  ,  3  , f5_2), new((g, 0, f), (t, t, t), (0, e, d))),
            (new( 3  , f3_2, f5_3), new((g, 0, f), (g, 0, -f), (0, e, -d))),
            (new(f3_2, f3_2, f5_2), new((g, 0, f), (t, -t, t), (0, e, -d))),

            // 3 5 5
            (new( 3  ,  5  , f5_4), new((g, 0, f), (0, -e, -d), (d, 0, e))),
            (new(f3_2,  5  ,  5  ), new((g, 0, f), (d, 0, e), (0, e, d))),
            (new(f3_2, f5_4, f5_4), new((g, 0, f), (0, -e, -d), (0, e, -d))),
            
            (new( 3  ,  5  , f5_3), new((g, 0, f), (e, -d, 0), (d, 0, e))),
            (new( 3  , f5_4, f5_2), new((g, 0, f), (d, 0, -e), (0, e, -d))),
            (new(f3_2,  5  , f5_2), new((g, 0, f), (d, 0, -e), (0, e, d))),
            (new(f3_2, f5_4, f5_3), new((g, 0, f), (e, -d, 0), (0, e, -d))),

            (new( 3,   f5_2, f5_3), new((g, 0, f), (d, 0, -e), (e, d, 0))),
            (new(f3_2, f5_2, f5_2), new((g, 0, f), (e, -d, 0), (e, d, 0))),
            (new(f3_2, f5_3, f5_3), new((g, 0, f), (d, 0, -e), (-e, d, 0))),

            // 5 5 5
            (new( 5  ,  5  , f5_4), new((0, e, d), (d, 0, -e), (e, d, 0))),
            (new(f5_4, f5_4, f5_4), new((0, e, d), (e, -d, 0), (0, e, -d))),
            (new(f5_2, f5_2, f5_2), new((0, e, d), (0, -e, d), (d, 0, e))),
            (new(f5_2, f5_3, f5_3), new((0, e, d), (d, 0, -e), (0, e, -d))),

            (new(2, 2, 2), null),
            (new(2, 2, 3), null),
            (new(2, 2, 4), null),
            (new(2, 2, f4_3), null),
            (new(2, 2, 5), null),
            (new(2, 2, f5_4), null),
            (new(2, 2, f5_2), null),
            (new(2, 2, f5_3), null),
            (new(2, 2, 6), null),
            (new(2, 2, new(6, 5)), null),
            (new(2, 2, 7), null),
            (new(2, 2, new(7, 6)), null),
            (new(2, 2, new(7, 2)), null),
            (new(2, 2, new(7, 5)), null),
            (new(2, 2, new(7, 3)), null),
            (new(2, 2, new(7, 4)), null),
        };

        AllSymbols = Array.ConvertAll(sets, t => t.Symbol);
        AllTriangles = sets.Where(t => t.Triangle is not null).ToDictionary(t => t.Symbol, t => t.Triangle!);
    }
}
