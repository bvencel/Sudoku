using System;
using System.Collections.Generic;

using SudokuGrid = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<byte>>>;

namespace Sudoku
{
    public class SudokuWithLegalValues
    {
        public SudokuWithLegalValues(SudokuGrid sudokuGrid)
        {
            Grid = sudokuGrid;
            Size = Grid.Count;
            RegionSize = (int)Math.Sqrt(Size);
        }

        public SudokuGrid Grid { get; set; }

        public int RegionSize { get; }

        public int Size { get; }

        public SudokuWithLegalValues Clone()
        {
            SudokuGrid clonedGrid = new SudokuGrid { };

            for (int i = 0; i < Size; i++)
            {
                List<List<byte>> newRow = new List<List<byte>>();

                for (int j = 0; j < Size; j++)
                {
                    List<byte> clonedLegalValues = new List<byte>();

                    foreach (byte item in Grid[i][j])
                    {
                        clonedLegalValues.Add(item);
                    }

                    newRow.Add(clonedLegalValues);
                }

                clonedGrid.Add(newRow);
            }

            SudokuWithLegalValues newInstance = new SudokuWithLegalValues(clonedGrid);
            return newInstance;
        }

        public SudokuWithLegalValues CloneWithNewValue(int row, int col, byte value)
        {
            SudokuGrid clonedGrid = new SudokuGrid { };

            for (int i = 0; i < Size; i++)
            {
                List<List<byte>> newRow = new List<List<byte>>();

                for (int j = 0; j < Size; j++)
                {
                    if (row == i && col == j)
                    {
                        newRow.Add(new List<byte> { value });
                        continue;
                    }

                    List<byte> clonedLegalValues = new List<byte>();

                    foreach (byte item in Grid[i][j])
                    {
                        clonedLegalValues.Add(item);
                    }

                    newRow.Add(clonedLegalValues);
                }

                clonedGrid.Add(newRow);
            }

            SudokuWithLegalValues newInstance = new SudokuWithLegalValues(clonedGrid);
            return newInstance;
        }
    }
}