namespace Sudoku
{
    public struct Region
    {
        public int ColEnd { get; set; }
        public int ColStart { get; set; }
        public int RowEnd { get; set; }
        public int RowStart { get; set; }

        public Region(int rowStart, int rowEnd, int colStart, int colEnd)
        {
            ColEnd = colEnd;
            ColStart = colStart;
            RowEnd = rowEnd;
            RowStart = rowStart;
        }
    }
}