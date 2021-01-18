using System;
using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class SnubDualPolyhedron<T> : WythoffianPolyhedron<T>
    {
        public SnubDualPolyhedron(Symmetry<T> symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronVertex<T>> GetVertices(Symmetry<T> symmetry)
        {
            return symmetry.Faces
                .Select(f => new PolyhedronVertex<T>(_opr.Zero, f))
                .Concat(symmetry.Vertices.Select(v => new PolyhedronVertex<T>(v.Vector, v)));
        }

        protected override IEnumerable<PolyhedronFace<T>> GetFaces(Symmetry<T> symmetry)
        {
            var count = symmetry.Faces.Count;
            return symmetry
                .Faces
                .Where(f => f.ElementType == 1)
                .Select(f =>
                {
                    IEnumerable<PolyhedronVertex<T>> GetVerties()
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

                    return new PolyhedronFace<T>(f, GetVerties());
                })
                .ToArray();
        }

        protected override void OnBasePointChanged(T value)
        {
            var polygon = Symmetry[0];

            // ねじれのみの特殊な頂点位置を求める
            var ard = Enumerable.Range(0, 3)
                .Select(i => _opr.Reverse(value, polygon[i].Vector, polygon[(i + 1) % 3].Vector))
                .ToArray();

            var snubPoint = _opr.Normalize(_opr.Sum(ard)); // 重心
            var snubDistance = 1.0;

            for (var i = 0; i < 3; i++)
                ard[i] = _opr.Reverse(snubPoint, polygon[i].Vector, polygon[(i + 1) % 3].Vector);

            var normal = _opr.Normalize(_opr.Cross(_opr.Subtract(ard[1], ard[0]), _opr.Subtract(ard[2], ard[0])));

            // カタランの立体と同じ位置にある頂点の距離
            var distances = polygon.Select(v => _opr.GetCrossPoint(v.Vector, normal, ard[0])).ToArray();

            // 立体の外接球を統一する
            var max = Math.Max(distances.Select(Math.Abs).Max(), Math.Abs(snubDistance));
            for (var i = 0; i < distances.Length; i++)
                distances[i] /= max;
            snubDistance /= max;

            // カタランの立体と同じ位置にある頂点
            for (var i = 0; i < Symmetry.Vertices.Count; i++)
                Vertices[Symmetry.Order + i].Vector = _opr.Multiply(Symmetry.Vertices[i].Vector, distances[Symmetry.Vertices[i].ElementType]);

            // 一様多面体と同じ位置にある頂点
            Vertices[0].Vector = _opr.Multiply(snubPoint, snubDistance);
            CopyUniformVertices();
        }

        private void CopyUniformVertices()
        {
            var count = Symmetry.Faces.Count;
            for (int i = 1; i < count; i++)
            {
                var copyDef = CopyDifinitions[i - 1];
                var source = copyDef.Source;
                var edge1 = copyDef.ReverseEdge1;
                var edge2 = copyDef.ReverseEdge2;
                Vertices[i].Vector = _opr.Reverse(Vertices[source].Vector, Symmetry[source][edge1].Vector, Symmetry[source][edge2].Vector);
            }
        }
    }
}
