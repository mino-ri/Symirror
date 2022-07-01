using Symirror4.Core.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Symirror4.Core.Symmetry;

public class SymmetryTetrahedron : ISymmetryElement, IReadOnlyList<SymmetryVertex>
{
    private readonly SymmetryVertex[] _vertices;
    private readonly SymmetryTetrahedron?[] _nexts = new SymmetryTetrahedron?[4];

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    public int ElementType { get; }

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    public int Index { get; }

    /// <summary><see cref="SymmetryElementCategory.Cell"/>を返します。</summary>
    public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Cell;

    /// <summary>このオブジェクト自身を返します。</summary>
    public ISymmetryElement SymmetryElement => this;

    /// <summary>多角形を構成する頂点を取得します。</summary>
    public SymmetryVertex this[int index] => _vertices[index];

    int IReadOnlyCollection<SymmetryVertex>.Count => 4;

    internal SymmetryTetrahedron(int elementType, int index, SymmetryVertex vertex0, SymmetryVertex vertex1, SymmetryVertex vertex2, SymmetryVertex vertex3)
    {
        ElementType = elementType;
        Index = index;
        _vertices = new[] { vertex0, vertex1, vertex2, vertex3 };
    }

    public SymmetryTetrahedron? GetNext(int reverseElementType) => _nexts[reverseElementType];

    internal void SetNext(int reverseElementType, SymmetryTetrahedron next) => _nexts[reverseElementType] = next;

    public IEnumerator<SymmetryVertex> GetEnumerator() => ((IEnumerable<SymmetryVertex>)_vertices).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public GlomericSphere Face(int index) =>
        new(_vertices[(index + 1) % 4].Point,
            _vertices[(index + 2) % 4].Point,
            _vertices[(index + 3) % 4].Point);

    public Vector4D WeightedVertex(int index)
    {
        var a = Vector4D.Dot(_vertices[(index + 1) % 4].Point, _vertices[(index + 2) % 4].Point);
        var b = Vector4D.Dot(_vertices[(index + 2) % 4].Point, _vertices[(index + 3) % 4].Point);
        var c = Vector4D.Dot(_vertices[(index + 3) % 4].Point, _vertices[(index + 1) % 4].Point);
        return _vertices[index].Point * Sqrt(1.0 - a * a - b * b - c * c + 2.0 * a * b * c);
    }

    public GlomericPoint GetBisectorCross(int index0, int index1)
    {
        return GlomericPoint.Normalize(
            WeightedVertex(index0) +
            WeightedVertex(index1));
    }

    public GlomericPoint GetTrisectorCross(int index)
    {
        return GlomericPoint.Normalize(
            WeightedVertex((index + 1) % 4) +
            WeightedVertex((index + 2) % 4) +
            WeightedVertex((index + 3) % 4));
    }

    public GlomericPoint GetIncenter()
    {
        return GlomericPoint.Normalize(
            WeightedVertex(0) +
            WeightedVertex(1) +
            WeightedVertex(2) +
            WeightedVertex(3));
    }

    /// <summary>この超球胞四面体と指定した超球胞四面体が共有している頂点の数を取得します。</summary>
    /// <param name="other">調査する超球胞四面体。</param>
    /// <returns>共有している頂点の数。</returns>
    internal int CountSharedVertices(SymmetryTetrahedron other)
    {
        var result = 0;
        for (var i = 0; i < 4; i++) if (this[i] == other[i]) result++;
        return result;
    }

    /// <summary>この超球胞四面体と指定した超球胞四面体が共有していない頂点のインデックスを取得します。</summary>
    /// <param name="other">調査する超球胞四面体。</param>
    /// <returns>共有していない頂点のインデックスの最小値。</returns>
    internal int GetUnsharedVertexIndex(SymmetryTetrahedron other)
    {
        for (var i = 0; i < 4; i++) if (this[i] != other[i]) return i;
        return 0;
    }

    /// <summary>この超球胞四面体と指定した超球胞四面体が面で隣接しているかどうか調査します。</summary>
    /// <param name="other">調査する超球胞四面体。</param>
    /// <returns>2つの超球胞四面体が隣接している場合 true。それ以外の場合 false。</returns>
    internal bool IsNext(SymmetryTetrahedron other)
    {
        return _nexts.Contains(other);
    }

