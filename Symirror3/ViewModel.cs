using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Symirror3.Core;
using Symirror3.Core.Symmetry;
using Symirror3.Rendering;
using PType = Symirror3.Core.Polyhedrons.PolyhedronType;

#pragma warning disable CA1822 // メンバーを static に設定します
namespace Symirror3
{
    public sealed class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher _dispatcher;

        public UILanguage[] AllLanguages => UILanguage.Default;
        private UILanguage _language = UILanguage.Default[0];
        public UILanguage Language
        {
            get => _language;
            set => SetValue(ref _language, value);
        }

        public SymmetrySymbol[] AllSymbols => SymmetrySymbol.AllSymbols;

        private SymmetrySymbol _symbol = SymmetrySymbol.AllSymbols[12];
        public SymmetrySymbol Symbol
        {
            get => _symbol;
            set => SetValue(ref _symbol, value, v => new ChangeSymbol(v, _polyhedronType.Value));
        }

        private readonly bool[] _faceVisibles = { true, true, true, true, true };
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

        public EnumValue<PType>[] AllPolyhedronTypes { get; }
        private EnumValue<PType> _polyhedronType;
        public EnumValue<PType> PolyhedronType
        {
            get => _polyhedronType;
            set => SetValue(ref _polyhedronType, value, v => new ChangePolyhedronType(v.Value));
        }

        public EnumValue<FaceViewType>[] AllFaceViewTypes { get; }
        private EnumValue<FaceViewType> _faceViewType;
        public EnumValue<FaceViewType> FaceViewType
        {
            get => _faceViewType;
            set => SetValue(ref _faceViewType, value, v => new ChangeFaceViewType(v.Value));
        }

        public EnumValue<FaceRenderType>[] AllFaceRenderTypes { get; }
        private EnumValue<FaceRenderType> _faceRenderType;
        public EnumValue<FaceRenderType> FaceRenderType
        {
            get => _faceRenderType;
            set => SetValue(ref _faceRenderType, value, v => new ChangeFaceRenderType(v.Value));
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
            var index = Math.Clamp(ParseInt(arg), 0, 2);
            var point = _dispatcher.Symmetry.Faces[0][index].Point;
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ICommand MoveBasePointToBisectorCrossCommand { get; }
        public void MoveBasePointToBisectorCross(object? arg)
        {
            var index = Math.Clamp(ParseInt(arg), 0, 2);
            var face = _dispatcher.Symmetry.Faces[0];
            var point = face.GetBisectorCross(index);
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ICommand MoveBasePointToIncenterCommand { get; }
        public void MoveBasePointToIncenter(object? _)
        {
            var face = _dispatcher.Symmetry.Faces[0];
            var point = PolyhedronType.Value switch
            {
                PType.Snub or PType.SnubDual
                    when PolyhedronUtilities.TryGetSnubPoint(Symbol, out var p) => p,
                PType.Dirhombic
                    when PolyhedronUtilities.TryGetDirhombicPoint(Symbol, out var p) => p,
                _ => face.GetIncenter(),
            };
            _dispatcher.SendMessage(new MoveBasePointTo(point));
        }

        public ViewModel(Control control)
        {
            AllPolyhedronTypes = GetEnums<PType>();
            _polyhedronType = AllPolyhedronTypes[1];
            AllFaceViewTypes = GetEnums<FaceViewType>();
            _faceViewType = AllFaceViewTypes[0];
            AllFaceRenderTypes = GetEnums<FaceRenderType>();
            _faceRenderType = AllFaceRenderTypes[0];

            _dispatcher = new Dispatcher(control.Handle, control.Width, control.Height, Symbol);
            ResetRotationCommand = new ActionCommand(ResetRotation);
            MoveBasePointToVertexCommand = new ActionCommand(MoveBasePointToVertex);
            MoveBasePointToBisectorCrossCommand = new ActionCommand(MoveBasePointToBisectorCross);
            MoveBasePointToIncenterCommand = new ActionCommand(MoveBasePointToIncenter);

            _dispatcher.SendMessage(new ChangeSymbol(Symbol, PolyhedronType.Value));
            _dispatcher.SendMessage(new ChangeLight(Light, Shadow));
            _dispatcher.SendMessage(new ChangeFaceRenderType(FaceRenderType.Value));
            _dispatcher.SendMessage(new ChangeFaceViewType(FaceViewType.Value));
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

        private static int ParseInt(object? index)
        {
            return index switch
            {
                int i => i,
                string s when int.TryParse(s, out var i) => i,
                _ => 0,
            };
        }

        public void Dispose() => _dispatcher.Dispose();

        private EnumValue<TEnum>[] GetEnums<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Select(e => new EnumValue<TEnum>(e, this))
                .ToArray();
        }
    }

    public class EnumValue<TEnum> : INotifyPropertyChanged
        where TEnum : struct, Enum
    {
        private readonly ViewModel _parent;

        public TEnum Value { get; }

        public string ViewText => _parent.Language.EnumNames[Value];

        public EnumValue(TEnum value, ViewModel parent)
        {
            Value = value;
            _parent = parent;
            _parent.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.Language))
                    OnPropertyChanged(nameof(ViewText));
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
