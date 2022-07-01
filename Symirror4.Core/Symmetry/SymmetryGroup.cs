using System.Collections.Generic;
using System.Linq;

namespace Symirror4.Core.Symmetry;

public partial class SymmetryGroup
{
    private readonly SymmetryTetrahedron[] _cells;
    private readonly SymmetryVertex[] _vertices;

    public SymmetrySymbol Symbol { get; }

    internal SymmetryGroup(SymmetrySymbol symbol, SymmetryTetrahedron[] cells, SymmetryVertex[] vertices)
    {
        Symbol = symbol;
        _cells = cells;
        _vertices = vertices;
    }

    /// <summary>この球面充填を構成する球面三角形を列挙します。</summary>
    public IReadOnlyList<SymmetryTetrahedron> Cells => _cells;

    /// <summary>この球面充填の頂点を列挙します。</summary>
    public IReadOnlyList<SymmetryVertex> Vertices => _vertices;

    private SymmetryEdge[]? _edges;
    /// <summary>この球面充填の辺を列挙します。</summary>
    public IReadOnlyList<SymmetryEdge> Edges => _edges ??= GetEdges();

    private SymmetryEdge[] GetEdges()
    {
        var edges = new List<SymmetryEdge>(384);
        var edgeIndex = 0;
        foreach (var v in _vertices)
        {
            foreach (var p in v.AroundCells
                .SelectMany(c => c.Where(x => x.Index > v.Index))
                .Distinct())
            {
                edges.Add(new SymmetryEdge(v, p, edgeIndex++));
            }
        }

        return edges.ToArray();
    }

    /// <summary>この球面充填を構成する球面三角形の数を取得します。</summary>
    public int Order => _cells.Length;

    /// <summary>指定したインデックスを持つこの球面充填を構成する球面三角形を取得します。</summary>
    public SymmetryTetrahedron this[int index] => _cells[index];
}
