using System;
using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public abstract class WythoffianPolyhedron<T> : PolyhedronBase<T>
    {
        protected WythoffianPolyhedron(Symmetry<T> symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronVertex<T>> GetVertices(Symmetry<T> symmetry)
        {
            return symmetry.Faces
                .Select(f => new PolyhedronVertex<T>(_opr.Zero, f));
        }

        protected override void OnBasePointChanged(T value)
        {
            Vertices[0].Vector = value;
            for (int i = 1; i < Vertices.Length; i++)
            {
                var copyDef = CopyDifinitions[i - 1];
                Vertices[i].Vector = _opr.Reverse(Vertices[copyDef.Source].Vector,
                    Symmetry[copyDef.Source][copyDef.ReverseEdge1].Vector,
                    Symmetry[copyDef.Source][copyDef.ReverseEdge2].Vector);
            }
        }

        private CopyDifinition[]? _copyDifinitions;
        protected CopyDifinition[] CopyDifinitions => _copyDifinitions ??= GetCopyDifinition();

        private CopyDifinition[] GetCopyDifinition()
        {
            var count = Symmetry.Order;
            var indices = new[] { 0, 1, 2 };
            var result = new CopyDifinition[count - 1];
            for (var i = 1; i < count; i++)
            {
                var current = Symmetry.Faces[i];
                var source = Symmetry.Faces.Take(i).First(current.IsNext);
                result[i - 1] = new CopyDifinition(source.Index, Array.FindAll(indices, n => current[n] == source[n]));
            }
            return result;
        }

        // 対象のインデックスの頂点が、どの頂点をどの辺で反転して得られるものかを表す
        protected readonly struct CopyDifinition
        {
            // コピー元となる球面三角形のインデックス
            public readonly int Source;
            // コピーに使用する辺の両端の頂点のElementType
            public readonly int ReverseEdge1;
            // コピーに使用する辺の両端の頂点のElementType
            public readonly int ReverseEdge2;

            public CopyDifinition(int source, int[] reverseEdges)
            {
                Source = source;
                ReverseEdge1 = reverseEdges[0];
                ReverseEdge2 = reverseEdges[1];
            }
        }
    }
}
