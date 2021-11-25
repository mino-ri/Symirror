using Symirror3.Core.Symmetry;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

public class DirhombicPolyhedron<T> : SnubPolyhedron<T>
{
    public DirhombicPolyhedron(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

    protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
    {
        if (symmetry.Symbol[0] != 2)
            return base.GetFaces(symmetry);

        // 回転する面, ひとつの頂点のまわりを、ひとつ飛ばしで結ぶ
        var rotationFaces = symmetry.Vertices
            .SelectMany(v =>
            {
                var baseElementType = (1 + v.ElementType * 2) % 4;
                return new[]
                {
                        new PolyhedronFace<T>(new SymmetryElement(v.ElementCategory, baseElementType, v.Index),
                            symmetry
                            .GetAround(v)
                            .Where(x => x.ElementType == 0)
                            .Select(f => Vertices[f.Index])),
                        new PolyhedronFace<T>(new SymmetryElement(v.ElementCategory, (baseElementType + 1) % 4, v.Index),
                            symmetry
                            .GetAround(v)
                            .Where(x => x.ElementType == 1)
                            .Select(f => symmetry.GetSingleNext(f, v))
                            .Select(f => Vertices[f.Index])),
                };
            });

        // ねじれ面, ひとつの球面三角形のまわりの面を繋ぐ
        var snubFaces = symmetry.Faces
            .Where(f => f.ElementType == 1)
            .Select(f => new PolyhedronFace<T>(
                new SymmetryElement(f.ElementCategory, 1, f.Index),
                symmetry.GetNexts(f).Select(n => Vertices[n.Index])));

        var snubFaces2 = symmetry.Faces
            .Where(f => f.ElementType == 0)
            .Select(f =>
                new PolyhedronFace<T>(
                    new SymmetryElement(f.ElementCategory, 1, f.Index),
                symmetry
                        .GetNexts(f)
                        .Select(next => next[0])
                        .Distinct()
                        .SelectMany(symmetry.GetAround)
                        .Where(next => next.ElementType == 0 && next != f)
                        .Select(n => Vertices[n.Index])));

        return rotationFaces.Concat(snubFaces).Concat(snubFaces2);
    }
}
