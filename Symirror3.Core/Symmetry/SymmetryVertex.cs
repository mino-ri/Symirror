namespace Symirror3.Core.Symmetry;

public class SymmetryVertex : ISymmetryElement
{
    /// <summary>頂点の位置を取得します。</summary>
    public SphericalPoint Point { get; }

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    public int ElementType { get; }

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    public int Index { get; }

    /// <summary><see cref="SymmetryElementCategory.Vertex"/>を返します。</summary>
    public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Vertex;

    public SymmetryVertex(SphericalPoint vector, int symmetryElementType, int index)
    {
        Point = vector;
        ElementType = symmetryElementType;
        Index = index;
    }

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public override string ToString() => $"{ElementType} {Point}";
}
