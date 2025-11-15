using Symirror3.Core.Symmetry;
using System.Collections.Generic;

namespace Symirror3.Core.Polyhedrons;

public class PolyhedronFace<T>(ISymmetryElement symmetryElement, IEnumerable<PolyhedronVertex<T>> vertices)
{
    /// <summary>この要素と対応する、元の対称性要素を取得します。</summary>
    public ISymmetryElement SymmetryElement { get; } = symmetryElement;

    /// <summary>多角形を構成する頂点を取得します。</summary>
    public PolyhedronVertex<T>[] Vertices { get; } = [.. vertices];
}
