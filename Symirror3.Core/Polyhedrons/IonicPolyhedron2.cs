using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class IonicPolyhedron2<T> : SnubPolyhedron<T>
    {
        public IonicPolyhedron2(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

        protected override IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry)
        {
            // このタイプの半オーダー多面体が作れない場合
            if (symmetry.Symbol.Count(x => x.Numerator % 2 == 0) < 2)
                return base.GetFaces(symmetry);

            // 他2つと違う扱いをする面のタイプ
            var chiralType = symmetry.Symbol[2].Numerator % 2 == 1 ? 2
                           : symmetry.Symbol[1].Numerator % 2 == 1 ? 1
                           : 0;

            // 各面を採用するかどうかのフラグ
            var facePickFlags = new bool?[symmetry.Order];
            // まだ採用するかどうか決定していない面の数
            var leftFace = symmetry.Order - 1;

            facePickFlags[0] = true;

            var queue = new Queue<SymmetryTriangle>();
            queue.Enqueue(symmetry.Faces[0]);

            while (leftFace > 0)
            {
                var targetFace = queue.Dequeue();

                // 0 or 1配置の採用フラグを広げる
                foreach (var f in symmetry.GetAround(targetFace[chiralType]))
                {
                    if (!facePickFlags[f.Index].HasValue)
                    {
                        // 0 or 1配置なので、採用済のものと採用状況を同じにする
                        facePickFlags[f.Index] = facePickFlags[targetFace.Index].Value;
                        queue.Enqueue(f);
                        leftFace--;
                    }
                }

                // 1/2配置の採用フラグを広げる
                for (var vertexOffset = 1; vertexOffset < 3; vertexOffset++)
                {
                    var targetVertex = targetFace[(chiralType + vertexOffset) % 3];
                    var aroundFaces = symmetry.GetAround(targetVertex).ToArray();

                    var baseIndex = -1;
                    var flags = new bool[4];

                    // 2枚連続して採用フラグが決定されている面を探す
                    for (var i = 0; i < aroundFaces.Length; i++)
                    {
                        if (facePickFlags[aroundFaces[i].Index].HasValue &&
                            facePickFlags[aroundFaces[(i + 1) % aroundFaces.Length].Index].HasValue)
                        {
                            baseIndex = i;

                            flags[0] = facePickFlags[aroundFaces[i].Index].Value;
                            flags[1] = facePickFlags[aroundFaces[(i + 1) % aroundFaces.Length].Index].Value;
                            flags[2] = !facePickFlags[aroundFaces[i].Index].Value;
                            flags[3] = !facePickFlags[aroundFaces[(i + 1) % aroundFaces.Length].Index].Value;
                            break;
                        }
                    }

                    if (baseIndex != -1)
                    {
                        for (var i = 2; i < aroundFaces.Length; i++)
                        {
                            var f = aroundFaces[(baseIndex + i) % aroundFaces.Length];
                            if (!facePickFlags[f.Index].HasValue)
                            {
                                facePickFlags[f.Index] = flags[i % 4];
                                queue.Enqueue(f);
                                leftFace--;
                            }
                        }
                    }
                }
            }

            // 回転する面, ひとつの頂点のまわりを、採用されているもののみ繋ぐ
            return symmetry.Vertices
                .Select(v =>
                {
                    var vertices = symmetry
                            .GetAround(v)
                            .Where(x => facePickFlags[x.Index].Value)
                            .Select(f => Vertices[f.Index])
                            .ToArray();

                    if (vertices.Any())
                        return new PolyhedronFace<T>(v, vertices);

                    // 特別扱いしている頂点は、全採用 or 全不採用なので、不採用の場合はひとまわり外側の頂点を採用する
                    var triangles = symmetry
                        .GetAround(v)
                        .ToArray();

                    return new PolyhedronFace<T>(triangles.First(f => f.ElementType == 1),
                        triangles.Select(f =>
                            Vertices[symmetry.GetNexts(f).First(x => f.GetUnsharedVertexIndex(x) == chiralType).Index]));
                });
        }
    }
}
