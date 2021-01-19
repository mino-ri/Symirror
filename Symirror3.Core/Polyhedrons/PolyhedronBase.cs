using System.Collections.Generic;
using System.Linq;
using Symirror3.Core.Symmetry;

namespace Symirror3.Core.Polyhedrons
{
    public abstract class PolyhedronBase<T>
    {
        public PolyhedronVertex<T>[] Vertices { get; }
        public PolyhedronFace<T>[] Faces { get; }
        protected readonly IVectorOperator<T> _opr;

        public SymmetryGroup Symmetry { get; }

        private SphericalPoint _basePoint;
        public SphericalPoint BasePoint
        {
            get => _basePoint;
            set
            {
                _basePoint = value;
                OnBasePointChanged(value);
            }
        }

        protected PolyhedronBase(SymmetryGroup symmetry, IVectorOperator<T> opr)
        {
            Symmetry = symmetry;
            _opr = opr;
            _basePoint = new SphericalPoint(0.0, 0.0, 1.0);
            Vertices = GetVertices(symmetry).ToArray();
            Faces = GetFaces(symmetry).ToArray();
            OnBasePointChanged(_basePoint);
        }

        protected abstract IEnumerable<PolyhedronVertex<T>> GetVertices(SymmetryGroup symmetry);
        protected abstract IEnumerable<PolyhedronFace<T>> GetFaces(SymmetryGroup symmetry);
        protected abstract void OnBasePointChanged(SphericalPoint value);
    }
}
