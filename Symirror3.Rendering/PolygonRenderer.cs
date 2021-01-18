using System;
using System.Collections.Generic;
using System.Numerics;
using IndirectX;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;

namespace Symirror3.Rendering
{
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

        public int LightFactor { get; set; }
        public int ShadowFactor { get; set; }

        public void Render(IEnumerable<PolyhedronFace<Vector3>> polyhedron, Graphics graphics)
        {
            foreach (var polygon in polyhedron)
            {
                RenderCore(polygon, graphics);
            }
        }

        public abstract void OnActivate(Graphics graphics);

        protected abstract void RenderCore(PolyhedronFace<Vector3> polygon, Graphics graphics);

        private float GetDefuseFactor(Vector3 normal, Vector3 light)
        {
            return Math.Max(0f, Vector3.Dot(normal, light));
        }

        private float GetSpeculaFactor(Vector3 normal, Vector3 light, Vector3 sight)
        {
            var half = Vector3.Normalize(light + sight);
            return (float)Math.Pow(Math.Max(0, Vector3.Dot(normal, half)), 5);
        }

        /// <summary>指定したポリゴンに割り当てられる、規定の光適用後の色を取得します。</summary>
        /// <param name="polygon">色を取得するポリゴン。</param>
        /// <param name="graphics"><see cref="IndirectX.EasyRenderer.Graphics"/>オブジェクト。</param>
        protected Color GetLightedColor(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            return GetLightedColor(
                GetBaseColor(polygon.SymmetryElement),
                GetNormal(polygon, graphics),
                polygon.Vertices[0].Vector,
                graphics);
        }

        /// <summary>指定した色をベースに、現在の設定で現在の向きの面に光または影を適用した色を返します。</summary>
        /// <param name="baseColor">ベースカラー。</param>
        /// <param name="normal">法線ベクトル。</param>
        /// <param name="graphics"><see cref="IndirectX.EasyRenderer.Graphics"/>オブジェクト。</param>
        protected Color GetLightedColor(Color baseColor, Vector3 normal, Vector3 location, Graphics graphics)
        {
            // 視線の向いている方向
            var sight = Vector3.Normalize(location * graphics.World + new Vector3(0f, 0f, 5f));

            // 画面に向いているほうを表にするため、必要に応じて法線ベクトルを逆転させる
            if (Vector3.Dot(normal, sight) < 0f)
                normal = -normal;

            // 光源のある方向
            var keyLight = Vector3.Normalize(new(1f, 1f, 0.7f));
            var fillLight = Vector3.Normalize(new(-1f, 0.2f, 0.8f));

            var keyDefuse = GetDefuseFactor(normal, keyLight);
            var fillDefuse = GetDefuseFactor(normal, fillLight);
            var defuse = 1f - (1f - Math.Min(1f, keyDefuse + fillDefuse * 0.3f)) * ShadowFactor / 100f;

            var keySpecula = GetSpeculaFactor(normal, keyLight, sight) * LightFactor / 100f;
            var fillSpecula = GetSpeculaFactor(normal, fillLight, sight) * 0.3f * LightFactor / 100f;
            var specula = keySpecula + fillSpecula;

            if (defuse < 1f)
            {
                var newDefuse = defuse + specula;
                defuse = Math.Min(1f, newDefuse);
                specula = Math.Max(0f, newDefuse - 1f);
            }

            return new Color(
                Math.Min(1f, baseColor.R * defuse + specula),
                Math.Min(1f, baseColor.G * defuse + specula),
                Math.Min(1f, baseColor.B * defuse + specula));
        }

        /// <summary>ポリゴンの法線ベクトルを取得します。</summary>
        /// <param name="polygon">法線ベクトルを取得するポリゴン。</param>
        protected Vector3 GetNormal(PolyhedronFace<Vector3> polygon, Graphics graphics)
        {
            if (polygon.Vertices.Length <= 4)
            {
                return Vector3.Normalize(graphics.World.Transform(Vector3.Cross(
                    polygon.Vertices[1].Vector - polygon.Vertices[0].Vector,
                    polygon.Vertices[2].Vector - polygon.Vertices[0].Vector)));
            }
            else
            {
                return Vector3.Normalize(graphics.World.Transform(Vector3.Cross(
                    polygon.Vertices[2].Vector - polygon.Vertices[0].Vector,
                    polygon.Vertices[4].Vector - polygon.Vertices[0].Vector)));
            }
        }

        /// <summary>指定された対称性要素に割り当てられた色を取得します。</summary>
        /// <param name="symmetryElement">色を取得する対称性要素</param>
        protected Color GetBaseColor(ISymmetryElement symmetryElement)
        {
            return symmetryElement.ElementCategory == SymmetryElementCategory.Face
                ? Colors[(symmetryElement.ElementType + 3) % Colors.Length]
                : Colors[symmetryElement.ElementType % Colors.Length];
        }
    }
}
