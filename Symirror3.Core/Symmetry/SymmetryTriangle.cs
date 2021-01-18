using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Symirror3.Core.Symmetry
{
    public class SymmetryTriangle<T> : ISymmetryElement, IReadOnlyList<SymmetryVertex<T>>
    {
        private readonly SymmetryVertex<T>[] _vertices;

        /// <summary>同じカテゴリの要素内での、区別のための種別番号を取得します。</summary>
        public int ElementType { get; }

        /// <summary>同じ種類の要素内での、区別のためのインデックスを取得します。</summary>
        public int Index { get; }

        /// <summary><see cref="SymmetryElementCategory.Face"/>を返します。</summary>
        public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Face;

        /// <summary>このオブジェクト自身を返します。</summary>
        public ISymmetryElement SymmetryElement => this;

        /// <summary>多角形を構成する頂点を取得します。</summary>
        public SymmetryVertex<T> this[int index] => _vertices[index];

        int IReadOnlyCollection<SymmetryVertex<T>>.Count => 3;

        internal SymmetryTriangle(int elementType, int index, params SymmetryVertex<T>[] vertexes)
        {
            ElementType = elementType;
            Index = index;
            _vertices = vertexes;
        }

        public IEnumerator<SymmetryVertex<T>> GetEnumerator() => ((IEnumerable<SymmetryVertex<T>>)_vertices).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>この球面三角形と指定した球面三角形が共有している頂点の数を取得します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>共有している頂点の数。</returns>
        internal int CountSharedVertices(SymmetryTriangle<T> other)
        {
            return this.Zip(other, (x, y) => x == y).Count(r => r);
        }

        /// <summary>この球面三角形と指定した球面三角形が共有していない頂点のインデックスを取得します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>共有していない頂点のインデックスの最小値。</returns>
        internal int GetUnsharedVertexIndex(SymmetryTriangle<T> other)
        {
            for (var i = 0; i < 3; i++)
                if (this[i] != other[i]) return i;
            return 0;
        }

        /// <summary>この球面三角形と指定した球面三角形が辺で隣接しているかどうか調査します。</summary>
        /// <param name="other">調査する球面三角形。</param>
        /// <returns>2つの球面三角形が隣接している場合 true。それ以外の場合 false。</returns>
        internal bool IsNext(SymmetryTriangle<T> other)
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
        internal static SymmetryTriangle<T> CreateTrianglCore(IVectorOperator<T> opr, Fraction a, Fraction b, Fraction c)
        {
            var angleA = PI / a;
            var angleB = PI / b;
            var angleC = PI / c;

            var cosB = Sphere.OppositeCos(angleB, angleA, angleC);
            var cosC = Sphere.OppositeCos(angleC, angleA, angleB);
            var sinB = Sqrt(1d - cosB * cosB);
            var sinC = Sqrt(1d - cosC * cosC);

            return new SymmetryTriangle<T>(0, 0,
                new SymmetryVertex<T>(opr.Create(0.0, 0.0, 1.0), 0, 0),
                new SymmetryVertex<T>(opr.Create(sinC, 0f, cosC), 1, 1),
                new SymmetryVertex<T>(opr.Create(sinB * Cos(angleA), sinB * Sin(angleA), cosB), 2, 2));

            // A
            // |\
            //b| \c
            // |__\ 
            //C    B
        }
    }
}
