using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Symirror3.Core.Symmetry
{
    public class SymmetryTriangle : ISymmetryElement, IReadOnlyList<SymmetryVertex>
    {
        private readonly SymmetryVertex[] _vertices;

        /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
        public int ElementType { get; }

        /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
        public int Index { get; }

        /// <summary><see cref="SymmetryElementCategory.Face"/>を返します。</summary>
        public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Face;

        /// <summary>このオブジェクト自身を返します。</summary>
        public ISymmetryElement SymmetryElement => this;

        /// <summary>多角形を構成する頂点を取得します。</summary>
        public SymmetryVertex this[int index] => _vertices[index];

        int IReadOnlyCollection<SymmetryVertex>.Count => 3;

        internal SymmetryTriangle(int elementType, int index, SymmetryVertex vertex0, SymmetryVertex vertex1, SymmetryVertex vertex2)
        {
            ElementType = elementType;
            Index = index;
            _vertices = new[] { vertex0, vertex1, vertex2 };
        }

        public IEnumerator<SymmetryVertex> GetEnumerator() => ((IEnumerable<SymmetryVertex>)_vertices).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public SphericalRing Edge(int index) =>
            new SphericalRing(_vertices[(index + 1) % 3].Point, _vertices[(index + 2) % 3].Point);

        public SphericalPoint GetBisectorCross(int index)
        {
            var a = _vertices[index].Point;
            var b = _vertices[(index + 1) % 3].Point;
            var c = _vertices[(index + 2) % 3].Point;
            return SphericalPoint.Normalize(
                b * SphericalPoint.OppositeSin(a, c) +
                c * SphericalPoint.OppositeSin(a, b));
        }

        public SphericalPoint GetIncenter()
        {
            var a = _vertices[0].Point;
            var b = _vertices[1].Point;
            var c = _vertices[2].Point;
            return SphericalPoint.Normalize(
                a * SphericalPoint.OppositeSin(b, c) +
                b * SphericalPoint.OppositeSin(a, c) +
                c * SphericalPoint.OppositeSin(a, b));
        }

        /// <summary>この球面三角形と指定した球面三角形が共有している頂点の数を取得します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>共有している頂点の数。</returns>
        internal int CountSharedVertices(SymmetryTriangle other)
        {
            return this.Zip(other, (x, y) => x == y).Count(r => r);
        }

        /// <summary>この球面三角形と指定した球面三角形が共有していない頂点のインデックスを取得します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>共有していない頂点のインデックスの最小値。</returns>
        internal int GetUnsharedVertexIndex(SymmetryTriangle other)
        {
            for (var i = 0; i < 3; i++)
                if (this[i] != other[i]) return i;
            return 0;
        }

        /// <summary>この球面三角形と指定した球面三角形が辺で隣接しているかどうか調査します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>2つの球面三角形が隣接している場合 true。それ以外の場合 false。</returns>
        internal bool IsNext(SymmetryTriangle other)
        {
            if (this == other) return false;

            for (int i = 0; i < 3; i++)
                if (this[i] == other[i] && this[(i + 1) % 3] == other[(i + 1) % 3])
                    return true;

            return false;
        }

        /// <summary>指定された分数のワイソフ記号から、球面充填を構成する1枚目の球面三角形を取得します。</summary>
        /// <param name="a">ワイソフ記号の1要素を表す分数。</param>
        /// <param name="b">ワイソフ記号の1要素を表す分数。</param>
        /// <param name="c">ワイソフ記号の1要素を表す分数。</param>
        /// <param name="createVector">ベクトルのインスタンスを生成する関数。</param>
        public static SymmetryTriangle Create(SymmetrySymbol symbol)
        {
            var angleA = PI / symbol.F0;
            var angleB = PI / symbol.F1;
            var angleC = PI / symbol.F2;

            var cosB = OppositeCos(angleB, angleA, angleC);
            var cosC = OppositeCos(angleC, angleA, angleB);
            var sinB = Sqrt(1d - cosB * cosB);
            var sinC = Sqrt(1d - cosC * cosC);

            return new SymmetryTriangle(0, 0,
                new SymmetryVertex(new(0.0, 0.0, 1.0), 0, 0),
                new SymmetryVertex(new(sinC, 0.0, cosC), 1, 1),
                new SymmetryVertex(new(sinB * Cos(angleA), sinB * Sin(angleA), cosB), 2, 2));

            // A
            // |\
            //b| \c
            // |__\ 
            //C    B
        }

        /// <summary>直角球面三角形の3つの角の大きさから、角aの対辺AのCos値を求めます。</summary>
        /// <param name="a">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="b">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="c">球面三角形の角の大きさ(ラジアン)。</param>
        /// <returns>角aの対辺AのCos値。</returns>
        private static double OppositeCos(double a, double b, double c) => (Cos(b) * Cos(c) + Cos(a)) / (Sin(b) * Sin(c));
    }
}