    /// <summary>指定された分数のワイソフ記号から、超球胞充填を構成する1つ目の超球胞四面体を取得します。</summary>
    /// <param name="symbol">対称性を表す<see cref="SymmetrySymbol"/>。</param>
    internal static SymmetryTetrahedron Create(SymmetrySymbol symbol, int[] cellCounts)
    {
        var angleE = PI / symbol.F0;
        var angleF = PI / symbol.F1;
        var angleG = PI / symbol.F4;
        var angleH = PI / symbol.F5;
        var angleI = PI / symbol.F3;
        var angleJ = PI / symbol.F2;

        var sinE = Sin(angleE);
        var sinF = Sin(angleF);
        var sinG = Sin(angleG);
        // var sinH = Sin(angleH);
        var sinI = Sin(angleI);
        var sinJ = Sin(angleJ);

        var cosE = Cos(angleE);
        var cosF = Cos(angleF);
        var cosG = Cos(angleG);
        var cosH = Cos(angleH);
        var cosI = Cos(angleI);
        var cosJ = Cos(angleJ);

        var cosCa = (cosE * cosG + cosF) / (sinE * sinG);
        var cosCb = (cosE * cosI + cosJ) / (sinE * sinI);
        var cosCd = (cosG * cosI + cosH) / (sinG * sinI);

        var cosDa = (cosE * cosF + cosG) / (sinE * sinF);
        var cosDb = (cosE * cosJ + cosI) / (sinE * sinJ);
        var cosDc = (cosF * cosJ + cosH) / (sinF * sinJ);

        var sinCa = Sqrt(Max(0d, 1d - cosCa * cosCa));
        // var sinCb = Sqrt(Max(0d, 1d - cosCb * cosCb));
        var sinCd = Sqrt(Max(0d, 1d - cosCd * cosCd));
        var sinDa = Sqrt(Max(0d, 1d - cosDa * cosDa));
        var sinDb = Sqrt(Max(0d, 1d - cosDb * cosDb));
        var sinDc = Sqrt(Max(0d, 1d - cosDc * cosDc));

        var cosEdgeE = (cosDa * cosDb + cosDc) / (sinDa * sinDb);
        var cosEdgeF = (cosDa * cosDc + cosDb) / (sinDa * sinDc);
        var cosEdgeG = (cosCa * cosCd + cosCb) / (sinCa * sinCd);

        var sinEdgeE = Sqrt(Max(0d, 1d - cosEdgeE * cosEdgeE));
        var sinEdgeF = Sqrt(Max(0d, 1d - cosEdgeF * cosEdgeF));
        var sinEdgeG = Sqrt(Max(0d, 1d - cosEdgeG * cosEdgeG));

        var cell = new SymmetryTetrahedron(0, 0,
            new SymmetryVertex(new(0f, 0f, 0f, 1f), 0, 0, cellCounts[0]),
            new SymmetryVertex(new(0f, 0f, CorrectError(sinEdgeE), CorrectError(cosEdgeE)), 1, 1, cellCounts[1]),
            new SymmetryVertex(new(0f, CorrectError(sinDa * sinEdgeF), CorrectError(cosDa * sinEdgeF), CorrectError(cosEdgeF)), 2, 2, cellCounts[2]),
            new SymmetryVertex(new(CorrectError(sinE * sinCa * sinEdgeG), CorrectError(cosE * sinCa * sinEdgeG), CorrectError(cosCa * sinEdgeG), CorrectError(cosEdgeG)), 3, 3, cellCounts[3]));

        for (var i = 0; i < 4; i++)
            cell[i].AddAroundCell(cell);

        return cell;
    }

    /// <summary>値が限りなく0または1に近い場合、それを0または1にします。</summary>
    private static float CorrectError(double a)
    {
        if (-0.00001 < a && a < 0.00001)
            return 0f;

        if (0.99999 < a && a < 1.00001)
            return 1f;

        return (float)a;
    }
}
