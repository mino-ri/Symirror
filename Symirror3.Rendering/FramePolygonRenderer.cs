using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symirror3.Core.Polyhedrons;

namespace Symirror3.Rendering
{
    internal class FramePolygonRenderer : PolygonRenderer
    {
        private List<Vector3> _renderingList = new(Graphics.TriangleCount);

        public override void OnActivate(Graphics graphics)
        {
            graphics.IgnoreStencil();
            graphics.SetStripIndices();
        }

        protected override void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            var vertexCount = polygon.Vertices.Length;
            var center = polygon.Vertices.Aggregate(default(Vector3), (seed, v) => seed + v.Vector) / vertexCount;
            var vertices = graphics.Vertices;

            // 2角形以下は描画キャンセル
            if (vertexCount < 3)
                return;

            _renderingList.Clear();
            var latest = polygon.Vertices.Last().Vector;
            for (var i = 0; i < polygon.Vertices.Length; i++)
            {
                if (!Vector3Operator.ApproximatelyEqual(polygon.Vertices[i].Vector, latest, 1f / 256f))
                {
                    _renderingList.Add(polygon.Vertices[i].Vector);
                    latest = polygon.Vertices[i].Vector;
                }
            }

            vertexCount = _renderingList.Count;

            // 2角形以下は描画キャンセル
            if (vertexCount < 3)
                return;

            SetMaterial(polygon, graphics);

            for (var i = 0; i < vertexCount; i++)
            {
                var target = _renderingList[i];
                var next = Vector3.Normalize(_renderingList[(i + 1) % vertexCount] - target);
                var prev = Vector3.Normalize(_renderingList[(i + vertexCount - 1) % vertexCount] - target);
                var cos = Vector3.Dot(next, prev);

                vertices[i * 2].Vector = target;
                vertices[i * 2 + 1].Vector = target + Vector3.Normalize(next + prev) * (MathF.Sqrt(2f / (1f - cos)) / 16f);

                // 元の頂点よりも遠い位置に「辺の内側の頂点」を作ってしまった場合、面の中央に補正する
                if ((vertices[i * 2].Vector - center).Length() < (vertices[i * 2 + 1].Vector - center).Length())
                    vertices[i * 2].Vector = center;
            }

            vertices[vertexCount * 2] = vertices[0];
            vertices[vertexCount * 2 + 1] = vertices[1];

            graphics.DrawIndexed(vertexCount * 2);
        }
    }
}
