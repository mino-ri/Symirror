using System.Collections.Generic;

namespace Symirror4.Core.Symmetry;

/// <summary>対称性オブジェクトの頂点を表します。</summary>
public class SymmetryVertex : ISymmetryElement
{
    private int _cellCount;

    /// <summary>この頂点を囲んでいる胞を取得します。</summary>
    public SymmetryTetrahedron[] AroundCells { get; }

    /// <summary>頂点の位置を取得します。</summary>
    public GlomericPoint Point { get; }

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    public int ElementType { get; }

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    public int Index { get; }

    /// <summary><see cref="SymmetryElementCategory.Vertex"/>を返します。</summary>
    public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Vertex;

    public SymmetryVertex(GlomericPoint point, int elementType, int index, int cellCount)
    {
        Point = point;
        ElementType = elementType;
        Index = index;
        _cellCount = 0;
        AroundCells = new SymmetryTetrahedron[cellCount];
    }

    internal void AddAroundCell(SymmetryTetrahedron tetrahedron)
    {
        AroundCells[_cellCount] = tetrahedron;
        _cellCount++;
    }

    internal bool IsAroundCellsSingle => _cellCount == 1;

    internal bool IsAroundCellsFull => _cellCount == AroundCells.Length;

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public override string ToString() => Point.ToString();
}
