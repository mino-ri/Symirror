using System.Collections.Generic;
using System.Linq;

namespace Symirror3.Core
{
    public interface IVectorOperator<T>
    {
        /// <summary>ベクトルの符号を反転します。</summary>
        T Negate(T x);
        /// <summary>ベクトルを加算します。</summary>
        T Add(T x, T y);
        /// <summary>ベクトルを減算します。</summary>
        T Subtract(T x, T y);
        /// <summary>ベクトルのスカラー倍を求めます。</summary>
        T Multiply(T x, double scalar);
        /// <summary>ベクトルのスカラー除を求めます。</summary>
        T Divide(T x, double scalar);
        /// <summary>ベクトルの内積を求めます。</summary>
        double Dot(T x, T y);
        /// <summary>ベクトルの外積を求めます。</summary>
        T Cross(T x, T y);
        /// <summary>ベクトルの絶対値を1にします。</summary>
        T Normalize(T x);
        /// <summary>2つのベクトルがほぼ等しいか判断します。</summary>
        bool NearlyEqual(T x, T y, double error);
        /// <summary>3次元ベクトルを生成します。</summary>
        T Create(double x, double y, double z);
        /// <summary>零ベクトルを取得します。</summary>
        T Zero { get; }
    }

    public static class VectorOperator
    {
        public const double DefaultError = 1.0 / 8192.0;

        public static bool NearlyEqual<T>(this IVectorOperator<T> opr, T x, T y) => opr.NearlyEqual(x, y, DefaultError);

        /// <summary>ベクトル シーケンスの総和を計算します。</summary>
        /// <param name="source">対象のシーケンス。</param>
        /// <returns>対象シーケンスの総和。</returns>
        public static T Sum<T>(this IVectorOperator<T> opr, params T[] source)
        {
            return source.Aggregate(opr.Add);
        }

        /// <summary>ベクトル シーケンスの総和を計算します。</summary>
        /// <param name="source">対象のシーケンス。</param>
        /// <returns>対象シーケンスの総和。</returns>
        public static T Sum<T>(this IVectorOperator<T> opr, IEnumerable<T> source)
        {
            return source.Aggregate(opr.Add);
        }

        /// <summary>ベクトル シーケンスの総和を計算します。</summary>
        /// <param name="source">対象のシーケンス。</param>
        /// <returns>対象シーケンスの総和。</returns>
        public static T Average<T>(this IVectorOperator<T> opr, IEnumerable<T> source)
        {
            return source.Aggregate(
                (vector: opr.Zero, count: 0),
                (acm, value) => (opr.Add(acm.vector, value), acm.count + 1),
                acm => opr.Divide(acm.vector, acm.count));
        }
    }
}
