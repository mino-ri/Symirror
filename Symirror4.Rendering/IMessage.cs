using Symirror4.Core;
using Symirror4.Core.Polychorons;
using Symirror4.Core.Symmetry;

namespace Symirror4.Rendering;

public interface IMessage { }

public record ChangeLight(LightParameter Parameter, int Value) : IMessage;

public record MoveBasePoint(float RotateX, float RotateY) : IMessage;

public record ChangeBasePoint(GlomericPoint Point) : IMessage;

public record MoveBasePointTo(GlomericPoint To) : IMessage;

public record MoveBasePointFromTo(GlomericPoint From, GlomericPoint To, int FrameCount) : IMessage;

public record Rotate(float RotateX, float RotateY, float RotateZ) : IMessage;

public record ChangeAutoRotation(bool AutoRotation) : IMessage;

public record ResetRotation() : IMessage;

public record ChangeSymbol(SymmetrySymbol Symbol, PolychoronType PolychoronType) : IMessage;

public record ChangePolychoronType(PolychoronType PolychoronType) : IMessage;

public record ChangeFaceVisible(bool[] FaceVisibles) : IMessage;

public record ChangeFaceViewType(FaceViewType FaceViewType) : IMessage;

public record ChangeFaceRenderType(FaceRenderType FaceRenderType) : IMessage;

public enum LightParameter
{
    AmbientLight,
    DiffuseLight,
    SpecularLight,
    SpecularLightSharpness,
    LightSourceDistance,
}

public enum FaceViewType
{
    All,
    OneEach,
    VertexFigure,
}

public enum FaceRenderType
{
    Fill,
    EvenOdd,
    Frame,
}
