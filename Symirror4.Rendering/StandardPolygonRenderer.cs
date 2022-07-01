using Symirror4.Core.Polychorons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Symirror4.Rendering;

internal class StandardPolygonRenderer : PolygonRenderer
{
    private readonly Vector3[] _point2D = new Vector3[Graphics.TriangleCount];
    private readonly Vector3?[] _middles = new Vector3?[Graphics.TriangleCount];
    private readonly Line[] _edges = new Line[Graphics.TriangleCount];
    private readonly List<Vector3> _vertices = new(Graphics.TriangleCount);

    public override void OnActivate(Graphics graphics)
    {
        graphics.IgnoreStencil();
        graphics.SetFanIndices();
    }

    protected override void RenderCore(PolychoronFace<Vector3> polygon, Graphics graphics)
    {
        // 2角形以下は描画キャンセル
        if (polygon.Vertices.Length < 3)
            return;

        var vertices = graphics.Vertices;
        if (polygon.Vertices.Length == 3)
        {
            for (var i = 0; i < 3; i++)
            {
                vertices[i].Vector = polygon.Vertices[i].Vector;
            }

            SetMaterial(polygon, graphics);
            graphics.DrawList(1);
        }
        else
        {
            var center = default(Vector3);

            _vertices.Clear();
            var latest = polygon.Vertices.Last().Vector;
            for (var i = 0; i < polygon.Vertices.Length; i++)
            {
                if (!Vector3Operator.ApproximatelyEqual(polygon.Vertices[i].Vector, latest, 1f / 256f))
                {
                    _vertices.Add(polygon.Vertices[i].Vector);
                    latest = polygon.Vertices[i].Vector;
                    center += polygon.Vertices[i].Vector;
                }
            }

            // 2角形以下は描画キャンセル
            if (_vertices.Count < 3)
                return;

            SetMaterial(polygon, graphics);

            // 3角形は特別な処理をせず描画
            if (_vertices.Count == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    vertices[i].Vector = _vertices[i];
                }

                graphics.DrawList(1);
            }

            center /= _vertices.Count;

            int Before(int i) => (_vertices.Count + i - 1) % _vertices.Count;
            int After(int i) => (i + 1) % _vertices.Count;

            // 重心を原点とした座標系に変換
            for (var i = 0; i < _vertices.Count; i++)
                _point2D[i] = _vertices[i] - center;

            // Z = 0 になるように変換
            var normal = Vector3.Normalize(Vector3.Cross(_point2D[0], _point2D[1]));
            var quot = FromTo(normal, Vector3.UnitZ);
            for (var i = 0; i < _vertices.Count; i++)
                _point2D[i] = Vector3.Transform(_point2D[i], quot);

            // _edges[i] は _point2D[i] と _point2D[i + 1] を結ぶ辺
            for (var i = 0; i < _vertices.Count; i++)
                _edges[i] = Line.FromPoints(_point2D[i], _point2D[After(i)]);

            // _middles[i] は _edges[i] の左右の辺の交点
            for (var i = 0; i < _vertices.Count; i++)
            {
                var middle = Line.Cross(_edges[Before(i)], _edges[After(i)]);
                if (middle.HasValue)
                {
                    var baseDistance = _edges[i].Intercept;
                    var distance = _edges[i].SignedDistance(middle.Value);
                    _middles[i] =
                        0f <= baseDistance && 0f <= distance && distance <= baseDistance ||
                        baseDistance < 0f && baseDistance <= distance && distance <= 0f
                        ? middle
                        : null;
                }
                else
                {
                    _middles[i] = null;
                }
            }

            quot = Quaternion.Inverse(quot);

            for (var i = 0; i < _vertices.Count; i++)
            {
                vertices[i * 3].Vector = Vector3.Transform(_middles[i] ?? Vector3.Zero, quot) + center;
                vertices[i * 3 + 1].Vector = Vector3.Transform(_middles[Before(i)] ?? _point2D[i], quot) + center;
                vertices[i * 3 + 2].Vector = Vector3.Transform(_middles[After(i)] ?? _point2D[After(i)], quot) + center;
            }

            graphics.DrawList(_vertices.Count);
        }
    }

    private static Quaternion FromTo(Vector3 from, Vector3 to)
    {
        var axis = Vector3.Cross(to, from);
        return Vector3Operator.ApproximatelyEqual(axis, Vector3.Zero, 1f / 256f)
            ? Quaternion.Identity
            : Quaternion.CreateFromAxisAngle(Vector3.Normalize(axis), -MathF.Acos(Vector3.Dot(to, from)));
    }
}
