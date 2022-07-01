using Symirror4.Core.Polychorons;
using System.Numerics;

namespace Symirror4.Rendering;

internal class EvenOddPolygonRenderer : PolygonRenderer
{
    public override void OnActivate(Graphics graphics)
    {
        graphics.SetFanIndices();
    }

    protected override void RenderCore(PolychoronFace polygon, Graphics graphics)
    {
        // 2角形以下は描画キャンセル
        if (polygon.Vertices.Length < 3)
            return;

        SetMaterial(polygon, graphics);
        var vertices = graphics.Vertices;
        if (polygon.Vertices.Length == 3)
        {
            for (var i = 0; i < 3; i++)
            {
                vertices[i].Vector = polygon.Vertices[i].Vector;
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
                center += vector;
            }

            // [0]は面の中心
            vertices[0].Vector = center / polygon.Vertices.Length;
            vertices[polygon.Vertices.Length + 1] = vertices[1];

            graphics.BeginWriteStencil();
            graphics.DrawIndexed(polygon.Vertices.Length);
            graphics.UseStencilMask();
            graphics.DrawIndexed(polygon.Vertices.Length);
            graphics.ClearStencil();
        }
    }
}
