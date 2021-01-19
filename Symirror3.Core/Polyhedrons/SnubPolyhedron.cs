using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class SnubPolyhedron<T> : WythoffianPolyhedron<T>
    {
        public SnubPolyhedron(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
        {
            // 回転する面, ひとつの頂点のまわりを、ひとつ飛ばしで結ぶ
            var rotationFaces = symmetry.Vertices
                .Select(v => new PolyhedronFace<T>(v, symmetry
                        .GetAround(v)
                        .Where(x => x.ElementType == 0)
                        .Select(f => Vertices[f.Index])));

            // ねじれ面, ひとつの球面三角形のまわりの面を繋ぐ
            var snubFaces = symmetry.Faces
                .Where(f => f.ElementType == 1)
                .Select(f => new PolyhedronFace<T>(f, symmetry.GetNexts(f).Select(n => Vertices[n.Index])));

            return rotationFaces.Concat(snubFaces);
        }
    }
}
