using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public abstract class WythoffianPolyhedron<T>(SymmetryGroup symmetry, IVectorOperator<T> opr) : PolyhedronBase<T>(symmetry, opr)
{
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
            var copyDef = CopyDefinitions[i - 1];
            Vertices[i].Vector = _opr.Reverse(Vertices[copyDef.Source].Vector,
                Symmetry[copyDef.Source].Edge(copyDef.ReverseEdge));
        }
    }

    protected CopyDefinition[] CopyDefinitions => field ??= GetCopyDefinition();

    private CopyDefinition[] GetCopyDefinition()
    {
        var count = Symmetry.Order;
        var indices = new[] { 0, 1, 2 };
        var result = new CopyDefinition[count - 1];
        for (var i = 1; i < count; i++)
        {
            var current = Symmetry.Faces[i];
            var source = Symmetry.Faces.Take(i).First(current.IsNext);
            result[i - 1] = new CopyDefinition(source.Index, Array.Find(indices, n => current[n] != source[n]));
        }
        return result;
    }

    // 対象のインデックスの頂点が、どの頂点をどの辺で反転して得られるものかを表す
    protected readonly struct CopyDefinition(int source, int reverseEdge)
    {
        // コピー元となる球面三角形のインデックス
        public readonly int Source = source;
        // コピーに使用する辺の両端の頂点のElementType
        public readonly int ReverseEdge = reverseEdge;
    }
}
