using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using Symirror3.Core.Symmetry;
using System.Diagnostics.CodeAnalysis;

namespace Symirror3.Core
{
    /// <summary>幾何学系のユーティリティメソッドを提供します。</summary>
    public static class Sphere
    {
        /// <summary>球面三角形の3つの角の大きさから、角aの対辺Aの角度を求めます。</summary>
        /// <param name="a">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="b">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="c">球面三角形の角の大きさ(ラジアン)。</param>
        /// <returns>角aの対辺Aの角度。</returns>
        public static double OppositeAngle(double a, double b, double c)
        {
            var s = (a + b + c) / 2f;
            return Asin(Sqrt(-(Cos(s) * Cos(s - a)) / (Sin(b) * Sin(c)))) * 2d;
        }

        /// <summary>直角球面三角形の3つの角の大きさから、角aの対辺AのCos値を求めます。</summary>
        /// <param name="a">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="b">球面三角形の角の大きさ(ラジアン)。</param>
        /// <param name="c">球面三角形の角の大きさ(ラジアン)。</param>
        /// <returns>角aの対辺AのCos値。</returns>
        public static double OppositeCos(double a, double b, double c) => (Cos(b) * Cos(c) + Cos(a)) / (Sin(b) * Sin(c));

        private static double OppositeSin<T>(this IVectorOperator<T> opr, T a, T b)
        {
            var cos = opr.Dot(a, b);
            return Sqrt(1.0 - cos * cos);
        }

        /// <summary>球面上の点Aを、点B・Cおよび原点を通る平面で裏返します。</summary>
        /// <param name="a">裏返す対象の点。</param>
        /// <param name="b">裏返しの基準となる点。</param>
        /// <param name="c">裏返しの基準となる点。</param>
        /// <returns>点Aを、平面BCOで裏返した点。</returns>
        public static T Reverse<T>(this IVectorOperator<T> opr, T a, T b, T c)
        {
            var n = opr.Cross(b, c);
            return opr.Subtract(a, opr.Multiply(n, 2.0 * opr.Dot(n, a) / opr.Dot(n, n)));
        }

        /// <summary>シュワルツ三角形から構成された多面体のひとつの面の中心と頂点座標から、双対多面体の指定された面に対応する頂点の原点からの距離します。</summary>
        /// <param name="center">面の中心またはその延長上にある点。</param>
        /// <param name="vertex">頂点の座標。原点からの距離が1である必要があります。</param>
        /// <returns></returns>
        public static double CatalanPoint<T>(this IVectorOperator<T> opr, T center, T vertex)
        {
            return 1.0 / opr.Dot(center, vertex);
        }

        /// <summary>原点とcenterを通る直線が、法線ベクトルnormalを持ち点sを含む平面と交差する位置の原点からの距離を求めます。</summary>
        /// <param name="center">直線を表す単位ベクトル。</param>
        /// <param name="normal">平面の法線ベクトル。</param>
        /// <param name="s">平面に含まれる1点。</param>
        public static double GetCrossPoint<T>(this IVectorOperator<T> opr, T center, T normal, T s)
        {
            return opr.Dot(normal, s) / opr.Dot(normal, center);
        }

        public static T GetBisectorCross<T>(this IVectorOperator<T> opr, T a, T b, T c) =>
            opr.Normalize(opr.Add(
                opr.Multiply(b, opr.OppositeSin(c, a)),
                opr.Multiply(c, opr.OppositeSin(b, a))));

        public static T GetIncenter<T>(this IVectorOperator<T> opr, T a, T b, T c) =>
            opr.Normalize(opr.Sum(
                opr.Multiply(a, opr.OppositeSin(b, c)),
                opr.Multiply(b, opr.OppositeSin(a, c)),
                opr.Multiply(c, opr.OppositeSin(a, b))));

        public static T Lerp<T>(this IVectorOperator<T> opr, T a, T b, double amount)
        {
            if (amount <= 0.0) return a;
            if (amount >= 1.0) return b;

            var omega = Acos(opr.Dot(a, b));
            var invSinOmega = 1.0 / Sin(omega);
            var s1 = Sin((1 - amount) * omega) * invSinOmega;
            var s2 = Sin(amount * omega) * invSinOmega;
            return opr.Add(opr.Multiply(a, s1), opr.Multiply(b, s2));
        }
    }
}
