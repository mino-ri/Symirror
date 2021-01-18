using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symirror3.Core.Polyhedrons;

namespace Symirror3.Rendering
{
    internal class HoledPolygonRenderer : PolygonRenderer
    {
        public override void OnActivate(Graphics graphics) => graphics.SetStripIndices();

        protected override void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            var vertexCount = polygon.Vertices.Length;
            var center = polygon.Vertices.Aggregate(default(Vector3), (seed, v) => seed + v.Vector) / vertexCount;
            var _buffer = graphics.Vertices;

            // 2角形以下は描画キャンセル
            if (vertexCount < 3)
                return;

            var renderingList = new List<Vector3>();
            var latest = polygon.Vertices.Last().Vector;
            for (var i = 0; i < polygon.Vertices.Length; i++)
            {
                if (!Vector3Operator.Instance.NearlyEqual(polygon.Vertices[i].Vector, latest, 1f / 256f))
                {
                    renderingList.Add(polygon.Vertices[i].Vector);
                    latest = polygon.Vertices[i].Vector;
                }
            }

            vertexCount = renderingList.Count;

            // 2角形以下は描画キャンセル
            if (vertexCount < 3)
                return;

            var color = GetLightedColor(polygon, graphics);

            for (var i = 0; i < vertexCount; i++)
            {
                var target = renderingList[i];
                var next = Vector3.Normalize(renderingList[(i + 1) % vertexCount] - target);
                var prev = Vector3.Normalize(renderingList[(i + vertexCount - 1) % vertexCount] - target);
                var cos = Vector3.Dot(next, prev);

                _buffer[i * 2].Vector = target;
                _buffer[i * 2 + 1].Vector = target + Vector3.Normalize(next + prev) * (MathF.Sqrt(2f / (1f - cos)) / 16f);

                // 元の頂点よりも遠い位置に「辺の内側の頂点」を作ってしまった場合、面の中央に補正する
                if ((_buffer[i * 2].Vector - center).Length() < (_buffer[i * 2 + 1].Vector - center).Length())
                    _buffer[i * 2].Vector = center;
            }

            for (int i = 0; i < vertexCount * 2; i++)
                _buffer[i].Color = color;

            _buffer[vertexCount * 2] = _buffer[0];
            _buffer[vertexCount * 2 + 1] = _buffer[1];

            graphics.Draw(vertexCount * 2);
        }
    }
}
