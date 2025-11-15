namespace Symirror3.Core.Symmetry;

/// <summary>対称性を構成する要素を表します。</summary>
public interface ISymmetryElement
{
    /// <summary>オブジェクトの、対称性を表す要素のカテゴリを取得します。</summary>
    SymmetryElementCategory ElementCategory { get; }

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    int ElementType { get; }

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    int Index { get; }
}

public class SymmetryElement(SymmetryElementCategory elementCategory, int elementType, int index) : ISymmetryElement
{
    /// <summary>オブジェクトの、対称性を表す要素のカテゴリを取得します。</summary>
    public SymmetryElementCategory ElementCategory { get; } = elementCategory;

    /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
    public int ElementType { get; } = elementType;

    /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
    public int Index { get; } = index;
}

/// <summary><see cref="ISymmetryElement"/>のカテゴリを表します。</summary>
public enum SymmetryElementCategory
{
    /// <summary>面。</summary>
    Face,
    /// <summary>頂点。</summary>
    Vertex,
}
