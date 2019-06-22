using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku
{
    public static partial class SudokuHelper
    {
        public static bool IsInRegion(Region region, int row, int col)
        {
            return
                row >= region.RowStart && row <= region.RowEnd && col >= region.ColStart && col <= region.ColEnd;
        }

        /// <summary>
        /// Main entry point of the helper class.
        /// Solves the sudoku.
        /// </summary>
        /// <param name="sudokuToSolve"></param>
        /// <param name="nrMaxSolutions"></param>
        /// <param name="failedAttempts"></param>
        /// <returns></returns>
        public static List<SudokuWithLegalValues> Solve(SudokuWithLegalValues sudokuToSolve, int nrMaxSolutions, SudokuWithLegalValues alreadySolvedForDebugging, out int failedAttempts)
        {
            failedAttempts = 0;
            Stopwatch sw = new Stopwatch();
            Stopwatch swOverall = new Stopwatch();

            Log("Original sudoku:", true);
            LogSudoku(sudokuToSolve, true);

            Log("Pre-reduction -----------------------------------------------------------------------------------------------------------------", true);

            int reducedCount = 0;
            swOverall.Start();
            sw.Start();
            if (!PreReduceUntilPossible_SingleNaked(ref sudokuToSolve, out int intermedReducedCount))
            {
                LogError($"Failed reducing naked singles");

                return null;
            }
            else
            {
                Log($"Eliminated {intermedReducedCount} from nakeds", true);

                reducedCount += intermedReducedCount;
            }

            bool success = PreReduceUntilPossible_NakedPairs(ref sudokuToSolve, out int reducedNakedPairsCount);

            if (!success)
            {
                LogError($"Failed reducing naked pairs");

                return new List<SudokuWithLegalValues>();
            }
            else
            {
                Log($"Eliminated {reducedNakedPairsCount} from naked pairs");
                reducedCount += reducedNakedPairsCount;
            }

            if (!PreReduceUntilPossible_SingleNaked(ref sudokuToSolve, out intermedReducedCount))
            {
                LogError($"Failed reducing naked singles");

                return null;
            }
            else
            {
                Log($"Eliminated {intermedReducedCount} from nakeds");

                reducedCount += intermedReducedCount;
            }

            Log($"Preprocessing took {sw.ElapsedMilliseconds} ms and eliminated {reducedCount} items", true);

            Log("", true);
            Log("Starting brute-force -----------------------------------------------------------------------------------------------------------------", true);

            List<SudokuWithLegalValues> solvedSudokus = new List<SudokuWithLegalValues>();

            if (alreadySolvedForDebugging != null)
            {
                Console.ReadKey();
                Console.Clear();
            }

            sw.Restart();
            SolveRecursive(sudokuToSolve, 0, 0, nrMaxSolutions, alreadySolvedForDebugging, ref solvedSudokus, ref failedAttempts);
            sw.Stop();

            Log($"Brute solved in {sw.ElapsedMilliseconds}ms, {failedAttempts} failed attempts", true);

            if (solvedSudokus.Count > 0)
            {
                Log("Solved sudoku", true);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Log("Failed to solve sudoku", true);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Log($"Overall duration: {swOverall.ElapsedMilliseconds}ms", true);
            Log("", true);

            return solvedSudokus;
        }

        private static bool EliminateValuesFromCol(ref SudokuWithLegalValues sudoku, int row, int col, byte value, bool forwardOnly, out int reducedCounter)
        {
            reducedCounter = 0;

            if (!forwardOnly || row < sudoku.Size - 1)
            {
                for (int i = forwardOnly ? row + 1 : 0; i < sudoku.Size; i++)
                {
                    // Skip the current value
                    if (i == row)
                    {
                        continue;
                    }

                    if (sudoku.Grid[i][col].Remove(value))
                    {
                        Log($"Removed value {value} from [{i + 1}, {col + 1}] after it was set for [{row + 1}, {col + 1}], during col cleanup");

                        if (sudoku.Grid[i][col].Count == 0)
                        {
                            Log($"No items remain after removing {value} from position [{i + 1}, {col + 1}] during col value elimination of [{row + 1}, {col + 1}]");
                            return false;
                        }

                        reducedCounter++;
                    }
                }
            }

            return true;
        }

        private static bool EliminateValuesFromRegion(ref SudokuWithLegalValues sudoku, int row, int col, byte value, bool forwardOnly, bool assumeRowsAndColdAlreadyEliminated, out int reducedCounter)
        {
            reducedCounter = 0;
            Region region = GetRegionCoordinates(sudoku.RegionSize, row, col);

            if (row < sudoku.Size - 1 || col < sudoku.Size - 1)
            {
                for (int i = region.RowStart; i <= region.RowEnd; i++)
                {
                    for (int j = region.ColStart; j <= region.ColEnd; j++)
                    {
                        if (row == i && col == j)
                        {
                            // Skip current cell
                            continue;
                        }

                        if (
                            forwardOnly && (
                                // Previous row
                                i < row ||
                                (i == row && j < col)))
                        {
                            // If necessary, skip elements before the current cell
                            continue;
                        }

                        if (
                            assumeRowsAndColdAlreadyEliminated && (
                                i == row ||
                                j == col))
                        {
                            // If necessary, skip row and cell, because they were already cleaned
                            continue;
                        }

                        if (sudoku.Grid[i][j].Remove(value))
                        {
                            if (sudoku.Grid[i][j].Count == 0)
                            {
                                Log($"No items remain after removing {value} from position [{i + 1}, {j + 1}] during region value elimination of [{row + 1}, {col + 1}]");
                                return false;
                            }

                            reducedCounter++;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sudoku"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <param name="forwardOnly">Only touches forward positions</param>
        /// <param name="reducedCounter"></param>
        /// <returns></returns>
        private static bool EliminateValuesFromRow(ref SudokuWithLegalValues sudoku, int row, int col, byte value, bool forwardOnly, out int reducedCounter)
        {
            reducedCounter = 0;

            if (!forwardOnly || col < sudoku.Size - 1)
            {
                for (int i = forwardOnly ? col + 1 : 0; i < sudoku.Size; i++)
                {
                    // Skip the current value
                    if (i == col)
                    {
                        continue;
                    }

                    if (sudoku.Grid[row][i].Remove(value))
                    {
                        Log($"Removed value {value} from [{row + 1}, {i + 1}] after it was set for [{row + 1}, {col + 1}], during row cleanup");

                        if (sudoku.Grid[row][i].Count == 0)
                        {
                            Log($"No items remain after removing {value} from position [{row + 1}, {i + 1}] during row value elimination of [{row + 1}, {col + 1}]");
                            return false;
                        }

                        reducedCounter++;
                    }
                }
            }

            return true;
        }

        private static byte GetCurrentLegalValue(SudokuWithLegalValues sudoku, int regionRow, int regionCol)
        {
            return (byte)(
                (regionRow * sudoku.RegionSize) +
                regionCol + 1);
        }

        private static void GetNextCellCoordinates(int sudokuLength, int row, int col, out int newRow, out int newCol)
        {
            if (
                // Bottom right cell will exit bounds
                col >= sudokuLength - 1 &&
                row >= sudokuLength - 1)
            {
                newRow = sudokuLength;
                newCol = sudokuLength;
            }
            else
            {
                // Last col will increase row and set col to 0
                if (col >= sudokuLength - 1)
                {
                    newCol = 0;
                    newRow = row + 1;
                }
                else
                {
                    // Row remains, col advances
                    newCol = col + 1;
                    newRow = row;
                }
            }
        }

        private static Region GetRegionCoordinates(int sudokuRegionSize, int row, int col)
        {
            if (sudokuRegionSize <= 0)
            {
                return new Region(0, 0, 0, 0);
            }

            int regionRowStart = (int)Math.Floor((decimal)(row / sudokuRegionSize)) * sudokuRegionSize;
            int regionRowEnd = regionRowStart + sudokuRegionSize - 1;

            int regionColStart = (int)Math.Floor((decimal)(col / sudokuRegionSize)) * sudokuRegionSize;
            int regionColEnd = regionColStart + sudokuRegionSize - 1;

            return new Region(
                regionRowStart,
                regionRowEnd,

                regionColStart,
                regionColEnd);
        }

        private static bool IsLastPosition(int sudokuSize, int row, int col)
        {
            return (row >= sudokuSize - 1) && (col >= sudokuSize - 1);
        }

        private static bool IsPartOfSolution(SudokuWithLegalValues sudoku, int row, int col, SudokuWithLegalValues solvedSudoku)
        {
            for (int i = 0; i < sudoku.Size; i++)
            {
                for (int j = 0; j < sudoku.Size; j++)
                {
                    if (sudoku.Grid[i][j].Count != 1 || solvedSudoku.Grid[i][j].Count != 1 || sudoku.Grid[i][j][0] != solvedSudoku.Grid[i][j][0])
                    {
                        return false;
                    }

                    // Only check until current position
                    if (i == row && j == col)
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        private static void SolveRecursive(SudokuWithLegalValues sudoku, int row, int col, int nrMaxSolutions, SudokuWithLegalValues alreadySolvedForDebugging, ref List<SudokuWithLegalValues> solvedSudokus, ref int failedAttempts)
        {
            if (solvedSudokus.Count >= nrMaxSolutions)
            {
                return;
            }

            List<byte> originalValues = sudoku.Grid[row][col];

            if (originalValues.Count == 0)
            {
                LogError($"No values in [{row + 1}][{col + 1}]");
                return;
            }

            foreach (byte valueToTest in originalValues)
            {
                if (originalValues.Count == 1 || ValueIsLegalForPosition(sudoku, row, col, valueToTest))
                {
                    SudokuWithLegalValues temporaryClone;

                    // Clone before any modification and use the clone from now on and set the newly found legal value
                    if (originalValues.Count == 1)
                    {
                        temporaryClone = sudoku;
                    }
                    else
                    {
                        temporaryClone = sudoku.CloneWithNewValue(row, col, valueToTest);
                    }

                    // One solution found
                    if (IsLastPosition(temporaryClone.Size, row, col))
                    {
                        solvedSudokus.Add(temporaryClone);
                        return;
                    }

                    if (originalValues.Count > 1)
                    {
                        if (EliminateValuesFromRow(ref temporaryClone, row, col, valueToTest, true, out int reducedCounter))
                        {
                            if (reducedCounter > 0)
                            {
                                Log($"Eliminated {reducedCounter} items from row after setting value {valueToTest} to [{row + 1}, {col + 1}]", false);
                            }
                        }
                        else
                        {
                            Log($"[SolveRemainingRecursive] Eliminating values from row {row + 1} failed");
                            continue;
                        }

                        if (EliminateValuesFromCol(ref temporaryClone, row, col, valueToTest, true, out reducedCounter))
                        {
                            if (reducedCounter > 0)
                            {
                                Log($"Eliminated {reducedCounter} items from col after setting value {valueToTest} to [{row + 1}, {col + 1}]", false);
                            }
                        }
                        else
                        {
                            Log($"[SolveRemainingRecursive] Eliminating values from col {col + 1} failed");
                            continue;
                        }

                        if (EliminateValuesFromRegion(ref temporaryClone, row, col, valueToTest, true, true, out reducedCounter))
                        {
                            if (reducedCounter > 0)
                            {
                                Log($"Eliminated {reducedCounter} items from region after setting value {valueToTest} to [{row + 1}, {col + 1}]", false);
                            }
                        }
                        else
                        {
                            Log($"[SolveRemainingRecursive] Eliminating values from region of [{row + 1}, {col + 1}] failed");
                            continue;
                        }

                        if (false)
                        {
                            if (EliminateNakedPairsAfterPosition(ref temporaryClone, row, col, out reducedCounter))
                            {
                                if (reducedCounter > 0)
                                {
                                    Log($"Eliminated {reducedCounter} item through naked pairs after setting value {valueToTest} to [{row + 1}, {col + 1}]", true);
                                }
                            }
                            else
                            {
                                Log($"Eliminating naked pairs from region of [{row + 1}, {col + 1}] failed");
                                continue;
                            }
                        }

                        if (alreadySolvedForDebugging != null)
                        {
                            if (IsPartOfSolution(sudoku, row, col, alreadySolvedForDebugging))
                            {
                                DisplaySudokuToConsole(temporaryClone, row, col, false, true);
                                Console.ReadKey();
                            }
                        }
                    }

                    // Advance position
                    GetNextCellCoordinates(
                        temporaryClone.Size,
                        row,
                        col,
                        out int newRow,
                        out int newCol);

                    SolveRecursive(
                        temporaryClone,
                        newRow,
                        newCol,
                        nrMaxSolutions,
                        alreadySolvedForDebugging,
                        ref solvedSudokus,
                        ref failedAttempts);
                }
            }

            // This "thread" of solutions has failed
            failedAttempts++;

            if (failedAttempts % 100000 == 0)
            {
                Log($"[{DateTime.Now.ToString("hh:mm:ss.fff")}] {failedAttempts} attempts failed so far", true);
            }

            Log($"Failed [{row + 1}][{col + 1}].");
        }

        private static bool ValueIsLegalForPosition(SudokuWithLegalValues sudokuToSolve, int row, int col, byte valueToTest)
        {
            return
                ValueIsLegalInRowAndCol(sudokuToSolve, row, col, valueToTest) &&
                ValueIsLegalInRegion(sudokuToSolve, row, col, valueToTest);
        }

        private static bool ValueIsLegalInRegion(SudokuWithLegalValues sudokuToSolve, int row, int col, byte valueToTest)
        {
            Region region = GetRegionCoordinates(sudokuToSolve.RegionSize, row, col);

            for (int i = region.RowStart; i < region.RowEnd; i++)
            {
                for (int j = region.ColStart; j < region.ColEnd; j++)
                {
                    if (row == i && col == j)
                    {
                        // Skip current cell
                        continue;
                    }

                    if (sudokuToSolve.Grid[i][j].Count == 1 && sudokuToSolve.Grid[i][j].Contains(valueToTest))
                    {
                        Log($"    Value {valueToTest} is not legal in region [{region.RowStart + 1}, {region.ColStart + 1}][{region.RowEnd + 1}, {region.ColEnd + 1}]");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ValueIsLegalInRowAndCol(SudokuWithLegalValues sudokuToSolve, int row, int col, byte valueToTest)
        {
            for (int index = 0; index < sudokuToSolve.Size; index++)
            {
                // Ignore current cell
                if (index != col)
                {
                    if (sudokuToSolve.Grid[row][index].Count == 1 && sudokuToSolve.Grid[row][index].Contains(valueToTest))
                    {
                        Log($"    Value {valueToTest} is not legal in row of [{row + 1}, {col + 1}]");
                        return false;
                    }
                }

                if (index != row && sudokuToSolve.Grid[index][col].Count == 1 && sudokuToSolve.Grid[index][col].Contains(valueToTest))
                {
                    Log($"    Value {valueToTest} is not legal in col of [{row + 1}, {col + 1}]");
                    return false;
                }
            }

            return true;
        }
    }
}