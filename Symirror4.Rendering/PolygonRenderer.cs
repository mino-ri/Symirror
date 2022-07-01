using IndirectX;
using Symirror4.Core.Polychorons;
using Symirror4.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Symirror4.Rendering;

internal abstract class PolygonRenderer
{
    protected static readonly Color[] Colors =
    {
            new Color(0xFF03AF7A),
            new Color(0xFFFFD700),
            new Color(0xFFFF4B0A),
            new Color(0xFF4DC4FF),
            new Color(0xFF005AFF),
        };

    public void Render(IEnumerable<PolychoronFace> Polychoron, Graphics graphics)
    {
        var faces = Polychoron.ToArray();

#if SHADOW_MAP
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
#endif

        foreach (var polygon in faces)
        {
            RenderCore(polygon, graphics);
        }
    }

    public abstract void OnActivate(Graphics graphics);

    protected abstract void RenderCore(PolychoronFace polygon, Graphics graphics);

    /// <summary>指定された対称性要素に割り当てられた色を取得します。</summary>
    /// <param name="symmetryElement">色を取得する対称性要素</param>
    protected static Color GetBaseColor(ISymmetryElement symmetryElement)
    {
        return symmetryElement.ElementCategory == SymmetryElementCategory.Face
            ? Colors[(symmetryElement.ElementType + 3) % Colors.Length]
            : Colors[symmetryElement.ElementType % Colors.Length];
    }

    protected static void SetMaterial(PolychoronFace polygon, Graphics graphics)
    {
        graphics.SetMaterial(GetBaseColor(polygon.SymmetryElement));
    }
}
