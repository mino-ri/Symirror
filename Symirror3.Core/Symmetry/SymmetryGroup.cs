using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core.Symmetry;

public class SymmetryGroup
{
    private readonly SymmetryTriangle[] _faces;
    private readonly SymmetryVertex[] _vertices;

    public SymmetrySymbol Symbol { get; }

    /// <summary>この球面充填を構成する球面三角形を列挙します。</summary>
    public IReadOnlyList<SymmetryTriangle> Faces => _faces;

    /// <summary>この球面充填の頂点リストを列挙します。頂点は、その種類によってグループ化されています。</summary>
    public IReadOnlyList<SymmetryVertex> Vertices => _vertices;

    /// <summary>この球面充填を構成する球面三角形の数を取得します。</summary>
    public int Order => _faces.Length;

    /// <summary>指定したインデックスを持つこの球面充填を構成する球面三角形を取得します。</summary>
    public SymmetryTriangle this[int index] => _faces[index];

    public SymmetryGroup(SymmetrySymbol symbol)
    {
        Symbol = symbol;
        (_vertices, _faces) = new Builder(symbol).GetTriangles();
    }

    /// <summary>指定した頂点の周囲にある面を、隣接する順に列挙します。</summary>
    /// <param name="vertex">調査する頂点。</param>
    /// <returns>指定した頂点の周囲にある面のシーケンス。</returns>
    public IEnumerable<SymmetryTriangle> GetAround(SymmetryVertex vertex)
    {
        var list = _faces.Where(t => t.Contains(vertex)).ToList();
        if (list.Count == 0) yield break;

        var face = list[0];
        while (face != null && list.Count > 0)
        {
            yield return face;
            list.Remove(face);
            face = list.FirstOrDefault(face.IsNext);
        }
    }

    /// <summary>指定した面と辺を介して隣接している面を列挙します。</summary>
    /// <param name="face">調査する面。</param>
    /// <returns>face と辺を介して隣接している面のシーケンス。</returns>
    public IEnumerable<SymmetryTriangle> GetNexts(SymmetryTriangle face)
    {
        var list = _faces.Where(face.IsNext).ToList();
        if (list.Count == 0) yield break;

        var result = list[0];
        while (result != null && list.Count > 0)
        {
            yield return result;
            list.Remove(result);
            result = list.FirstOrDefault(f => result.CountSharedVertices(f) > 0);
        }
    }

    /// <summary>指定した面と辺を介して隣接しており、指定した点を含まない面を取得します。</summary>
    /// <param name="face"></param>
    /// <param name="point"></param>
    public SymmetryTriangle GetSingleNext(SymmetryTriangle face, SymmetryVertex point)
    {
        return _faces.First(f => f[point.ElementType] != point && face.IsNext(f));
    }

    private class Builder
    {
        private readonly SymmetrySymbol _symbol;
        // それぞれのElementTypeを持つ頂点にいくつの面が集まるか
        private readonly int[] _faceCounts;
        // 最終的な出力となる面
        private readonly List<SymmetryTriangle> _faces;
        // 各頂点と、現在集まっている面の数のペア
        private readonly Dictionary<SymmetryVertex, int> _vertices;
        // 次に周囲を探索する faces のインデックス
        private int _faceIndex;

        public Builder(SymmetrySymbol symbol)
        {
            var firstFace = SymmetryTriangle.Create(symbol);
            _symbol = symbol;
            _faceCounts = _symbol.Select(s => s.Numerator * 2).ToArray();
            _faces = new() { firstFace };
            _faceIndex = 0;
            _vertices = new();

            foreach (var v in firstFace)
                _vertices[v] = 1;
        }

        public (SymmetryVertex[], SymmetryTriangle[]) GetTriangles()
        {
            while (_vertices.Any(kv => kv.Value < _faceCounts[kv.Key.ElementType]))
            {
                for (var i = 0; i < 3; i++)
                    Reverse(i);

                _faceIndex++;
                if (_faceIndex > 120) throw new ApplicationException("指定されたワイソフ記号から、球面を充填できませんでした。");
            }

            return (_vertices.Keys.ToArray(), _faces.ToArray());
        }

        // faces[targetIndex]の、reverseIndex を裏返して別の面を作る
        private void Reverse(int reverseIndex)
        {
            var newFace = _faces[_faceIndex].ToArray();

            // reverseIndex を含まない辺で対象の頂点を裏返す

            var vector = new SphericalRing(newFace[(reverseIndex + 1) % 3].Point, newFace[(reverseIndex + 2) % 3].Point)
                .Reverse(newFace[reverseIndex].Point);

            // 同一とみなせる頂点を探す
            if (_vertices.Keys.FirstOrDefault(v => v.ElementType == reverseIndex && SphericalPoint.ApproximatelyEqual(v.Point, vector)) is { } vertex)
            {
                newFace[reverseIndex] = vertex;
                // 見付かった頂点にこれ以上辺を追加できないか、同一の面が存在する場合
                if (_vertices[vertex] >= _faceCounts[reverseIndex] || _faces.Any(newFace.SequenceEqual))
                    return;
            }
            else
            {
                newFace[reverseIndex] = new SymmetryVertex(vector, reverseIndex, _vertices.Count);
                _vertices[newFace[reverseIndex]] = 0;
            }

            foreach (var v in newFace)
                _vertices[v] += 1;

            _faces.Add(new SymmetryTriangle(_faces[_faceIndex].ElementType ^ 1, _faces.Count,
                newFace[0], newFace[1], newFace[2]));
        }
    }
}
