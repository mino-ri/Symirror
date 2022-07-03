using IndirectX;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Symirror3.Rendering;

internal abstract class PolygonRenderer
{
    private static readonly Color[] Colors =
    {
        new Color(0xFF804000),
        new Color(0xFFFF4B0A),
        new Color(0xFFFFD700),
        new Color(0xFF4ACC0A),
        new Color(0xFF03AD58),
        new Color(0xFF40E0FF),
        new Color(0xFF0070FF),
        new Color(0xFFC316F0),
        new Color(0xFFFFFFFF),
        new Color(0xFFC8C8CB),
    };

    private int[] ColorIndices = new int[] { 0, 1, 2, 3, 4 };

    public virtual void Render(IEnumerable<PolyhedronFace<Vector3>> polyhedron, Graphics graphics)
    {
        var faces = polyhedron.ToArray();

        try
        {
            graphics.BeginShadowMap();
            graphics.ClearShadowDepth();
            foreach (var polygon in faces)
            {
                RenderCore(polygon, graphics);
            }
        }
        finally
        {
            graphics.EndShadowMap();
        }

        foreach (var polygon in faces)
        {
            RenderCore(polygon, graphics);
        }
    }

    public void OnActivate(Graphics graphics, int[] colorIndexes)
    {
        ColorIndices = colorIndexes;
        OnActivateCore(graphics);
    }

    protected abstract void OnActivateCore(Graphics graphics);

    protected abstract void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics);

    /// <summary>ポリゴンの法線ベクトルを取得します。</summary>
    /// <param name="polygon">法線ベクトルを取得するポリゴン。</param>
    protected static Vector3 GetNormal(PolyhedronFace<Vector3> polygon, Graphics graphics)
    {
        Vector3 normal;
        if (polygon.Vertices.Length <= 4)
        {
            normal = Vector3.Normalize(graphics.World.Transform(Vector3.Cross(
                polygon.Vertices[1].Vector - polygon.Vertices[0].Vector,
                polygon.Vertices[2].Vector - polygon.Vertices[0].Vector)));
        }
        else
        {
            normal = Vector3.Normalize(graphics.World.Transform(Vector3.Cross(
                polygon.Vertices[2].Vector - polygon.Vertices[0].Vector,
                polygon.Vertices[4].Vector - polygon.Vertices[0].Vector)));
        }

        // 視線の向いている方向
        var sight = Vector3.Normalize(polygon.Vertices[0].Vector * graphics.World + new Vector3(0f, 0f, 5f));

        // 画面に向いているほうを表にするため、必要に応じて法線ベクトルを逆転させる
        if (Vector3.Dot(normal, sight) < 0f)
            normal = -normal;

        return normal;
    }

    /// <summary>指定された対称性要素に割り当てられた色を取得します。</summary>
    /// <param name="symmetryElement">色を取得する対称性要素</param>
    protected Color GetBaseColor(ISymmetryElement symmetryElement)
    {
        return Colors[symmetryElement.ElementCategory == SymmetryElementCategory.Face
            ? ColorIndices[(symmetryElement.ElementType + 3) % ColorIndices.Length]
            : ColorIndices[symmetryElement.ElementType % ColorIndices.Length]];
    }

    protected void SetMaterial(PolyhedronFace<Vector3> polygon, Graphics graphics)
    {
        graphics.SetMaterial(GetBaseColor(polygon.SymmetryElement), GetNormal(polygon, graphics));
    }
}
