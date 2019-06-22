using Sudoku.Entities;
using Sudoku.Enums;
using System;
using System.Text;

namespace Sudoku.Helpers
{
    public static partial class SudokuHelper
    {
        private const bool LogDetailed = false;

        private static string BottomBorder { get; set; }
        private static string IntermedBorder { get; set; }
        private static string MidBorder { get; set; }
        private static string TopBorder { get; set; }

        public static void DisplaySudokuToConsole(SudokuWithLegalValues sudoku, int currentRow, int currentCol, bool onlyDisplayCurrentRegion, bool displayInSamePlace)
        {
            if (TopBorder == null)
            {
                TopBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Top);
            }

            if (MidBorder == null)
            {
                MidBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Mid);
            }

            if (IntermedBorder == null)
            {
                IntermedBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Intermed);
            }

            if (BottomBorder == null)
            {
                BottomBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Bottom);
            }

            if (displayInSamePlace)
            {
                Console.SetCursorPosition(0, 0);
            }

            Console.WriteLine(TopBorder);

            for (int rowIndex = 0; rowIndex < sudoku.Size; rowIndex++)
            {
                for (int regionRow = 0; regionRow < sudoku.RegionSize; regionRow++)
                {
                    Console.Write("║");

                    for (int colIndex = 0; colIndex < sudoku.Size; colIndex++)
                    {
                        for (int regionCol = 0; regionCol < sudoku.RegionSize; regionCol++)
                        {
                            byte currentLegalValue = GetCurrentLegalValue(sudoku, regionRow, regionCol);
                            DisplayLegalValueToConsole(sudoku, rowIndex, colIndex, currentLegalValue, rowIndex == currentRow && colIndex == currentCol);
                        }

                        if ((colIndex + 1) % sudoku.RegionSize == 0)
                        {
                            Console.Write("║");
                        }
                        else
                        {
                            Console.Write("│");
                        }
                    }

                    Console.WriteLine();
                }

                if (rowIndex + 1 == sudoku.Size)
                {
                    Console.WriteLine(BottomBorder);
                }
                else
                {
                    if ((rowIndex + 1) % sudoku.RegionSize == 0)
                    {
                        Console.WriteLine(MidBorder);
                    }
                    else
                    {
                        Console.WriteLine(IntermedBorder);
                    }
                }

                if (onlyDisplayCurrentRegion && rowIndex > currentRow + 1)
                {
                    return;
                }
            }
        }

        public static string GenerateDisplayStringCompact(SudokuWithLegalValues sudoku)
        {
            StringBuilder sudokuText = new StringBuilder();
            int nrDigits = sudoku.Size.ToString().Length + 1;

            for (int rowIndex = 0; rowIndex < sudoku.Size; rowIndex++)
            {
                for (int colIndex = 0; colIndex < sudoku.Size; colIndex++)
                {
                    if (sudoku.Grid[rowIndex][colIndex].Count > 0)
                    {
                        sudokuText.Append(sudoku.Grid[rowIndex][colIndex][0].ToString().PadLeft(nrDigits));
                    }
                    else
                    {
                        sudokuText.Append(" ".PadLeft(nrDigits));
                    }

                    if ((colIndex + 1) % sudoku.RegionSize == 0)
                    {
                        sudokuText.Append(" ");
                    }
                }

                sudokuText.AppendLine();

                if ((rowIndex + 1) % sudoku.RegionSize == 0)
                {
                    sudokuText.AppendLine();
                }
            }

            return sudokuText.ToString();
        }

        public static void LogSudoku(SudokuWithLegalValues sudoku, bool forceLog = false)
        {
            string sudokuText = GenerateDisplayString(sudoku);
            Log(sudokuText, forceLog);
            Log(string.Empty, forceLog);
            Log(string.Empty, forceLog);
        }

        public static void LogSudokuCompact(SudokuWithLegalValues sudoku, bool forceLog = false)
        {
            string sudokuText = GenerateDisplayString(sudoku);
            Log(sudokuText, forceLog);
            Log(string.Empty, forceLog);
            Log(string.Empty, forceLog);
        }

        private static void AppendLegalValue(SudokuWithLegalValues sudoku, StringBuilder sudokuText, int rowIndex, int colIndex, byte currentLegalValue)
        {
            int nrDigits = sudoku.Size.ToString().Length + 1;

            if (sudoku.Grid[rowIndex][colIndex].Contains(currentLegalValue))
            {
                sudokuText.Append(currentLegalValue.ToString().PadLeft(nrDigits));
            }
            else
            {
                sudokuText.Append(string.Empty.PadLeft(nrDigits, ' '));
            }
        }

        private static void DisplayLegalValueToConsole(SudokuWithLegalValues sudoku, int rowIndex, int colIndex, byte currentLegalValue, bool highlight)
        {
            int nrDigits = sudoku.Size.ToString().Length + 1;

            if (sudoku.Grid[rowIndex][colIndex].Contains(currentLegalValue))
            {
                if (highlight)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.Write(currentLegalValue.ToString().PadLeft(nrDigits));

                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.Write(string.Empty.PadLeft(nrDigits, ' '));
            }
        }

        private static string GenerateDisplayString(SudokuWithLegalValues sudoku)
        {
            if (TopBorder == null)
            {
                TopBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Top);
            }

            if (MidBorder == null)
            {
                MidBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Mid);
            }

            if (IntermedBorder == null)
            {
                IntermedBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Intermed);
            }

            if (BottomBorder == null)
            {
                BottomBorder = GenerateHorizontalBorder(sudoku.Size, sudoku.RegionSize, HorizontalBorderType.Bottom);
            }

            StringBuilder sudokuText = new StringBuilder();

            sudokuText.AppendLine(TopBorder);

            for (int rowIndex = 0; rowIndex < sudoku.Size; rowIndex++)
            {
                for (int regionRow = 0; regionRow < sudoku.RegionSize; regionRow++)
                {
                    sudokuText.Append("║");

                    for (int colIndex = 0; colIndex < sudoku.Size; colIndex++)
                    {
                        for (int regionCol = 0; regionCol < sudoku.RegionSize; regionCol++)
                        {
                            byte currentLegalValue = GetCurrentLegalValue(sudoku, regionRow, regionCol);
                            AppendLegalValue(sudoku, sudokuText, rowIndex, colIndex, currentLegalValue);
                        }

                        if ((colIndex + 1) % sudoku.RegionSize == 0)
                        {
                            sudokuText.Append("║");
                        }
                        else
                        {
                            sudokuText.Append("│");
                        }
                    }

                    sudokuText.AppendLine();
                }

                if (rowIndex + 1 == sudoku.Size)
                {
                    sudokuText.AppendLine(BottomBorder);
                }
                else
                {
                    if ((rowIndex + 1) % sudoku.RegionSize == 0)
                    {
                        sudokuText.AppendLine(MidBorder);
                    }
                    else
                    {
                        sudokuText.AppendLine(IntermedBorder);
                    }
                }
            }

            return sudokuText.ToString();
        }

        private static string GenerateHorizontalBorder(int sudokuSize, int sudokuRegionSize, HorizontalBorderType borderType)
        {
            int nrDigits = sudokuSize.ToString().Length + 1;
            string doubleHorizontal = new string(borderType == HorizontalBorderType.Intermed ? '─' : '═', sudokuRegionSize * nrDigits);

            StringBuilder horizontalBorder = new StringBuilder();

            horizontalBorder.Append(GetLeft(borderType));

            for (int i = 0; i < sudokuSize; i++)
            {
                horizontalBorder.Append(doubleHorizontal);

                if (i < sudokuSize - 1)
                {
                    if ((i + 1) % sudokuRegionSize == 0)
                    {
                        horizontalBorder.Append(GetMidMain(borderType));
                    }
                    else
                    {
                        horizontalBorder.Append(GetMid(borderType));
                    }
                }
            }

            horizontalBorder.Append(GetRight(borderType));
            return horizontalBorder.ToString();
        }

        private static string GetLeft(HorizontalBorderType borderType)
        {
            switch (borderType)
            {
                case HorizontalBorderType.Top:
                    return "╔";

                case HorizontalBorderType.Mid:
                    return "╠";

                case HorizontalBorderType.Bottom:
                    return "╚";

                default:
                    return "╟";
            }
        }

        private static string GetMid(HorizontalBorderType borderType)
        {
            switch (borderType)
            {
                case HorizontalBorderType.Top:
                    return "╤";

                case HorizontalBorderType.Mid:
                    return "╪";

                case HorizontalBorderType.Bottom:
                    return "╧";

                default:
                    return "┼";
            }
        }

        private static string GetMidMain(HorizontalBorderType borderType)
        {
            switch (borderType)
            {
                case HorizontalBorderType.Top:
                    return "╦";

                case HorizontalBorderType.Mid:
                    return "╬";

                case HorizontalBorderType.Bottom:
                    return "╩";

                default:
                    return "╫";
            }
        }

        private static string GetRight(HorizontalBorderType borderType)
        {
            switch (borderType)
            {
                case HorizontalBorderType.Top:
                    return "╗";

                case HorizontalBorderType.Mid:
                    return "╣";

                case HorizontalBorderType.Bottom:
                    return "╝";

                default:
                    return "╢";
            }
        }

        private static void Log(string messageToLog = "", bool forceLog = false)
        {
            if (forceLog || LogDetailed)
            {
                Console.WriteLine(messageToLog);
            }
        }

        private static void LogError(string messageToLog)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {messageToLog}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}