using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons;

public class PolyhedronSelector<T>
{
    private readonly IVectorOperator<T> _opr;

    public SymmetrySymbol Symbol
    {
        get => Symmetry.Symbol;
        set => Symmetry = new SymmetryGroup(value);
    }

    public SymmetryGroup Symmetry { get; private set; }

    public PolyhedronSelector(IVectorOperator<T> opr, SymmetrySymbol symbol)
    {
        _opr = opr;
        Symmetry = new SymmetryGroup(symbol);
    }

    public PolyhedronBase<T> GetPolyhedron(PolyhedronType polyhedronType)
    {
        return polyhedronType switch
        {
            PolyhedronType.Symmetry => new SymmetrySourcePolyhedron<T>(Symmetry, _opr),
            PolyhedronType.Normal => new NormalPolyhedron<T>(Symmetry, _opr),
            PolyhedronType.Snub => new SnubPolyhedron<T>(Symmetry, _opr),
            PolyhedronType.Ionic1 => new IonicPolyhedron1<T>(Symmetry, _opr),
            PolyhedronType.Ionic2 => new IonicPolyhedron2<T>(Symmetry, _opr),
            PolyhedronType.Dirhombic => new DirhombicPolyhedron<T>(Symmetry, _opr),
            PolyhedronType.Dual => new DualPolyhedron<T>(Symmetry, _opr),
            PolyhedronType.SnubDual => new SnubDualPolyhedron<T>(Symmetry, _opr),
            _ => new NormalPolyhedron<T>(Symmetry, _opr),
        };
    }
}

public enum PolyhedronType
{
    Symmetry,
    Normal,
    Snub,
    Ionic1,
    Ionic2,
    Dirhombic,
    Dual,
    SnubDual,
}
