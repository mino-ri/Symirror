namespace Symirror4.Core.Symmetry;

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

/// <summary>
/// 
/// </summary>
/// <param name="ElementCategory">オブジェクトの、対称性を表す要素のカテゴリを取得します。</param>
/// <param name="ElementType">同じカテゴリの要素内での、区別のための種別番号を取得します。</param>
/// <param name="Index">同じ種類の要素内での、区別のためのインデックスを取得します。</param>
public record SymmetryElement(SymmetryElementCategory ElementCategory, int ElementType, int Index) : ISymmetryElement;

/// <summary><see cref="ISymmetryElement"/>のカテゴリを表します。</summary>
public enum SymmetryElementCategory
{
    /// <summary>胞。</summary>
    Cell,
    /// <summary>面。</summary>
    Face,
    /// <summary>辺。</summary>
    Edge,
    /// <summary>頂点。</summary>
    Vertex,
}
