using System;
using System.Collections.Generic;
using System.Linq;

namespace Symirror4.Core.Symmetry;

partial class SymmetryGroup
{
    public static SymmetryGroup Create(SymmetrySymbol symbol)
    {
        // それぞれのElementTypeを持つ頂点にいくつの面が集まるか
        var cellCounts = new int[]
        {
                Sphere.GetSymmetryOrder(symbol[0], symbol[1], symbol[4]),
                Sphere.GetSymmetryOrder(symbol[0], symbol[3], symbol[2]),
                Sphere.GetSymmetryOrder(symbol[5], symbol[1], symbol[2]),
                Sphere.GetSymmetryOrder(symbol[5], symbol[3], symbol[4]),
        };

        // 理論上の最大オーダー数
        var maxCellCount = 384;
        if (symbol.Any(x => x.Numerator == 4))
            maxCellCount = 1152;
        if (symbol.Any(x => x.Numerator == 5))
            maxCellCount = 14400;

        // 理論上の最大頂点数
        var maxVertexCount =
            maxCellCount / cellCounts[0] +
            maxCellCount / cellCounts[1] +
            maxCellCount / cellCounts[2] +
            maxCellCount / cellCounts[3];

        // コピー元となる球面三角形
        var firstFace = SymmetryTetrahedron.Create(symbol, cellCounts);

        // 次に周囲を探索する cells のインデックス
        var index = 0;

        // まだ「空き」のある頂点の数
        var leftVertexCount = 0;

        // 最終的な出力となる面
        var cells = new List<SymmetryTetrahedron>(maxCellCount) { firstFace };

        // 生成された各頂点
        var vertexList = new[]
        {
                new List<SymmetryVertex>(maxCellCount / cellCounts[0]),
                new List<SymmetryVertex>(maxCellCount / cellCounts[1]),
                new List<SymmetryVertex>(maxCellCount / cellCounts[2]),
                new List<SymmetryVertex>(maxCellCount / cellCounts[3]),
            };

        foreach (var v in firstFace)
        {
            vertexList[v.ElementType].Add(v);
            ++leftVertexCount;
        }

        var newVertices = new SymmetryVertex[4];

        while (0 < leftVertexCount)
        {
            // 頂点1つを裏返して別の玲胞四面体を作る
            for (var reverseIndex = 0; reverseIndex < 4; reverseIndex++)
            {
                var targetCell = cells[index];
                if (targetCell.GetNext(reverseIndex) is not null)
                    continue;

                // 一旦、元となる玲胞四面体のコピーを作成
                for (var i = 0; i < 4; i++)
                    newVertices[i] = targetCell[i];

                // 対象の頂点を、それを含まない面で裏返す
                var vector = targetCell[reverseIndex].Point.Reverse(
                    targetCell[(reverseIndex + 1) % 4].Point,
                    targetCell[(reverseIndex + 2) % 4].Point,
                    targetCell[(reverseIndex + 3) % 4].Point);

                // 誤差許容範囲
                const float er = 1f / 1024f;
                // 同一とみなせる頂点を探す
                if (vertexList[reverseIndex].Find(v => GlomericPoint.ApproximatelyEqual(v.Point, vector, er)) is { } vertex)
                {
                    // 見付かった頂点にこれ以上辺を追加できない場合
                    if (vertex.IsAroundCellsFull)
                        continue;
                    else
                        newVertices[reverseIndex] = vertex;
                }
                // 同一とみなせる頂点がない場合：新しく生成する
                else
                {
                    vertex = new SymmetryVertex(vector, reverseIndex,
                        vertexList[0].Count + vertexList[1].Count + vertexList[2].Count + vertexList[3].Count,
                        cellCounts[reverseIndex]);

                    if (vertex.Index >= maxVertexCount)
                        throw new ApplicationException("指定された記号から、超球胞を充填できませんでした。");

                    newVertices[reverseIndex] = vertex;
                    vertexList[vertex.ElementType].Add(vertex);
                }

                var newCell = new SymmetryTetrahedron(targetCell.ElementType ^ 1, cells.Count,
                    newVertices[0], newVertices[1], newVertices[2], newVertices[3]);

                targetCell.SetNext(reverseIndex, newCell);
                newCell.SetNext(reverseIndex, targetCell);

                // 「裏返し」に使ったもの以外の接続している胞を登録
                for (var vi = 0; vi < 4; vi++)
                {
                    if (newCell.GetNext(vi) is not null) continue;

                    foreach (var cell1 in newVertices[(vi + 1) % 4].AroundCells)
                    {
                        if (cell1 is null) break;
                        foreach (var cell2 in newVertices[(vi + 2) % 4].AroundCells)
                        {
                            if (cell2 is null) break;
                            if (cell1 == cell2)
                            {
                                foreach (var cell3 in newVertices[(vi + 3) % 4].AroundCells)
                                {
                                    if (cell3 is null) break;
                                    if (cell2 == cell3)
                                    {
                                        cell3.SetNext(reverseIndex, newCell);
                                        newCell.SetNext(reverseIndex, cell3);
                                        goto LOOPEND;
                                    }
                                }
                            }
                        }
                    }

                LOOPEND:;
                }

                // 新しい胞の各頂点に集まる胞を登録する
                foreach (var v in newVertices)
                {
                    v.AddAroundCell(newCell);

                    if (v.IsAroundCellsSingle)
                        ++leftVertexCount;
                    else if (v.IsAroundCellsFull)
                        --leftVertexCount;
                }

                cells.Add(newCell);

                // 探索打ち切り条件：理論上の最大オーダーよりも多い数の四面体を生成した
                if (cells.Count > maxCellCount)
                    throw new ApplicationException("指定された記号から、超球胞を充填できませんでした。");
            }

            index++;

            // 探索打ち切り条件：胞のコピーがこれ以上ないのに、満たされていない頂点がある
            if (index >= cells.Count)
                throw new ApplicationException("指定された記号から、超球胞を充填できませんでした。");
        }

        var cellArray = cells.ToArray();
        var vertexArray = vertexList.SelectMany(x => x).OrderBy(x => x.Index).ToArray();

        return new SymmetryGroup(symbol, cellArray, vertexArray);
    }
}
