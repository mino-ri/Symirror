using System;
using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class SymmetrySourcePolyhedron<T> : PolyhedronBase<T>
    {
        public SymmetrySourcePolyhedron(Symmetry<T> symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

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
            // do nothing
        }
    }
}
