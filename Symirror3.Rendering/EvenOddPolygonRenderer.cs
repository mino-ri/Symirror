using System.Numerics;
using Symirror3.Core.Polyhedrons;

namespace Symirror3.Rendering
{
    internal class EvenOddPolygonRenderer : PolygonRenderer
    {
        public override void OnActivate(Graphics graphics)
        {
            graphics.SetFanIndices();
        }

        protected override void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            // 2角形以下は描画キャンセル
            if (polygon.Vertices.Length < 3)
                return;

            var color = GetLightedColor(polygon, graphics);
            var vertices = graphics.Vertices;
            if (polygon.Vertices.Length == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    vertices[i].Vector = polygon.Vertices[i].Vector;
                    vertices[i].Color = color;
                }

                graphics.IgnoreStencil();
                graphics.DrawIndexed(1);
            }
            else
            {
                var center = default(Vector3);

                for (var i = 0; i < polygon.Vertices.Length; i++)
                {
                    var vector = polygon.Vertices[i].Vector;
                    vertices[i + 1].Vector = vector;
                    vertices[i + 1].Color = color;
                    center += vector;
                }

                // [0]は面の中心
                vertices[0].Vector = center / polygon.Vertices.Length;
                vertices[0].Color = color;
                vertices[polygon.Vertices.Length + 1] = vertices[1];
                vertices[polygon.Vertices.Length + 1].Color = color;

                graphics.BeginWriteStencil();
                graphics.DrawIndexed(polygon.Vertices.Length);
                graphics.UseStencilMask();
                graphics.DrawIndexed(polygon.Vertices.Length);
                graphics.ClearStencil();
            }
        }
    }
}
