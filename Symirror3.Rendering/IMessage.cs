using Symirror3.Core;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;

namespace Symirror3.Rendering
{
    public interface IMessage { }

    public record ChangeLight(LightParameter Parameter, int Value) : IMessage;

    public record MoveBasePoint(float RotateX, float RotateY) : IMessage;

    public record ChangeBasePoint(SphericalPoint Point) : IMessage;

    public record MoveBasePointTo(SphericalPoint To) : IMessage;

    public record MoveBasePointFromTo(SphericalPoint From, SphericalPoint To, int FrameCount) : IMessage;

    public record Rotate(float RotateX, float RotateY, float RotateZ) : IMessage;

    public record ChangeAutoRotation(bool AutoRotation) : IMessage;

    public record ResetRotation() : IMessage;

    public record ChangeSymbol(SymmetrySymbol Symbol, PolyhedronType PolyhedronType) : IMessage;

    public record ChangePolyhedronType(PolyhedronType PolyhedronType) : IMessage;

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
}
