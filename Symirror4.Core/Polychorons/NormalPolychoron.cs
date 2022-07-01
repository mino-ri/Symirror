using Symirror4.Core.Symmetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Symirror4.Core.Polychorons;

public class NormalPolychoron : WythoffianPolychoronBase
{
    public NormalPolychoron(SymmetryGroup symmetryGroup) : base(symmetryGroup)
    {
    }

    protected override void GetElements(SymmetryGroup symmetry, out Span<PolychoronCell> cells, out Span<PolychoronFace> faces, out Span<PolychoronVertex> vertices)
    {
        var cellsArray = symmetry.Vertices.Select(x => new PolychoronCell(x, (Vector4)x.Point)).ToArray();
        var verticesArray = symmetry.Cells.Select(x => new PolychoronVertex(x)).ToArray();

        vertices = verticesArray;
        cells = cellsArray;

        faces = symmetry.Edges.Select(x => new PolychoronFace(
            x,
            cellsArray[x.Vertex1.Index],
            cellsArray[x.Vertex2.Index],
            x.GetAroundCells().Select(c => verticesArray[c.Index]))).ToArray();
    }

    protected override void OnBasePointChanged(Vector4 value)
    {
        CopyByDefinision(value);

        var distances = new[]
        {
                (Vector4)Symmetry[0][0].Point * value,
                (Vector4)Symmetry[0][1].Point * value,
                (Vector4)Symmetry[0][2].Point * value,
                (Vector4)Symmetry[0][3].Point * value,
            };

        foreach (var cell in Cells)
        {
            cell.Center = cell.Normal * distances[cell.SymmetryElement.ElementType];
        }
    }
}
