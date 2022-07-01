using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror4.Core.Symmetry;

public class SymmetryPage
{
    public string Title { get; }

    public SymmetrySymbol[] Symbols { get; }

    public SymmetryPage(string title, SymmetrySymbol[] symbols)
    {
        Title = title;
        Symbols = symbols;
    }

    public static SymmetryPage[] AllSymbols =
    {
            new SymmetryPage("I2 × A1 × A1", new[]
            {
                new SymmetrySymbol(2, 2,     2 ),
                new SymmetrySymbol(2, 2,     3 ),
                new SymmetrySymbol(2, 2, (3, 2)),
                new SymmetrySymbol(2, 2,     4 ),
                new SymmetrySymbol(2, 2, (4, 3)),
                new SymmetrySymbol(2, 2,     5 ),
                new SymmetrySymbol(2, 2, (5, 2)),
                new SymmetrySymbol(2, 2, (5, 3)),
                new SymmetrySymbol(2, 2, (5, 4)),
            }),

            new SymmetryPage("I2 × I2", new[]
            {
                new SymmetrySymbol( 3    , 2,  3    ),
                new SymmetrySymbol( 3    , 2,  4    ),
                new SymmetrySymbol( 3    , 2, (4, 3)),
                new SymmetrySymbol( 3    , 2,  5    ),
                new SymmetrySymbol( 3    , 2, (5, 2)),
                new SymmetrySymbol( 3    , 2, (5, 3)),
                new SymmetrySymbol( 3    , 2, (5, 4)),
                new SymmetrySymbol( 4    , 2,  4    ),
                new SymmetrySymbol( 4    , 2, (4, 3)),
                new SymmetrySymbol( 4    , 2,  5    ),
                new SymmetrySymbol( 4    , 2, (5, 2)),
                new SymmetrySymbol( 4    , 2, (5, 3)),
                new SymmetrySymbol( 4    , 2, (5, 4)),
                new SymmetrySymbol((4, 3), 2, (4, 3)),
                new SymmetrySymbol((4, 3), 2,  5    ),
                new SymmetrySymbol((4, 3), 2, (5, 2)),
                new SymmetrySymbol((4, 3), 2, (5, 3)),
                new SymmetrySymbol((4, 3), 2, (5, 4)),
                new SymmetrySymbol( 5    , 2,  5    ),
                new SymmetrySymbol( 5    , 2, (5, 2)),
                new SymmetrySymbol( 5    , 2, (5, 3)),
                new SymmetrySymbol( 5    , 2, (5, 4)),
                new SymmetrySymbol((5, 2), 2, (5, 2)),
                new SymmetrySymbol((5, 2), 2, (5, 3)),
                new SymmetrySymbol((5, 2), 2, (5, 4)),
                new SymmetrySymbol((5, 3), 2, (5, 3)),
                new SymmetrySymbol((5, 3), 2, (5, 4)),
                new SymmetrySymbol((5, 4), 2, (5, 4)),
            }),

            new SymmetryPage("A3 × A1", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  3    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (3, 2), (3, 2)),
            }),

            new SymmetryPage("BC3 × A1", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2),  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  4    , (4, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2),  4    ,  4    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (4, 3), (4, 3)),
            }),

            new SymmetryPage("H3 × A1", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  5    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (5, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (5, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (5, 4),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2),  5    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 4),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  5    , (5, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  5    , (5, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (5, 2), (5, 4),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (5, 3), (5, 4),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  3    , (5, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  3    , (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (3, 2),  5    ),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (3, 2), (5, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (3, 2), (5, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (3, 2), (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  5    , (5, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    ,  5    , (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (5, 2), (5, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  3    , (5, 2), (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2),  5    ,  5    ),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2),  5    , (5, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 2), (5, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 3), (5, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 3), (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (3, 2), (5, 4), (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    ,  5    ,  5    , (5, 4)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (5, 2), (5, 2), (5, 2)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (5, 2), (5, 3), (5, 3)),
                new SymmetrySymbol( 2    ,  2    ,  2    , (5, 4), (5, 4), (5, 4)),
            }),

            new SymmetryPage("A4", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  3    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2),  3    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    , (3, 2),  3    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2), (3, 2), (3, 2)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    , (3, 2),  3    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2), (3, 2), (3, 2)),
                new SymmetrySymbol((3, 2), (3, 2),  3    ,  3    , (3, 2),  3    ),
                new SymmetrySymbol( 3    , (3, 2),  3    , (3, 2),  3    ,  3    ),
                new SymmetrySymbol((3, 2), (3, 2), (3, 2), (3, 2), (3, 2), (3, 2)),
            }),

            new SymmetryPage("D4", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  2    ,  3    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  2    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2),  2    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2),  2    , (3, 2)),
                new SymmetrySymbol( 2    , (3, 2),  3    ,  3    ,  3    ,  2    ),
                new SymmetrySymbol( 2    , (3, 2),  3    , (3, 2), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  3    ,  3    ,  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    , (3, 2),  3    ,  3    , (3, 2),  3    ),
                new SymmetrySymbol( 2    , (3, 2),  3    , (3, 2),  3    ,  3    ),
                new SymmetrySymbol( 2    ,  3    ,  3    , (3, 2), (3, 2), (3, 2)),
                new SymmetrySymbol( 2    , (3, 2), (3, 2), (3, 2), (3, 2), (3, 2)),
            }),

            new SymmetryPage("BC4", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    , (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2),  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2), (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    ,  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    , (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2),  4    ,  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2), (4, 3),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  4    ,  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    ,  4    ,  3    , (3, 2),  3    ),
                new SymmetrySymbol( 2    ,  2    ,  4    , (3, 2), (3, 2), (3, 2)),
                new SymmetrySymbol( 2    ,  2    , (4, 3),  3    ,  3    , (3, 2)),
                new SymmetrySymbol( 2    ,  2    , (4, 3),  3    , (3, 2),  3    ),
                new SymmetrySymbol( 2    ,  2    , (4, 3), (3, 2), (3, 2), (3, 2)),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    ,  4    , (4, 3)),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  3    , (4, 3),  4    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2),  4    ,  4    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (3, 2), (4, 3), (4, 3)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    ,  4    , (4, 3)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  3    , (4, 3),  4    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2),  4    ,  4    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (3, 2), (4, 3), (4, 3)),
                new SymmetrySymbol( 3    , (4, 3),  3    , (3, 2),  4    ,  4    ),
                new SymmetrySymbol( 3    , (4, 3), (3, 2),  3    ,  4    , (4, 3)),
                new SymmetrySymbol((3, 2),  4    , (3, 2), (3, 2),  4    ,  4    ),
                new SymmetrySymbol((3, 2), (4, 3), (3, 2), (3, 2), (4, 3), (4, 3)),
            }),

            new SymmetryPage("F4", new[]
            {
                new SymmetrySymbol( 2    ,  2    ,  3    ,  4    ,  3    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  4    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (4, 3),  3    ,  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (4, 3), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  4    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (4, 3), (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  4    ,  3    , (4, 3)),
                new SymmetrySymbol( 2    ,  2    ,  3    ,  4    , (3, 2),  4    ),
                new SymmetrySymbol( 2    ,  2    ,  3    , (4, 3), (3, 2), (4, 3)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  4    ,  3    , (4, 3)),
                new SymmetrySymbol( 2    ,  2    , (3, 2),  4    , (3, 2),  4    ),
                new SymmetrySymbol( 2    ,  2    , (3, 2), (4, 3), (3, 2), (4, 3)),
                new SymmetrySymbol( 2    , (4, 3),  3    ,  4    ,  3    ,  2    ),
                new SymmetrySymbol( 2    ,  4    ,  3    ,  4    , (3, 2),  2    ),
                new SymmetrySymbol( 2    , (4, 3),  3    , (4, 3), (3, 2),  2    ),
                new SymmetrySymbol( 2    , (4, 3), (3, 2),  4    , (3, 2),  2    ),
                new SymmetrySymbol( 2    ,  4    ,  3    ,  4    ,  3    , (4, 3)),
                new SymmetrySymbol( 2    , (4, 3),  3    ,  4    , (3, 2),  4    ),
                new SymmetrySymbol( 2    , (4, 3),  3    , (4, 3),  3    ,  4    ),
                new SymmetrySymbol( 2    , (4, 3), (3, 2),  4    ,  3    , (4, 3)),
                new SymmetrySymbol( 2    ,  4    , (3, 2),  4    , (3, 2),  4    ),
                new SymmetrySymbol( 2    , (4, 3), (3, 2), (4, 3), (3, 2), (4, 3)),
                new SymmetrySymbol( 4    , (4, 3),  3    , (4, 3),  3    ,  4    ),
                new SymmetrySymbol( 4    , (4, 3), (3, 2),  4    ,  3    , (4, 3)),
                new SymmetrySymbol( 4    ,  4    , (3, 2),  4    , (3, 2),  4    ),
                new SymmetrySymbol((4, 3), (4, 3), (3, 2), (4, 3), (3, 2), (4, 3)),
            }),
        };
}
