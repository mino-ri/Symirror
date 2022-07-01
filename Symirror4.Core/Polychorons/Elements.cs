using Symirror4.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Symirror4.Core.Polychorons;

public class PolychoronCell
{
    public List<PolychoronFace> Faces { get; }

    public Vector4 Normal { get; set; }

    public Vector4 Center { get; set; }

    public ISymmetryElement SymmetryElement { get; }

    internal void AddFace(PolychoronFace face) => Faces.Add(face);

    public PolychoronCell(ISymmetryElement symmetryElement)
    {
        SymmetryElement = symmetryElement;
        Faces = new List<PolychoronFace>();
    }

    public PolychoronCell(ISymmetryElement symmetryElement, Vector4 normal)
    {
        SymmetryElement = symmetryElement;
        Faces = new List<PolychoronFace>();
        Normal = normal;
    }
}

public class PolychoronFace
{
    public PolychoronVertex[] Vertices { get; }

    public PolychoronCell Cell1 { get; }

    public PolychoronCell Cell2 { get; }

    public ISymmetryElement SymmetryElement { get; }

    public PolychoronFace(ISymmetryElement symmetryElement, PolychoronCell cell1, PolychoronCell cell2, IEnumerable<PolychoronVertex> vertices)
    {
        SymmetryElement = symmetryElement;
        Vertices = vertices.AsArray();
        Cell1 = cell1;
        Cell2 = cell2;

        Cell1.AddFace(this);
        Cell2.AddFace(this);
    }
}

public class PolychoronVertex
{
    public Vector4 Vector { get; set; }

    public ISymmetryElement SymmetryElement { get; }

    public PolychoronVertex(ISymmetryElement symmetryElement)
    {
        SymmetryElement = symmetryElement;
    }

    public override string ToString() => SymmetryElement.ElementType + ": " + Vector;
}
