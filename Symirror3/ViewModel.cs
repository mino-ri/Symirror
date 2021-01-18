using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Symirror3.Core;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;
using Symirror3.Rendering;

namespace Symirror3
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private Dispatcher _dispatcher;

        public SymmetrySymbol[] AllSymbols => SymmetrySymbol.AllSymbols;

        private SymmetrySymbol _symbol = SymmetrySymbol.AllSymbols[12];
        public SymmetrySymbol Symbol
        {
            get => _symbol;
            set => SetValue(ref _symbol, value, v => new ChangeSymbol(v, _solidType));
        }

        private bool[] _faceVisibles = { true, true, true, true, true };
        public bool FaceVisible0
        {
            get => _faceVisibles[0];
            set => SetValue(ref _faceVisibles[0], value, _ => new ChangeFaceVisible(_faceVisibles));
        }

        public bool FaceVisible1
        {
            get => _faceVisibles[1];
            set => SetValue(ref _faceVisibles[1], value, _ => new ChangeFaceVisible(_faceVisibles));
        }

        public bool FaceVisible2
        {
            get => _faceVisibles[2];
            set => SetValue(ref _faceVisibles[2], value, _ => new ChangeFaceVisible(_faceVisibles));
        }

        public bool FaceVisible3
        {
            get => _faceVisibles[3];
            set => SetValue(ref _faceVisibles[3], value, _ => new ChangeFaceVisible(_faceVisibles));
        }

        public bool FaceVisible4
        {
            get => _faceVisibles[4];
            set => SetValue(ref _faceVisibles[4], value, _ => new ChangeFaceVisible(_faceVisibles));
        }

        public SolidType[] AllSolidTypes => Enum.GetValues<SolidType>();
        private SolidType _solidType = SolidType.通常;
        public SolidType SolidType
        {
            get => _solidType;
            set => SetValue(ref _solidType, value, v => new ChangeSolidType(v));
        }

        public FaceViewType[] AllFaceViewTypes => Enum.GetValues<FaceViewType>();
        private FaceViewType _faceViewType = FaceViewType.全て表示;
        public FaceViewType FaceViewType
        {
            get => _faceViewType;
            set => SetValue(ref _faceViewType, value, v => new ChangeFaceViewType(v));
        }

        public FaceRenderType[] AllFaceRenderTypes => Enum.GetValues<FaceRenderType>();
        private FaceRenderType _faceRenderType = FaceRenderType.通常;
        public FaceRenderType FaceRenderType
        {
            get => _faceRenderType;
            set
            {
                if (SetValue(ref _faceRenderType, value))
                {
                    OnPropertyChanged(nameof(IsRenderTypeHoled));
                    _dispatcher.SendMessage(new ChangeFaceRenderType(value));
                }
            }
        }

        public bool IsRenderTypeHoled
        {
            get => FaceRenderType == FaceRenderType.穴あき;
            set => FaceRenderType = value ? FaceRenderType.穴あき : FaceRenderType.通常;
        }

        private bool _autoRotation;
        public bool AutoRotation
        {
            get => _autoRotation;
            set => SetValue(ref _autoRotation, value, v => new ChangeAutoRotation(v));
        }

        private int _light = 40;
        public int Light
        {
            get => _light;
            set => SetValue(ref _light, value, _ => new ChangeLight(_light, _shadow));
        }

        private int _shadow = 80;
        public int Shadow
        {
            get => _shadow;
            set => SetValue(ref _shadow, value, _ => new ChangeLight(_light, _shadow));
        }

        public ICommand ResetRotationCommand { get; }
        public void ResetRotation(object? _) => _dispatcher.SendMessage(new ResetRotation());

        public void Rotate(float x, float y, float z) => _dispatcher.SendMessage(new Rotate(x, y, z));

        public void MoveBasePoint(float x, float y) => _dispatcher.SendMessage(new MoveBasePoint(x, y));

        public ICommand MoveBasePointToVertexCommand { get; }
        public void MoveBasePointToVertex(object? arg)
        {
            var point = _dispatcher.Symmetry.Faces[0][Math.Clamp(ParseInt(arg), 0, 2)].Vector;
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ICommand MoveBasePointToBisectorCrossCommand { get; }
        public void MoveBasePointToBisectorCross(object? arg)
        {
            var index = Math.Clamp(ParseInt(arg), 0, 2);
            var face = _dispatcher.Symmetry.Faces[0];
            var point = Sphere.GetBisectorCross(Vector3Operator.Instance,
                face[index].Vector, face[(index + 1) % 3].Vector, face[(index + 2) % 3].Vector);
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ICommand MoveBasePointToIncenterCommand { get; }
        public void MoveBasePointToIncenter(object? _)
        {
            var face = _dispatcher.Symmetry.Faces[0];
            var point = SolidType switch
            {
                SolidType.変形 or SolidType.変形双対
                    when Vector3Operator.Instance.TryGetSnubPoint(Symbol, out var p) => p,
                SolidType.二重斜方
                    when Vector3Operator.Instance.TryGetDirhombicPoint(Symbol, out var p) => p,
                _ => Vector3Operator.Instance.GetIncenter(face[0].Vector, face[1].Vector, face[2].Vector),
            };
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ViewModel(Control control)
        {
            _dispatcher = new Dispatcher(control.Handle, control.Width, control.Height, Symbol);
            ResetRotationCommand = new ActionCommand(ResetRotation);
            MoveBasePointToVertexCommand = new ActionCommand(MoveBasePointToVertex);
            MoveBasePointToBisectorCrossCommand = new ActionCommand(MoveBasePointToBisectorCross);
            MoveBasePointToIncenterCommand = new ActionCommand(MoveBasePointToIncenter);

            _dispatcher.SendMessage(new ChangeSymbol(Symbol, SolidType));
            _dispatcher.SendMessage(new ChangeLight(Light, Shadow));
            _dispatcher.SendMessage(new ChangeFaceRenderType(FaceRenderType));
            _dispatcher.SendMessage(new ChangeFaceViewType(FaceViewType));
            _dispatcher.SendMessage(new ChangeFaceVisible(_faceVisibles));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SetValue<T>(ref T storage, T value, Func<T, IMessage> action, [CallerMemberName] string propertyName = "")
        {
            if (SetValue(ref storage, value, propertyName)) _dispatcher.SendMessage(action(value));
        }

        private int ParseInt(object? index)
        {
            return index switch
            {
                int i => i,
                string s when int.TryParse(s, out var i) => i,
                _ => 0,
            };
        }

        public void Dispose() => _dispatcher.Dispose();
    }
}
