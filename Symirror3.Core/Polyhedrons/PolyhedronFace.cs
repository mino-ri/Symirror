using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class PolyhedronFace<T>
    {
        /// <summary>この要素と対応する、元の対称性要素を取得します。</summary>
        public ISymmetryElement SymmetryElement { get; }

        /// <summary>多角形を構成する頂点を取得します。</summary>
        public PolyhedronVertex<T>[] Vertices { get; }

        public PolyhedronFace(ISymmetryElement symmetryElement, IEnumerable<PolyhedronVertex<T>> vertices)
        {
            SymmetryElement = symmetryElement;
            Vertices = vertices.ToArray();
        }
    }
}
