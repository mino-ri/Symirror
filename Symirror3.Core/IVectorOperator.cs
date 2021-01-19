namespace Symirror3.Core
{
    public interface IVectorOperator<T>
    {
        /// <summary>ベクトルを、指定した大円を含む平面で裏返します。</summary>
        /// <param name="vector">裏返すベクトル。</param>
        /// <param name="ring">裏返しの基準となる面に含まれる大円。</param>
        T Reverse(T vector, in SphericalRing ring);
        /// <summary>3次元ベクトルを生成します。</summary>
        T Create(double x, double y, double z);
        /// <summary>零ベクトルを取得します。</summary>
        T Zero { get; }
    }

    public static class VectorOperator
    {
        public const double DefaultError = 1.0 / 8192.0;

        public static T Convert<T>(this IVectorOperator<T> opr, SphericalPoint point) =>
            opr.Create(point.X, point.Y, point.Z);

        public static T Convert<T>(this IVectorOperator<T> opr, Numerics.Vector3D point) =>
            opr.Create(point.X, point.Y, point.Z);
    }
}
