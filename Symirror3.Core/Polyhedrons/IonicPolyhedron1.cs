using Symirror3.Core.Symmetry;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Polyhedrons;

/// <summary>* 2p $ q $ 2r で表されるタイプの半オーダー多面体</summary>
public class IonicPolyhedron1<T> : SnubPolyhedron<T>
{
    public IonicPolyhedron1(SymmetryGroup symmetry, IVectorOperator<T> opr) : base(symmetry, opr) { }

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

            // キラル配置の採用フラグを広げる
            foreach (var f in symmetry.GetAround(targetFace[chiralType]))
            {
                if (!facePickFlags[f.Index].HasValue)
                {
                    // キラル配置なので、採用済のものとタイプが同じものを採用する
                    facePickFlags[f.Index] = !facePickFlags[targetFace.Index]!.Value ^ f.ElementType == targetFace.ElementType;
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
                    var flag1 = facePickFlags[aroundFaces[i].Index];
                    var flag2 = facePickFlags[aroundFaces[(i + 1) % aroundFaces.Length].Index];
                    if (flag1.HasValue && flag2.HasValue)
                    {
                        baseIndex = i;

                        flags[0] = flag1.Value;
                        flags[1] = flag2.Value;
                        flags[2] = !flag1.Value;
                        flags[3] = !flag2.Value;
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
        var rotationFaces = symmetry.Vertices
            .Select(v => new PolyhedronFace<T>(v, symmetry
                    .GetAround(v)
                    .Where(x => facePickFlags[x.Index]!.Value)
                    .Select(f => Vertices[f.Index])));

        var snubFaces = new List<PolyhedronFace<T>>();

        // ねじれ面, 採用されていない面を中心に、周囲の頂点を繋ぐ
        foreach (var face in symmetry.Faces.Where(x => !facePickFlags[x.Index]!.Value))
        {
            var aroundFaces = symmetry.GetNexts(face).ToArray();
            var pairFace = aroundFaces.First(x => face.GetUnsharedVertexIndex(x) == chiralType);
            if (face.Index > pairFace.Index)
                continue;

            var pairAroundFaces = symmetry.GetNexts(pairFace).ToArray();
            snubFaces.Add(new PolyhedronFace<T>(face.ElementType == 0 ? pairFace : face, new[]
            {
                    Vertices[aroundFaces.First(x => face.GetUnsharedVertexIndex(x) == (chiralType + 1) % 3).Index],
                    Vertices[aroundFaces.First(x => face.GetUnsharedVertexIndex(x) == (chiralType + 2) % 3).Index],
                    Vertices[pairAroundFaces.First(x => pairFace.GetUnsharedVertexIndex(x) == (chiralType + 2) % 3).Index],
                    Vertices[pairAroundFaces.First(x => pairFace.GetUnsharedVertexIndex(x) == (chiralType + 1) % 3).Index],
                }));
        }

        return rotationFaces.Concat(snubFaces);
    }
}
