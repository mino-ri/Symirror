using System.Numerics;
using Symirror3.Core.Polyhedrons;

namespace Symirror3.Rendering
{
    internal class StandardPolygonRenderer : PolygonRenderer
    {
        public override void OnActivate(Graphics graphics) => graphics.SetFanIndices();

        protected override void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            // 2角形以下は描画キャンセル
            if (polygon.Vertices.Length < 3)
                return;

            var color = GetLightedColor(polygon, graphics);
            var _buffer = graphics.Vertices;
            if (polygon.Vertices.Length == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    _buffer[i].Vector = polygon.Vertices[i].Vector;
                    _buffer[i].Color = color;
                }

                graphics.Draw(1);
            }
            else
            {
                var center = default(Vector3);

                for (var i = 0; i < polygon.Vertices.Length; i++)
                {
                    var vector = polygon.Vertices[i].Vector;
                    _buffer[i + 1].Vector = vector;
                    _buffer[i + 1].Color = color;
                    center += vector;
                }

                // [0]は面の中心
                _buffer[0].Vector = center / polygon.Vertices.Length;
                _buffer[0].Color = color;
                _buffer[polygon.Vertices.Length + 1] = _buffer[1];
                _buffer[polygon.Vertices.Length + 1].Color = color;

                graphics.Draw(polygon.Vertices.Length);
            }
        }
    }
}
