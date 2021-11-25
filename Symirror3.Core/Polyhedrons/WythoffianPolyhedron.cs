using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public abstract class WythoffianPolyhedron<T> : PolyhedronBase<T>
{
    protected WythoffianPolyhedron(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

    protected override IEnumerable<PolyhedronVertex<T>> GetVertices(SymmetryGroup symmetry)
    {
        return symmetry.Faces
            .Select(f => new PolyhedronVertex<T>(_opr.Zero, f));
    }

    protected override void OnBasePointChanged(SphericalPoint value)
    {
        Vertices[0].Vector = _opr.Convert(value);
        for (var i = 1; i < Vertices.Length; i++)
        {
            var copyDef = CopyDifinitions[i - 1];
            Vertices[i].Vector = _opr.Reverse(Vertices[copyDef.Source].Vector,
                Symmetry[copyDef.Source].Edge(copyDef.ReverseEdge));
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
            result[i - 1] = new CopyDifinition(source.Index, Array.Find(indices, n => current[n] != source[n]));
        }
        return result;
    }

    // 対象のインデックスの頂点が、どの頂点をどの辺で反転して得られるものかを表す
    protected readonly struct CopyDifinition
    {
        // コピー元となる球面三角形のインデックス
        public readonly int Source;
        // コピーに使用する辺の両端の頂点のElementType
        public readonly int ReverseEdge;

        public CopyDifinition(int source, int reverseEdge)
        {
            Source = source;
            ReverseEdge = reverseEdge;
        }
    }
}
