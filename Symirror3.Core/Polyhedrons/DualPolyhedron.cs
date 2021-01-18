using System;
using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class DualPolyhedron<T> : PolyhedronBase<T>
    {
        public DualPolyhedron(Symmetry<T> symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronVertex<T>> GetVertices(Symmetry<T> symmetry)
        {
            return symmetry.Vertices.Select(v => new PolyhedronVertex<T>(v.Vector, v));
        }

        protected override IEnumerable<PolyhedronFace<T>> GetFaces(Symmetry<T> symmetry)
        {
            return symmetry
                .Faces
                .Select(f => new PolyhedronFace<T>(f, f.Select(v => Vertices[v.Index])));
        }

        protected override void OnBasePointChanged(T value)
        {
            var triangle = Symmetry.Faces[0];
            var distances = triangle.Select(v => _opr.CatalanPoint(v.Vector, value)).ToArray();
            var max = distances.Select(Math.Abs).Max();
            for (var i = 0; i < distances.Length; i++)
                distances[i] /= max;

            for (var i = 0; i < Vertices.Length; i++)
                Vertices[i].Vector = _opr.Multiply(Symmetry.Vertices[i].Vector, distances[Symmetry.Vertices[i].ElementType]);
        }
    }
}
