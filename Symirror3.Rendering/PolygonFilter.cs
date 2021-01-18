using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;

namespace Symirror3.Rendering
{
    public class PolygonFilter
    {
        private bool[] _faceVisibles = new bool[] { true, true, true, true, true };
        private bool[] _isShown = new bool[] { false, false, false, false, false };

        private FaceViewType _faceViewType = FaceViewType.全て表示;
        private Vector3 _targetVertex;

        public IEnumerable<PolyhedronFace<Vector3>> Filter(PolyhedronBase<Vector3> source) =>
            _faceViewType switch
            {
                FaceViewType.各1枚のみ => FilterEachOne(source),
                FaceViewType.頂点形状 => FilterVertexFigure(source),
                _ => FilterAll(source),
            };

        public void HandleMessage(ChangeFaceVisible message) => message.FaceVisibles.CopyTo(_faceVisibles.AsSpan());

        public void HandleMessage(ChangeFaceViewType message) => _faceViewType = message.FaceViewType;

        private int GetColorType(PolyhedronFace<Vector3> face) =>
            face.SymmetryElement.ElementCategory == SymmetryElementCategory.Face
                ? (face.SymmetryElement.ElementType + 3) % 5
                : face.SymmetryElement.ElementType % 5;

        private IEnumerable<PolyhedronFace<Vector3>> FilterAll(PolyhedronBase<Vector3> source)
        {
            return source.Faces
                .Where(face => _faceVisibles[GetColorType(face)]);
        }

        private IEnumerable<PolyhedronFace<Vector3>> FilterEachOne(PolyhedronBase<Vector3> source)
        {
            _isShown.AsSpan().Clear();
            return source.Faces
                .Where(face =>
                {
                    var colorType = GetColorType(face);
                    if (_isShown[colorType]) return false;
                    _isShown[colorType] = true;
                    return _faceVisibles[colorType];
                });
        }

        private IEnumerable<PolyhedronFace<Vector3>> FilterVertexFigure(PolyhedronBase<Vector3> source)
        {
            _targetVertex = source.Vertices[0].Vector;
            return source.Faces
              .Where(face =>
              {
                  var colorType = GetColorType(face);
                  return _faceViewType == FaceViewType.頂点形状 &&
                    face.Vertices.All(v => !Vector3Operator.Instance.NearlyEqual(_targetVertex, v.Vector, 1f / 32f))
                      ? false
                      : _faceVisibles[colorType];
              });
        }
    }
}
