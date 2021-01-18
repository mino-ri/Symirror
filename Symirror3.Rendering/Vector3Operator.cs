using System;
using System.Numerics;

namespace Symirror3.Rendering
{
    public class Vector3Operator : Core.IVectorOperator<Vector3>
    {
        public Vector3 Zero => default;
        public Vector3 Add(Vector3 x, Vector3 y) => x + y;
        public Vector3 Create(double x, double y, double z) => new((float)x, (float)y, (float)z);
        public Vector3 Cross(Vector3 x, Vector3 y) => Vector3.Cross(x, y);
        public Vector3 Divide(Vector3 x, double scalar) => x / (float)scalar;
        public double Dot(Vector3 x, Vector3 y) => Vector3.Dot(x, y);
        public Vector3 Multiply(Vector3 x, double scalar) => x * (float)scalar;
        public Vector3 Negate(Vector3 x) => -x;
        public Vector3 Normalize(Vector3 x) => Vector3.Normalize(x);
        public Vector3 Subtract(Vector3 x, Vector3 y) => x - y;
        public bool NearlyEqual(Vector3 x, Vector3 y, double error) =>
            Math.Abs(x.X - y.X) < error &&
            Math.Abs(x.Y - y.Y) < error &&
            Math.Abs(x.Z - y.Z) < error;

        public static Vector3Operator Instance { get; } = new();
    }
}
