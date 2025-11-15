using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public class SnubDualPolyhedron<T>(SymmetryGroup symmetry, IVectorOperator<T> opr) : WythoffianPolyhedron<T>(symmetry, opr)
{
    protected override IEnumerable<PolyhedronVertex<T>> GetVertices(SymmetryGroup symmetry)
    {
        return symmetry.Faces
            .Select(f => new PolyhedronVertex<T>(_opr.Zero, f))
            .Concat(symmetry.Vertices.Select(v => new PolyhedronVertex<T>(_opr.Convert(v.Point), v)));
    }

    protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
    {
        var count = symmetry.Faces.Count;
        return [.. symmetry
            .Faces
            .Where(f => f.ElementType == 1)
            .Select(f =>
            {
                IEnumerable<PolyhedronVertex<T>> GetVertices()
                {
                    var fromFaces =
                        symmetry.GetNexts(f)
                                .OrderBy(r => (f.GetUnsharedVertexIndex(r) + 1) % 3)
                                .Select(r => Vertices[r.Index])
                                .ToArray();

                    if (symmetry.Symbol[0].Numerator != 2) yield return Vertices[f[0].Index + count];
                    yield return fromFaces[0];
                    if (symmetry.Symbol[1].Numerator != 2) yield return Vertices[f[1].Index + count];
                    yield return fromFaces[1];
                    if (symmetry.Symbol[2].Numerator != 2) yield return Vertices[f[2].Index + count];
                    yield return fromFaces[2];
                }

                return new PolyhedronFace<T>(f, GetVertices());
            })];
    }

    protected override void OnBasePointChanged(SphericalPoint value)
    {
        var polygon = Symmetry[0];

        // ねじれのみの特殊な頂点位置を求める
        var ard = Enumerable.Range(0, 3)
            .Select(i => polygon.Edge((i + 2) % 3).Reverse(value))
            .ToArray();

        var snubPoint = SphericalPoint.Normalize(ard[0] + ard[1] + ard[2]); // 重心
        var snubDistance = 1.0;

        for (var i = 0; i < 3; i++)
            ard[i] = polygon.Edge((i + 2) % 3).Reverse(snubPoint);

        var normal = SphericalPoint.Normalize(Numerics.Vector3D.Cross(ard[1] - ard[0], ard[2] - ard[0]));

        // カタランの立体と同じ位置にある頂点の距離
        var distances = polygon.Select(v => SphericalPoint.GetCrossPoint(v.Point, normal, ard[0])).ToArray();

        // 立体の外接球を統一する
        var max = Math.Max(distances.Select(Math.Abs).Max(), Math.Abs(snubDistance));
        for (var i = 0; i < distances.Length; i++)
            distances[i] /= max;
        snubDistance /= max;

        // カタランの立体と同じ位置にある頂点
        for (var i = 0; i < Symmetry.Vertices.Count; i++)
            Vertices[Symmetry.Order + i].Vector = _opr.Convert(Symmetry.Vertices[i].Point * distances[Symmetry.Vertices[i].ElementType]);

        // 一様多面体と同じ位置にある頂点
        Vertices[0].Vector = _opr.Convert(snubPoint * snubDistance);
        CopyUniformVertices();
    }

    private void CopyUniformVertices()
    {
        var count = Symmetry.Faces.Count;
        for (var i = 1; i < count; i++)
        {
            var copyDef = CopyDefinitions[i - 1];
            var source = copyDef.Source;
            var edge = copyDef.ReverseEdge;
            Vertices[i].Vector = _opr.Reverse(Vertices[source].Vector, Symmetry[source].Edge(edge));
        }
    }
}
