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

        public Symmetry<T> Symmetry { get; }

        private T _basePoint;
        public T BasePoint
        {
            get => _basePoint;
            set
            {
                _basePoint = value;
                OnBasePointChanged(value);
            }
        }

        protected PolyhedronBase(Symmetry<T> symmetry, IVectorOperator<T> opr)
        {
            Symmetry = symmetry;
            _opr = opr;
            _basePoint = opr.Create(0.0, 0.0, 1.0);
            Vertices = GetVertices(symmetry).ToArray();
            Faces = GetFaces(symmetry).ToArray();
            OnBasePointChanged(_basePoint);
        }

        protected abstract IEnumerable<PolyhedronVertex<T>> GetVertices(Symmetry<T> symmetry);
        protected abstract IEnumerable<PolyhedronFace<T>> GetFaces(Symmetry<T> symmetry);
        protected abstract void OnBasePointChanged(T value);
    }
}
