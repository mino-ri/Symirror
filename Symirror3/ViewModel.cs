using Symirror3.Core;
using Symirror3.Core.Symmetry;
using Symirror3.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PType = Symirror3.Core.Polyhedrons.PolyhedronType;
using Win32Control = System.Windows.Forms.Control;

#pragma warning disable CA1822 // メンバーを static に設定します
namespace Symirror3;

public sealed class ViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly System.Windows.Threading.Dispatcher _viewDispatcher;
    private readonly Dispatcher _dispatcher;
    private readonly GeneratorMap _generatorMap;
    private static readonly Brush[] _brushes =
    {
        ColorBrush(0x804000),
        ColorBrush(0xFF4B0A),
        ColorBrush(0xFFD700),
        ColorBrush(0x4ACC0A),
        ColorBrush(0x03AD58),
        ColorBrush(0x40E0FF),
        ColorBrush(0x0070FF),
        ColorBrush(0xC316F0),
        ColorBrush(0xFFFFFF),
        ColorBrush(0xC8C8CB),
    };

    private static Brush ColorBrush(uint value)
    {
        var brush = new SolidColorBrush(Color.FromRgb(
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value));
        brush.Freeze();
        return brush;
    }

    public UILanguage[] AllLanguages => UILanguage.Default;
    private UILanguage _language = UILanguage.Default[0];
    public UILanguage Language
    {
        get => _language;
        set => SetValue(ref _language, value);
    }

    public SymmetrySymbol[] AllSymbols => SymmetryTriangle.AllSymbols;

    private SymmetrySymbol _symbol = SymmetryTriangle.AllSymbols[12];
    public SymmetrySymbol Symbol
    {
        get => _symbol;
        set
        {
            if (SetValue(ref _symbol, value))
            {
                _dispatcher.SendMessage(new ChangeSymbol(_symbol, _polyhedronType.Value));
                _generatorMap.UpdateImage(SymmetryTriangle.Create(_symbol), _polyhedronType.Value);
            }
        }
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

    private readonly int[] _colorIndices = new[] { 4, 2, 1, 5, 6 };

    public Brush FaceBrush0 => _brushes[_colorIndices[0]];
    public Brush FaceBrush1 => _brushes[_colorIndices[1]];
    public Brush FaceBrush2 => _brushes[_colorIndices[2]];
    public Brush FaceBrush3 => _brushes[_colorIndices[3]];
    public Brush FaceBrush4 => _brushes[_colorIndices[4]];

    private void ChangeColor(object? arg, int colorIndex)
    {
        var index = arg as int? ?? int.Parse(arg?.ToString() ?? "");
        if (_colorIndices[index] != colorIndex)
        {
            _colorIndices[index] = colorIndex;
            OnPropertyChanged("FaceBrush" + index);
            _dispatcher.SendMessage(new ChangeColorIndices(_colorIndices));
        }
    }

    public ICommand ChangeColor0Command { get; }
    public void ChangeColor0(object? arg) => ChangeColor(arg, 0);

    public ICommand ChangeColor1Command { get; }
    public void ChangeColor1(object? arg) => ChangeColor(arg, 1);

    public ICommand ChangeColor2Command { get; }
    public void ChangeColor2(object? arg) => ChangeColor(arg, 2);

    public ICommand ChangeColor3Command { get; }
    public void ChangeColor3(object? arg) => ChangeColor(arg, 3);

    public ICommand ChangeColor4Command { get; }
    public void ChangeColor4(object? arg) => ChangeColor(arg, 4);

    public ICommand ChangeColor5Command { get; }
    public void ChangeColor5(object? arg) => ChangeColor(arg, 5);

    public ICommand ChangeColor6Command { get; }
    public void ChangeColor6(object? arg) => ChangeColor(arg, 6);

    public ICommand ChangeColor7Command { get; }
    public void ChangeColor7(object? arg) => ChangeColor(arg, 7);

    public ICommand ChangeColor8Command { get; }
    public void ChangeColor8(object? arg) => ChangeColor(arg, 8);

    public ICommand ChangeColor9Command { get; }
    public void ChangeColor9(object? arg) => ChangeColor(arg, 9);

    public EnumValue<PType>[] AllPolyhedronTypes { get; }
    private EnumValue<PType> _polyhedronType;
    public EnumValue<PType> PolyhedronType
    {
        get => _polyhedronType;
        set
        {
            if (SetValue(ref _polyhedronType, value))
            {
                _dispatcher.SendMessage(new ChangePolyhedronType(_polyhedronType.Value));
                _generatorMap.UpdateImage(SymmetryTriangle.Create(_symbol), _polyhedronType.Value);
            }
        }
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

    public LightParameterViewModel[] LightParameters { get; }

    private double _baseX;
    public double BaseX { get => _baseX; private set => SetValue(ref _baseX, value); }

    private double _baseY;
    public double BaseY { get => _baseY; private set => SetValue(ref _baseY, value); }

    private double _correctedX;
    public double CorrectedX { get => _correctedX; private set => SetValue(ref _correctedX, value); }

    private double _correctedY;
    public double CorrectedY { get => _correctedY; private set => SetValue(ref _correctedY, value); }

    private bool _showCorrectedPoint;
    public bool ShowCorrectedPoint { get => _showCorrectedPoint; private set => SetValue(ref _showCorrectedPoint, value); }

    public ImageSource GeneratorMap => _generatorMap.ImageSource;

    public void SendLightParameter(LightParameter parameter, int value) =>
        _dispatcher.SendMessage(new ChangeLight(parameter, value));

    public void Rotate(double x, double y, double z) => _dispatcher.SendMessage(new Rotate((float)x, (float)y, (float)z));

    public void MoveBasePointRelative(double x, double y)
    {
        var point = new Point(BaseX - x / 4, BaseY - y / 4);
        _dispatcher.SendMessage(new ChangeBasePoint(_generatorMap.DpiToModel(point)));
    }

    public async void MoveBasePointTo(Point point)
    {
        var vp = GetPoints().FirstOrDefault(IsNear);
        if (vp != default)
        {
            _dispatcher.SendMessage(new MoveBasePointTo(vp));
            var sp = _generatorMap.ModelToDpi(in vp);
            CorrectedX = sp.X;
            CorrectedY = sp.Y;
            ShowCorrectedPoint = false;
            await System.Windows.Threading.Dispatcher.Yield();
            ShowCorrectedPoint = true;
        }
        else
        {
            _dispatcher.SendMessage(new MoveBasePointTo(_generatorMap.DpiToModel(point)));
        }

        IEnumerable<SphericalPoint> GetPoints()
        {
            var face = _dispatcher.Symmetry.Faces[0];
            for (var i = 0; i < 3; i++)
            {
                yield return face[i].Point;
                yield return face.GetBisectorCross(i);
            }
            yield return PolyhedronType.Value switch
            {
                PType.Snub or PType.SnubDual when Symbol.TryGetSnubPoint(out var p) => p,
                PType.Dirhombic when Symbol.TryGetDirhombicPoint(out var p) => p,
                _ => face.GetIncenter(),
            };
        }

        bool IsNear(SphericalPoint modelPoint) =>
            (_generatorMap.ModelToDpi(modelPoint) - point).LengthSquared <= 256.0;
    }

    public void ChangeBasePoint(Point point)
    {
        _dispatcher.SendMessage(new ChangeBasePoint(_generatorMap.DpiToModel(point)));
    }

    public ICommand ResetRotationCommand { get; }
    public void ResetRotation(object? _) => _dispatcher.SendMessage(new ResetRotation());

    public ViewModel(Win32Control control, double mapSize, double dpiScale)
    {
        AllPolyhedronTypes = GetEnums<PType>();
        _polyhedronType = AllPolyhedronTypes[1];
        AllFaceViewTypes = GetEnums<FaceViewType>();
        _faceViewType = AllFaceViewTypes[0];
        AllFaceRenderTypes = GetEnums<FaceRenderType>();
        _faceRenderType = AllFaceRenderTypes[0];
        ResetRotationCommand = new ActionCommand(ResetRotation);
        ChangeColor0Command = new ActionCommand(ChangeColor0);
        ChangeColor1Command = new ActionCommand(ChangeColor1);
        ChangeColor2Command = new ActionCommand(ChangeColor2);
        ChangeColor3Command = new ActionCommand(ChangeColor3);
        ChangeColor4Command = new ActionCommand(ChangeColor4);
        ChangeColor5Command = new ActionCommand(ChangeColor5);
        ChangeColor6Command = new ActionCommand(ChangeColor6);
        ChangeColor7Command = new ActionCommand(ChangeColor7);
        ChangeColor8Command = new ActionCommand(ChangeColor8);
        ChangeColor9Command = new ActionCommand(ChangeColor9);

        _viewDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        _generatorMap = new GeneratorMap(mapSize, dpiScale, SymmetryTriangle.Create(_symbol), _polyhedronType.Value);
        _dispatcher = new Dispatcher(control.Handle, control.Width, control.Height, Symbol);
        LightParameters = new[]
        {
            new LightParameterViewModel(LightParameter.AmbientLight, 15, this),
            new LightParameterViewModel(LightParameter.DiffuseLight, 90, this),
            new LightParameterViewModel(LightParameter.SpecularLight, 25, this),
            new LightParameterViewModel(LightParameter.SpecularLightSharpness, 20, this),
            new LightParameterViewModel(LightParameter.LightSourceDistance, 40, this),
        };
        _dispatcher.BasePointChanged += p => _viewDispatcher.Invoke(() =>
        {
            var vp = _generatorMap.ModelToDpi(p);
            BaseX = vp.X;
            BaseY = vp.Y;
        });
        _dispatcher.SendMessage(new ChangeSymbol(Symbol, PolyhedronType.Value));
        _dispatcher.SendMessage(new ChangeFaceRenderType(FaceRenderType.Value));
        _dispatcher.SendMessage(new ChangeFaceViewType(FaceViewType.Value));
        _dispatcher.SendMessage(new ChangeFaceVisible(_faceVisibles));
        _dispatcher.SendMessage(new ChangeColorIndices(_colorIndices));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private bool SetValue<T>(ref T storage, T value, Func<T, IMessage> action, [CallerMemberName] string propertyName = "")
    {
        if (SetValue(ref storage, value, propertyName))
        {
            _dispatcher.SendMessage(action(value));
            return true;
        }

        return false;
    }

    public void Dispose() => _dispatcher.Dispose();

    private EnumValue<TEnum>[] GetEnums<TEnum>() where TEnum : struct, Enum
    {
        return Array.ConvertAll(Enum.GetValues<TEnum>(), e => new EnumValue<TEnum>(e, this));
    }
}

public class EnumValue<TEnum> : INotifyPropertyChanged
    where TEnum : struct, Enum
{
    protected readonly ViewModel _parent;

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

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class LightParameterViewModel : EnumValue<LightParameter>
{
    private int _parameterValue;
    public int ParameterValue
    {
        get => _parameterValue;
        set
        {
            if (_parameterValue == value) return;
            _parameterValue = value;
            OnPropertyChanged();
            _parent.SendLightParameter(Value, ParameterValue);
        }
    }

    public LightParameterViewModel(LightParameter value, int parameterValue, ViewModel parent)
        : base(value, parent)
    {
        _parameterValue = parameterValue;
        _parent.SendLightParameter(value, parameterValue);
    }
}
