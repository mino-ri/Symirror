using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons;

public class PolyhedronVertex<T>
{
    /// <summary>頂点の位置を取得または設定します。</summary>
    public T Vector { get; set; }

    /// <summary>この要素と対応する、元の対称性要素を取得します。</summary>
    public ISymmetryElement SymmetryElement { get; }

    public PolyhedronVertex(T vector, ISymmetryElement symmetryElement)
    {
        Vector = vector;
        SymmetryElement = symmetryElement;
    }
}
