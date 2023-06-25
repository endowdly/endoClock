namespace Endo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public enum FillStyleMode
    {
        LeftToRight,
        TopToBottom,
    }

    class ClockCell
    {
        public readonly int Index;
        public readonly int Row;
        public readonly int Col;
        public readonly Point Location;

        public bool IsClock;
        public ClockControl Clock;

        public ClockCell(int x, int y, int idx, Point p)
        {
            Row = y;
            Col = x;
            Index = idx;
            Location = p;
        }
    }

    class ClockCellMatrix : IEnumerable<ClockCell>
    {
        int nCols, nRows;
        int r;

        ClockCell[] cells;

        int GetIndex(int r, int c)
        {
            return r * nCols + c;
        }

        public ClockCell this[int index]
        {
            get { return cells[index]; }
            set { cells[index] = value; }
        }

        public ClockCellMatrix(int maxCols, int maxRows, int clockDims)
        {
            int x, y;
            int idx;
            Point p;

            nCols = maxCols;
            nRows = maxRows;
            r = clockDims;
            cells = new ClockCell[nCols * nRows];

            for (x = 0; x < maxCols; x++)
                for (y = 0; y < maxRows; y++)
                {
                    idx = GetIndex(y, x);
                    p = new Point(x * r, y * r);
                    cells[idx] = new ClockCell(x, y, idx, p);
                }
        }

        bool isIndexBad(int idx)
        {
            return (0 > idx || idx > cells.Length) || cells.Length < 1;
        }

        public IEnumerator<ClockCell> GetEnumerator()
        {
            return ((IEnumerable<ClockCell>)cells).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ClockCell GetLeft(int idx)
        {
            if (isIndexBad(idx))
                return null;

            var newIdx = idx - 1;

            if (newIdx < 0)
                return cells[cells.Length - 1];

            return cells[newIdx];
        }

        public ClockCell GetRight(int idx)
        {
            if (isIndexBad(idx))
                return null;

            var newIdx = idx + 1;

            if (newIdx > cells.Length - 1)
                return cells[0];

            return cells[newIdx];
        }

        public ClockCell GetAbove(int idx)
        {
            if (isIndexBad(idx))
                return null;

            int newIdx;

            var cell = cells[idx];
            var newRow = cell.Row - 1;

            if (newRow < 0)
                return GetLeft(GetIndex(nRows - 1, cell.Col));

            newIdx = GetIndex(newRow, cell.Col);

            return cells[newIdx];
        }

        public ClockCell GetBelow(int idx)
        {
            if (isIndexBad(idx))
                return null;

            int newIdx;

            var cell = cells[idx];
            var newRow = cell.Row + 1;

            if (newRow >= nRows)
                return GetRight(GetIndex(0, cell.Col));

            newIdx = GetIndex(newRow, cell.Col);

            return cells[newIdx];
        }
    }
}