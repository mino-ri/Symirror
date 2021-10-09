using System;
using System.Numerics;

namespace Symirror3.Rendering
{
    internal struct Line
    {
        public float XFactor;
        public float YFactor;
        public float Intercept;

        public Line(float xFactor, float yFactor, float intercept)
        {
            var d = MathF.Sqrt(xFactor * xFactor + yFactor * yFactor);
            XFactor = xFactor / d;
            YFactor = yFactor / d;
            Intercept = intercept / d;
        }

        public static Line FromPoints(Vector3 p1, Vector3 p2)
        {
            if (Vector3Operator.ApproximatelyEqual(p1, p2, Error))
                throw new InvalidOperationException();
            return new Line(p1.Y - p2.Y, p2.X - p1.X, p1.X * p2.Y - p2.X * p1.Y);
        }

        public float SignedDistance(Vector3 point) => XFactor * point.X + YFactor * point.Y + Intercept;

        private const float Error = 1f / 512f;

        public static Vector3? Cross(Line line1, Line line2)
        {
            var divider = line1.XFactor * line2.YFactor - line2.XFactor * line1.YFactor;
            if (-Error <= divider && divider <= Error)
                return null;

            return new Vector3(
                (line1.YFactor * line2.Intercept - line2.YFactor * line1.Intercept) / divider,
                -((line1.XFactor * line2.Intercept - line2.XFactor * line1.Intercept) / divider),
                0f);
        }
    }
}
