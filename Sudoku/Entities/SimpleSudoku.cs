using System;
using System.Collections.Generic;
using SudokuGrid = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<byte>>>;

namespace Sudoku.Entities
{
    public struct SimpleSudoku
    {
        public SimpleSudoku(byte[,] sudokuGrid)
        {
            Grid = sudokuGrid;
            Size = Grid.GetLength(0);
            RegionSize = (int)Math.Sqrt(Size);
        }

        public byte[,] Grid { get; set; }

        public int RegionSize { get; }

        public int Size { get; }

        public SudokuWithLegalValues ToSudokuWithLegalValues()
        {
            SudokuGrid resultGrid = new SudokuGrid { };

            for (int rowIndex = 0; rowIndex < Size; rowIndex++)
            {
                resultGrid.Add(new List<List<byte>> { });

                for (int colIndex = 0; colIndex < Size; colIndex++)
                {
                    resultGrid[rowIndex].Add(GetPossibleValuesSet(Grid[rowIndex, colIndex], Size));
                }
            }

            SudokuWithLegalValues result = new SudokuWithLegalValues(resultGrid);
            return result;
        }

        private List<byte> GetPossibleValuesSet(byte originalValue, int regionSize)
        {
            if (originalValue > 0)
            {
                return new List<byte> { originalValue };
            }
            else
            {
                return GetSetFilledWithPossibleValues(regionSize);
            }
        }

        private List<byte> GetSetFilledWithPossibleValues(int regionSize)
        {
            List<byte> result = new List<byte> { };

            for (byte i = 1; i <= regionSize; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}