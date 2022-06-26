using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using IndirectX.EasyRenderer;
using Symirror3.Core.Polyhedrons;

namespace Symirror3.Rendering;

internal class GlobalEvenOddPolygonRenderer : EvenOddPolygonRenderer
{
    const double error = 1d / 128d;
    private int _vertexIndex = 0;
    private int _indexIndex = 0;
    private readonly ushort[] _indices = new ushort[Graphics.TriangleCount * 3];

    protected override void OnActivateCore(Graphics graphics) { }

    public override void Render(IEnumerable<PolyhedronFace<Vector3>> polyhedron, Graphics graphics)
    {
        var faces = polyhedron
            .Select(face =>
            {
                var (normal, distance) = GetPlaneExpression(face);
                return new
                {
                    Normal = normal,
                    Distance = distance,
                    Faces = new List<PolyhedronFace<Vector3>>(new[] { face }),
                };
            })
            .ToArray();

        for (var i = 0; i < faces.Length; i++)
        {
            var current = faces[i];
            if (current.Faces[0].Vertices.Length < 3 || current.Normal == default && current.Distance == default)
            {
                current.Faces.Clear();
                continue;
            }

            for (var j = 0; j < i; j++)
            {
                var target = faces[j];
                if (target.Faces.Count > 0 &&
                    (Math.Abs(current.Distance - target.Distance) < error &&
                     Vector3Operator.ApproximatelyEqual(current.Normal, target.Normal, error) ||
                     Math.Abs(current.Distance + target.Distance) < error &&
                     Vector3Operator.ApproximatelyEqual(current.Normal, -target.Normal, error)))
                {
                    target.Faces.Add(current.Faces[0]);
                    current.Faces.Clear();
                    break;
                }
            }
        }

        try
        {
            graphics.BeginShadowMap();
            graphics.ClearShadowDepth();
            foreach (var face in faces)
            {
                RenderCoreForShadow(face.Faces, graphics);
            }
        }
        finally
        {
            graphics.EndShadowMap();
        }

        foreach (var face in faces)
        {
            RenderCore(face.Faces, graphics);
        }
    }

    private void SetPolygonToBuffer(PolyhedronFace<Vector3> polygon, Graphics graphics)
    {
        var vertices = graphics.Vertices;
        var center = default(Vector3);
        for (var i = 0; i < polygon.Vertices.Length; i++)
        {
            var vector = polygon.Vertices[i].Vector;
            vertices[_vertexIndex + i].Vector = vector;
            center += vector;
        }

        vertices[_vertexIndex + polygon.Vertices.Length].Vector = center / polygon.Vertices.Length;

        for (var i = 0; i < polygon.Vertices.Length; i++)
        {
            _indices[_indexIndex + i * 3 + 0] = (ushort)(_vertexIndex + polygon.Vertices.Length);
            _indices[_indexIndex + i * 3 + 1] = (ushort)(_vertexIndex + i);
            _indices[_indexIndex + i * 3 + 2] = (ushort)(_vertexIndex + (i + 1) % polygon.Vertices.Length);
        }

        _vertexIndex += polygon.Vertices.Length + 1;
        _indexIndex += polygon.Vertices.Length * 3;
    }

    private void RenderCoreForShadow(List<PolyhedronFace<Vector3>> polygons, Graphics graphics)
    {
        if (polygons.Count == 1)
        {
            graphics.SetFanIndices();
            RenderCore(polygons[0], graphics);
        }
        else if (polygons.Count > 1)
        {
            SetMaterial(polygons[0], graphics);

            // リセット
            _indices.AsSpan().Clear();
            _vertexIndex = 0;
            _indexIndex = 0;
            var vertexCount = 0;

            // 影用はマテリアルの変化が不要なためステンシルバッファ設定～描画まで一気に行う
            foreach (var polygon in polygons)
            {
                vertexCount += polygon.Vertices.Length;
                if (vertexCount > Graphics.TriangleCount) break;
                SetPolygonToBuffer(polygon, graphics);
            }

            var triangleCount = _indexIndex / 3;
            graphics.BeginWriteStencil();
            graphics.SetIndices(_indices);
            graphics.DrawIndexed(triangleCount);
            graphics.UseStencilMask();
            graphics.DrawIndexed(triangleCount);
            graphics.ClearStencil();
        }
    }

    private void RenderCore(List<PolyhedronFace<Vector3>> polygons, Graphics graphics)
    {
        if (polygons.Count == 1)
        {
            graphics.SetFanIndices();
            RenderCore(polygons[0], graphics);
        }
        else if (polygons.Count > 1)
        {
            SetMaterial(polygons[0], graphics);

            // リセット
            _indices.AsSpan().Clear();
            _vertexIndex = 0;
            _indexIndex = 0;
            var polygonIndexCounts = new int[polygons.Count];
            var vertexCount = 0;

            // 全体のステンシルバッファ設定
            for (var i = 0; i < polygons.Count; i++)
            {
                vertexCount += polygons[i].Vertices.Length;
                if (vertexCount > Graphics.TriangleCount) break;

                SetPolygonToBuffer(polygons[i], graphics);
                polygonIndexCounts[i] = _indexIndex;
            }

            var triangleCount = _indexIndex / 3;
            graphics.BeginWriteStencil();
            graphics.SetIndices(_indices);
            graphics.DrawIndexed(triangleCount);
            graphics.UseStencilMask();

            // 1枚ずつ描画
            var indexIndex = 0;
            vertexCount = 0;

            for (var i = 0; i < polygons.Count; i++)
            {
                vertexCount += polygons[i].Vertices.Length;
                if (vertexCount > Graphics.TriangleCount) break;

                SetMaterial(polygons[i], graphics);
                graphics.SetIndices(_indices.AsSpan()[indexIndex..polygonIndexCounts[i]]);
                graphics.DrawIndexed((polygonIndexCounts[i] - indexIndex) / 3);
                indexIndex = polygonIndexCounts[i];
            }

            graphics.ClearStencil();
        }
    }

    private (Vector3, double) GetPlaneExpression(PolyhedronFace<Vector3> polygon)
    {
        if (polygon.Vertices.Length < 3)
            return default;

        Vector3 normal;
        if (polygon.Vertices.Length <= 4)
        {
            normal = Vector3.Normalize(Vector3.Cross(
                polygon.Vertices[1].Vector - polygon.Vertices[0].Vector,
                polygon.Vertices[2].Vector - polygon.Vertices[0].Vector));
        }
        else
        {
            normal = Vector3.Normalize(Vector3.Cross(
                polygon.Vertices[2].Vector - polygon.Vertices[0].Vector,
                polygon.Vertices[4].Vector - polygon.Vertices[0].Vector));
        }

        if (Vector3Operator.ApproximatelyEqual(Vector3.Zero, normal, error))
            return default;

        if (normal.Z < 0.0) normal = -normal;
        return (normal, Vector3.Dot(normal, polygon.Vertices[0].Vector));
    }
}
