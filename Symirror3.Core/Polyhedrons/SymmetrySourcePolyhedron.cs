using Symirror3.Core.Symmetry;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public class SymmetrySourcePolyhedron<T>(SymmetryGroup symmetry, IVectorOperator<T> opr)
    : PolyhedronBase<T>(symmetry, opr)
{
    protected override IEnumerable<PolyhedronVertex<T>> GetVertices(SymmetryGroup symmetry)
    {
        return symmetry.Vertices.Select(v => new PolyhedronVertex<T>(_opr.Convert(v.Point), v));
    }

    protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
    {
        return symmetry
            .Faces
            .Select(f => new PolyhedronFace<T>(f, f.Select(v => Vertices[v.Index])));
    }

    protected override void OnBasePointChanged(SphericalPoint value)
    {
        // do nothing
    }
}
