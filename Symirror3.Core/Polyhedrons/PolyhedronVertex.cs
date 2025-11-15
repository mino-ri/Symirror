using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons;

public class PolyhedronVertex<T>(T vector, ISymmetryElement symmetryElement)
{
    /// <summary>頂点の位置を取得または設定します。</summary>
    public T Vector { get; set; } = vector;

    /// <summary>この要素と対応する、元の対称性要素を取得します。</summary>
    public ISymmetryElement SymmetryElement { get; } = symmetryElement;
}
