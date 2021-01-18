using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class NormalPolyhedron<T> : WythoffianPolyhedron<T>
    {
        public NormalPolyhedron(Symmetry<T> symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronFace<T>> GetFaces(Symmetry<T> symmetry)
        {
            return symmetry.Vertices
                .Select(v => new PolyhedronFace<T>(v, symmetry.GetAround(v).Select(f => Vertices[f.Index])));
        }
    }
}
