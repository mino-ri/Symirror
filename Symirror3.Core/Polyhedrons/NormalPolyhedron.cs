using Symirror3.Core.Symmetry;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public class NormalPolyhedron<T>(SymmetryGroup symmetry, IVectorOperator<T> opr)
    : WythoffianPolyhedron<T>(symmetry, opr)
{
    protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
    {
        return symmetry.Vertices
            .Select(v => new PolyhedronFace<T>(v, symmetry.GetAround(v).Select(f => Vertices[f.Index])));
    }
}
