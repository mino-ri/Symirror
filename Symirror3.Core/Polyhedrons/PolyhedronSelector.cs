using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public class PolyhedronSelector<T>
    {
        private readonly IVectorOperator<T> _opr;

        public SymmetrySymbol Symbol
        {
            get => Symmetry.Symbol;
            set => Symmetry = new Symmetry<T>(value, _opr);
        }

        public Symmetry<T> Symmetry { get; private set; }

        public PolyhedronSelector(IVectorOperator<T> opr, SymmetrySymbol symbol)
        {
            _opr = opr;
            Symmetry = new Symmetry<T>(symbol, _opr);
        }

        public PolyhedronBase<T> GetPolyhedron(SolidType solidType)
        {
            return solidType switch
            {
                SolidType.球面三角形 => new SymmetrySourcePolyhedron<T>(Symmetry, _opr),
                SolidType.通常 => new NormalPolyhedron<T>(Symmetry, _opr),
                SolidType.変形 => new SnubPolyhedron<T>(Symmetry, _opr),
                SolidType.半対称1 => new IonicPolyhedron1<T>(Symmetry, _opr),
                SolidType.半対称2 => new IonicPolyhedron2<T>(Symmetry, _opr),
                SolidType.二重斜方 => new DirhombicPolyhedron<T>(Symmetry, _opr),
                SolidType.双対 => new DualPolyhedron<T>(Symmetry, _opr),
                SolidType.変形双対 => new SnubDualPolyhedron<T>(Symmetry, _opr),
                _ => new NormalPolyhedron<T>(Symmetry, _opr),
            };
        }
    }

    public enum SolidType
    {
        球面三角形,
        通常,
        変形,
        半対称1,
        半対称2,
        二重斜方,
        双対,
        変形双対,
    }
}
