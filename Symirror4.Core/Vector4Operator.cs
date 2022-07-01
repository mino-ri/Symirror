using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Symirror4.Core;

public static class Vector4Operator
{

    /// <summary>玲胞上の点targetを、点A・B・Cおよび原点を通る3次元空間で裏返します。</summary>
    /// <param name="target">裏返す対象の点。</param>
    /// <param name="a">裏返しの基準となる点。</param>
    /// <param name="b">裏返しの基準となる点。</param>
    /// <param name="c">裏返しの基準となる点。</param>
    /// <returns>点targetを、3次元空間OABCで裏返した点。</returns>
    public static Vector4 Reverse(this Vector4 target, Vector4 a, Vector4 b, Vector4 c)
    {
        var (x, y, z, w) = CrossProductD(a, b, c);

        var dot = target.X * x + target.Y * y + target.Z * z + target.W * w;
        var normal = x * x + y * y + z * z + w * w;
        var factor = 2d * dot / normal;

        return new Vector4(
            (float)(target.X - factor * x),
            (float)(target.Y - factor * y),
            (float)(target.Z - factor * z),
            (float)(target.W - factor * w));
    }

    /// <summary>
    /// 指定した4次元ベクトルから外積を演算し、法線ベクトルを求めます。
    /// </summary>
    /// <param name="origin">原点ベクトル。</param>
    /// <param name="a">ベクトル1。</param>
    /// <param name="b">ベクトル2。</param>
    /// <param name="c">ベクトル3。</param>
    public static (double x, double y, double z, double w) CrossProductD(Vector4 a, Vector4 b, Vector4 c)
    {
        var p = (double)b.Z * c.W - b.W * c.Z;
        var q = (double)b.Y * c.W - b.W * c.Y;
        var r = (double)b.Y * c.Z - b.Z * c.Y;
        var s = (double)b.X * c.W - b.W * c.X;
        var t = (double)b.X * c.Z - b.Z * c.X;
        var u = (double)b.X * c.Y - b.Y * c.X;

        return (
         a.Y * p - a.Z * q + a.W * r,
        -a.X * p + a.Z * s - a.W * t,
         a.X * q - a.Y * s + a.W * u,
        -a.X * r + a.Y * t - a.Z * u);
    }

    /// <summary>
    /// ベクトル シーケンスの総和を計算します。
    /// </summary>
    /// <param name="source">対象のシーケンス。</param>
    /// <returns>対象シーケンスの総和。</returns>
    public static Vector4 Sum(this IEnumerable<Vector4> source)
    {
        return source.Aggregate((aqm, val) => aqm + val);
    }
}
