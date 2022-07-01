using System;
using System.Collections.Generic;

namespace Symirror4.Core.Symmetry;

public class SymmetryEdge : ISymmetryElement
{
    private static readonly int[,] _edgeIndices =
    {
            // vert0: (0, 1, 4),
            // vert1: (0, 3, 2),
            // vert2: (5, 1, 2),
            // vert3: (5, 3, 4),

            //0  1  2  3
            { 0, 0, 1, 4 }, // 0
            { 0, 1, 2, 3 }, // 1
            { 1, 2, 2, 5 }, // 2
            { 4, 3, 5, 3 }, // 3
        };

    public SymmetryElementCategory ElementCategory => SymmetryElementCategory.Edge;

    public int ElementType { get; }

    public int Index { get; }

    public SymmetryVertex Vertex1 { get; }

    public SymmetryVertex Vertex2 { get; }

    public SymmetryEdge(SymmetryVertex vertex1, SymmetryVertex vertex2, int index)
    {
        ElementType = _edgeIndices[vertex1.ElementType, vertex2.ElementType];
        Index = index;
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }

    /// <summary>この辺を囲っている胞を、隣合う順に列挙します。</summary>
    public IEnumerable<SymmetryTetrahedron> GetAroundCells()
    {
        SymmetryTetrahedron baseCell = null!;
        foreach (var cell1 in Vertex1.AroundCells)
        {
            foreach (var cell2 in Vertex2.AroundCells)
            {
                if (cell1 == cell2)
                {
                    baseCell = cell1;
                    break;
                }
            }
        }

        var nextTypes = stackalloc[] { -1, -1 };
        for (var i = 0; i < 4; i++)
        {
            if (i != Vertex1.ElementType && i != Vertex2.ElementType)
                nextTypes[nextTypes[0] == -1 ? 0 : 1] = i;
        }

        for (var cell = baseCell; cell != baseCell;)
        {
            yield return cell;
            cell = cell.GetNext(nextTypes[0]) ?? throw new Exception();
            yield return cell;
            cell = cell.GetNext(nextTypes[1]) ?? throw new Exception();
        }
    }
}
