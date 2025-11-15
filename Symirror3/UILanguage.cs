using Symirror3.Core.Polyhedrons;
using Symirror3.Rendering;
using System;
using System.Collections.Generic;

namespace Symirror3;

public enum UIText
{
    Symmetry,
    PolyhedronType,
    FacesVisibility,
    LightSetting,
    AutoRotation,
    ResetRotation,
}

public class UILanguage
{
    public string Language { get; init; } = "";

    public Dictionary<UIText, string> Texts { get; } = new();

    public Dictionary<Enum, string> EnumNames { get; } = new();

    public override string ToString() => Language;

    public static UILanguage[] Default { get; } =
    {
            new UILanguage
            {
                Language = "English",
                Texts =
                {
                    [UIText.Symmetry] = "Generator Source: ",
                    [UIText.PolyhedronType] = "Polyhedron Type: ",
                    [UIText.FacesVisibility] = "Faces Visibility",
                    [UIText.LightSetting] = "Light Settings",
                    [UIText.AutoRotation] = "Auto Rotation",
                    [UIText.ResetRotation] = "Reset Rotation",
                },
                EnumNames =
                {
                    [PolyhedronType.Symmetry] = "Symmetry",
                    [PolyhedronType.Normal] = "Normal",
                    [PolyhedronType.Snub] = "Snub",
                    [PolyhedronType.Ionic1] = "Ionic 1",
                    [PolyhedronType.Ionic2] = "Ionic 2",
                    [PolyhedronType.Dirhombic] = "Dirhombic",
                    [PolyhedronType.Dual] = "Dual",
                    [PolyhedronType.SnubDual] = "Snub Dual",
                    [FaceRenderType.Fill] = "Fill",
                    [FaceRenderType.Frame] = "Frame",
                    [FaceRenderType.EvenOdd] = "Even-odd fill",
                    [FaceRenderType.GlobalEvenOdd] = "Global even-odd",
                    [FaceViewType.All] = "All",
                    [FaceViewType.OneEach] = "One for Each Type",
                    [FaceViewType.VertexFigure] = "Vertex Figure",
                    [LightParameter.AmbientLight] = "Ambient light",
                    [LightParameter.DiffuseLight] = "Diffuse light",
                    [LightParameter.SpecularLight] = "Specular light",
                    [LightParameter.SpecularLightSharpness] = "Sharpness",
                    [LightParameter.LightSourceDistance] = "Source distance",
                },
            },
            new UILanguage
            {
                Language = "Japanese(日本語)",
                Texts =
                {
                    [UIText.Symmetry] = "球面三角形：",
                    [UIText.PolyhedronType] = "多面体の種類：",
                    [UIText.FacesVisibility] = "面の表示",
                    [UIText.LightSetting] = "光の設定",
                    [UIText.AutoRotation] = "自動回転",
                    [UIText.ResetRotation] = "回転リセット",
                },
                EnumNames =
                {
                    [PolyhedronType.Symmetry] = "球面三角形",
                    [PolyhedronType.Normal] = "通常",
                    [PolyhedronType.Snub] = "変形",
                    [PolyhedronType.Ionic1] = "半対称1",
                    [PolyhedronType.Ionic2] = "半対称2",
                    [PolyhedronType.Dirhombic] = "二重斜方",
                    [PolyhedronType.Dual] = "双対",
                    [PolyhedronType.SnubDual] = "変形双対",
                    [FaceRenderType.Fill] = "塗りつぶし",
                    [FaceRenderType.Frame] = "枠",
                    [FaceRenderType.EvenOdd] = "偶奇塗り",
                    [FaceRenderType.GlobalEvenOdd] = "包括的偶奇塗り",
                    [FaceViewType.All] = "全て表示",
                    [FaceViewType.OneEach] = "各1枚ずつ",
                    [FaceViewType.VertexFigure] = "頂点形状",
                    [LightParameter.AmbientLight] = "環境光",
                    [LightParameter.DiffuseLight] = "拡散光",
                    [LightParameter.SpecularLight] = "鏡面光",
                    [LightParameter.SpecularLightSharpness] = "鏡面光の鋭さ",
                    [LightParameter.LightSourceDistance] = "光源の距離",
                },
            },
        };
}
