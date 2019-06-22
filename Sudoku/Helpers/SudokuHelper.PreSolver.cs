using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public static partial class SudokuHelper
    {
        private static bool EliminateNakedPairsInRegionBasedOnColMatch(ref SudokuWithLegalValues sudoku, int row1, int row2, int col, byte value1, byte value2, out int nrRemovedElements)
        {
            nrRemovedElements = 0;

            Region region = GetRegionCoordinates(sudoku.RegionSize, row1, col);

            bool sameRegion = IsInRegion(region, row2, col);

            if (!sameRegion)
            {
                return true;
            }

            // Row was already checked
            for (int i = row1 + 1; i < region.RowEnd; i++)
            {
                for (int j = region.ColStart; j < region.ColEnd; j++)
                {
                    if (j == col)
                    {
                        continue;
                    }

                    if (sudoku.Grid[i][j].Remove(value1))
                    {
                        nrRemovedElements++;
                    }

                    if (sudoku.Grid[i][j].Remove(value2))
                    {
                        nrRemovedElements++;
                    }
                }
            }

            return true;
        }

        private static bool EliminateNakedPairsInRegionBasedOnRowMatch(ref SudokuWithLegalValues sudoku, int row, int col1, int col2, byte value1, byte value2, out int nrRemovedElements)
        {
            nrRemovedElements = 0;

            Region region = GetRegionCoordinates(sudoku.RegionSize, row, col1);

            // No need to check if current row is in last row
            if (row >= region.RowEnd)
            {
                return true;
            }

            bool sameRegion = IsInRegion(region, row, col2);

            if (!sameRegion)
            {
                return true;
            }

            for (int i = row + 1; i < region.RowEnd; i++)
            {
                for (int j = region.ColStart; j < region.ColEnd; j++)
                {
                    if (j == col1 || j == col2)
                    {
                        continue;
                    }

                    if (sudoku.Grid[i][j].Remove(value1))
                    {
                        nrRemovedElements++;
                    }

                    if (sudoku.Grid[i][j].Remove(value2))
                    {
                        nrRemovedElements++;
                    }
                }
            }

            return true;
        }

        private static bool EliminateNakedPairsOnCol(ref SudokuWithLegalValues sudoku, int row1, int row2, int col, byte value1, byte value2, out int nrRemovedElements)
        {
            nrRemovedElements = 0;

            for (int i = 0; i < sudoku.Size; i++)
            {
                if (i != row1 && i != row2)
                {
                    if (sudoku.Grid[i][col].Remove(value1))
                    {
                        nrRemovedElements++;
                    }

                    if (sudoku.Grid[i][col].Remove(value2))
                    {
                        nrRemovedElements++;
                    }
                }
            }

            return true;
        }

        private static bool EliminateNakedPairsOnRow(ref SudokuWithLegalValues sudoku, int row, int col1, int col2, byte value1, byte value2, out int nrRemovedElements)
        {
            nrRemovedElements = 0;

            for (int i = 0; i < sudoku.Size; i++)
            {
                if (i != col1 && i != col2)
                {
                    if (sudoku.Grid[row][i].Remove(value1))
                    {
                        nrRemovedElements++;
                    }

                    if (sudoku.Grid[row][i].Remove(value2))
                    {
                        nrRemovedElements++;
                    }
                }
            }

            return true;
        }

        private static bool PreReduceUntilPossible_NakedPairs(ref SudokuWithLegalValues sudoku, out int reducedCounter)
        {
            reducedCounter = 0;

            // Only go 'till the row before last
            for (int row = 0; row < sudoku.Size; row++)
            {
                // Only go 'till the col before last
                for (int col = 0; col < sudoku.Size; col++)
                {
                    List<byte> cellValues = sudoku.Grid[row][col];

                    if (cellValues.Count == 2)
                    {
                        if (col < sudoku.Size - 1)
                        {
                            // Look for similar in row
                            for (int i = col + 1; i < sudoku.Size; i++)
                            {
                                // Iterating through row, horizontally
                                if (sudoku.Grid[row][i].Count == 2 && cellValues.Contains(sudoku.Grid[row][i][0]) && cellValues.Contains(sudoku.Grid[row][i][1]))
                                {
                                    Log($"[{row + 1}][{col + 1}]→[{row + 1}][{i + 1}] is naked pair: {string.Join(", ", cellValues.Select(b => b.ToString()))}");

                                    if (!EliminateNakedPairsOnRow(ref sudoku, row, col, i, cellValues[0], cellValues[1], out int intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on row {row} had problems");
                                        return false;
                                    }
                                    reducedCounter += intermedCounter;

                                    // Eliminate in sector, if needed
                                    if (!EliminateNakedPairsInRegionBasedOnRowMatch(ref sudoku, row, col, i, cellValues[0], cellValues[1], out intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on region/row {row} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;
                                }
                            }
                        }

                        if (row < sudoku.Size - 1)
                        {
                            for (int i = row + 1; i < sudoku.Size; i++)
                            {
                                if (sudoku.Grid[i][col].Count == 2 && cellValues.Contains(sudoku.Grid[i][col][0]) && cellValues.Contains(sudoku.Grid[i][col][1]))
                                {
                                    Log($"[{row + 1}][{col + 1}]→[{i + 1}][{col + 1}] is naked pair: {string.Join(", ", cellValues.Select(b => b.ToString()))}");

                                    if (!EliminateNakedPairsOnCol(ref sudoku, row, i, col, cellValues[0], cellValues[1], out int intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on col {col} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;

                                    // Eliminate in sector, if needed
                                    if (!EliminateNakedPairsInRegionBasedOnColMatch(ref sudoku, row, i, col, cellValues[0], cellValues[1], out intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on region/col {row} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static bool EliminateNakedPairsAfterPosition(ref SudokuWithLegalValues sudoku, int row, int col, out int reducedCounter)
        {
            reducedCounter = 0;

            // Only go 'till the row before last
            for (int rowIndex = row; rowIndex < sudoku.Size; rowIndex++)
            {
                // Only go 'till the col before last
                for (int colIndex = 0; colIndex < sudoku.Size; colIndex++)
                {
                    if (rowIndex == row && colIndex <= col)
                    {
                        continue;
                    }

                    List<byte> cellValues = sudoku.Grid[rowIndex][colIndex];

                    if (cellValues.Count == 2)
                    {
                        if (colIndex < sudoku.Size - 1)
                        {
                            // Look for similar in row
                            for (int i = colIndex + 1; i < sudoku.Size; i++)
                            {
                                // Iterating through row, horizontally
                                if (sudoku.Grid[rowIndex][i].Count == 2 && cellValues.Contains(sudoku.Grid[rowIndex][i][0]) && cellValues.Contains(sudoku.Grid[rowIndex][i][1]))
                                {
                                    Log($"[{rowIndex + 1}][{colIndex + 1}]→[{rowIndex + 1}][{i + 1}] is naked pair: {string.Join(", ", cellValues.Select(b => b.ToString()))}");

                                    if (!EliminateNakedPairsOnRow(ref sudoku, rowIndex, colIndex, i, cellValues[0], cellValues[1], out int intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on row {rowIndex} had problems");
                                        return false;
                                    }
                                    reducedCounter += intermedCounter;

                                    // Eliminate in sector, if needed
                                    if (!EliminateNakedPairsInRegionBasedOnRowMatch(ref sudoku, rowIndex, colIndex, i, cellValues[0], cellValues[1], out intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on region/row {rowIndex} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;
                                }
                            }
                        }

                        if (rowIndex < sudoku.Size - 1)
                        {
                            for (int i = rowIndex + 1; i < sudoku.Size; i++)
                            {
                                if (sudoku.Grid[i][colIndex].Count == 2 && cellValues.Contains(sudoku.Grid[i][colIndex][0]) && cellValues.Contains(sudoku.Grid[i][colIndex][1]))
                                {
                                    Log($"[{rowIndex + 1}][{colIndex + 1}]→[{i + 1}][{colIndex + 1}] is naked pair: {string.Join(", ", cellValues.Select(b => b.ToString()))}");

                                    if (!EliminateNakedPairsOnCol(ref sudoku, rowIndex, i, colIndex, cellValues[0], cellValues[1], out int intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on col {colIndex} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;

                                    // Eliminate in sector, if needed
                                    if (!EliminateNakedPairsInRegionBasedOnColMatch(ref sudoku, rowIndex, i, colIndex, cellValues[0], cellValues[1], out intermedCounter))
                                    {
                                        LogError($"Naked pair cleanup on region/col {rowIndex} had problems");
                                        return false;
                                    }

                                    reducedCounter += intermedCounter;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Should be run after any other reducing.
        /// </summary>
        /// <param name="sudokuToSolve"></param>
        /// <returns></returns>
        private static bool PreReduceUntilPossible_SingleNaked(ref SudokuWithLegalValues sudokuToSolve, out int reducedCount)
        {
            reducedCount = 0;
            int intermedReducedCount;

            do
            {
                bool result = ReduceValuesBasedOnNakedSingles(ref sudokuToSolve, out intermedReducedCount);

                if (!result)
                {
                    LogError($"    Naked singles run failed, because there were no values left");
                    return false;
                }

                reducedCount += intermedReducedCount;
            }
            while (intermedReducedCount > 0);

            return true;
        }

        private static bool ReduceValuesBasedOnNakedSingles(ref SudokuWithLegalValues sudoku, out int reducedCounter)
        {
            reducedCounter = 0;

            for (int row = 0; row < sudoku.Size; row++)
            {
                for (int col = 0; col < sudoku.Size; col++)
                {
                    if (sudoku.Grid[row][col].Count == 0)
                    {
                        Log($"        [ReduceValuesBasedOnNakedSingles] [{row + 1}][{col + 1}] does not contain an item");
                        return false;
                    }

                    if (sudoku.Grid[row][col].Count == 1)
                    {
                        byte value = sudoku.Grid[row][col][0];

                        bool result = EliminateValuesFromRow(ref sudoku, row, col, value, false, out int partialCounter);
                        reducedCounter += partialCounter;

                        if (!result)
                        {
                            Log($"        [ReduceValuesBasedOnNakedSingles] Eliminating values from row {row + 1} failed");
                            return false;
                        }

                        result = EliminateValuesFromCol(ref sudoku, row, col, value, false, out partialCounter);
                        reducedCounter += partialCounter;

                        if (!result)
                        {
                            Log($"        [ReduceValuesBasedOnNakedSingles] Eliminating values from col {col + 1} failed");
                            return false;
                        }

                        result = EliminateValuesFromRegion(ref sudoku, row, col, value, false, true, out partialCounter);
                        reducedCounter += partialCounter;

                        if (!result)
                        {
                            Log($"        [ReduceValuesBasedOnNakedSingles] Eliminating values from region [{row + 1}, {col + 1}] failed");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}