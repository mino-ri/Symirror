using Symirror4.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Symirror4.Core.Polychorons;

public abstract class PolychoronBase
{
    public PolychoronCell[] Cells { get; protected set; }
    public PolychoronFace[] Faces { get; protected set; }
    public PolychoronVertex[] Vertices { get; protected set; }

    protected SymmetryGroup Symmetry { get; }

    protected PolychoronBase(SymmetryGroup symmetryGroup)
    {
        Symmetry = symmetryGroup;
        GetElements(symmetryGroup, out var cells, out var faces, out var vertices);
        Cells = cells.ToArray();
        Faces = faces.ToArray();
        Vertices = vertices.ToArray();
        BasePoint = Vector4.UnitW;
    }

    private Vector4 _basePoint;
    public Vector4 BasePoint
    {
        get => _basePoint;
        set
        {
            _basePoint = value;
            OnBasePointChanged(value);
        }
    }

    protected abstract void GetElements(SymmetryGroup symmetry, out Span<PolychoronCell> cells, out Span<PolychoronFace> faces, out Span<PolychoronVertex> vertices);
    protected abstract void OnBasePointChanged(Vector4 value);
}

public enum PolychoronType
{
    Standard,
}
