using Symirror4.Core.Symmetry;
using System.Numerics;

namespace Symirror4.Core.Polychorons;

public abstract class WythoffianPolychoronBase : PolychoronBase
{
    private CopyDefinition[]? _copyDefinitions;
    protected CopyDefinition[] CopyDefinitions => _copyDefinitions ??= GetCopyDefinition();

    protected WythoffianPolychoronBase(SymmetryGroup symmetryGroup) : base(symmetryGroup)
    {
    }

    protected void CopyByDefinision(Vector4 value)
    {
        Vertices[0].Vector = value;
        for (var i = 1; i < Vertices.Length; i++)
        {
            var sourceIndex = CopyDefinitions[i].Source;
            var reverceFace = CopyDefinitions[i].ReverseFace;

            Vertices[i].Vector = Vertices[sourceIndex].Vector.Reverse(
                (Vector4)Symmetry[sourceIndex][(reverceFace + 1) % 4].Point,
                (Vector4)Symmetry[sourceIndex][(reverceFace + 2) % 4].Point,
                (Vector4)Symmetry[sourceIndex][(reverceFace + 3) % 4].Point);
        }
    }

    private CopyDefinition[] GetCopyDefinition()
    {
        var count = Symmetry.Order;
        var result = new CopyDefinition[count];
        for (var index = 1; index < count; index++)
        {
            var current = Symmetry[index];
            for (var i = 0; i < 4; i++)
            {
                var next = current.GetNext(i);
                if (next?.Index < current.Index)
                {
                    result[index] = new CopyDefinition(next.Index, i);
                    break;
                }
            }
        }
        return result;
    }

    // 対象のインデックスの頂点が、どの頂点をどの辺で反転して得られるものかを表す
    protected struct CopyDefinition
    {
        // コピー元となる球面三角形のインデックス
        public int Source { get; }
        // コピーに使用する面のElementType
        public int ReverseFace { get; }

        public CopyDefinition(int source, int reverse)
        {
            Source = source;
            ReverseFace = reverse;
        }

        public override string ToString() => $"{Source} <{ReverseFace}>";
    }
}
