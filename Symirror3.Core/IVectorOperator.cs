namespace Symirror3.Core;

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

    public T Convert(in SphericalPoint point) => Create(point.X, point.Y, point.Z);

    public T Convert(in Numerics.Vector3D point) => Create(point.X, point.Y, point.Z);
}