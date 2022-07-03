using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public class DualPolyhedron<T> : PolyhedronBase<T>
{
    public DualPolyhedron(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

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
        var triangle = Symmetry.Faces[0];
        var distances = triangle.Select(v => SphericalPoint.CatalanPoint(v.Point, value)).ToArray();
        var max = distances.Select(Math.Abs).Max();
        for (var i = 0; i < distances.Length; i++)
            distances[i] /= max;

        for (var i = 0; i < Vertices.Length; i++)
            Vertices[i].Vector = _opr.Convert(Symmetry.Vertices[i].Point * distances[Symmetry.Vertices[i].ElementType]);
    }
}
