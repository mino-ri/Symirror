using System;
using System.Numerics;
using Symirror3.Core;

namespace Symirror3.Rendering
{
    public class Vector3Operator : IVectorOperator<Vector3>
    {
        public Vector3 Zero => default;
        public Vector3 Create(double x, double y, double z) => new((float)x, (float)y, (float)z);
        public Vector3 Reverse(Vector3 vector, in SphericalRing ring)
        {
            var normal = (Vector3)ring.Normal;
            return vector - normal * (2f * Vector3.Dot(normal, vector));
        }

        public static bool ApproximatelyEqual(Vector3 x, Vector3 y, double error) =>
            Math.Abs(x.X - y.X) < error &&
            Math.Abs(x.Y - y.Y) < error &&
            Math.Abs(x.Z - y.Z) < error;

        public static Vector3Operator Instance { get; } = new();
    }
}
