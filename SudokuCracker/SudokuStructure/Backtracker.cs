using System;
using System.Linq;

namespace SudokuCracker.SudokuStructure
{

    public class Backtracker
    {

        public Grille UnsolvedGrid;
        private readonly int _size;

        public Backtracker(Grille grid)
        {
            UnsolvedGrid = grid;
            _size = grid.Symbols.Count();
        }

        public bool Solve(int position = 0)
        {
            if (position == _size*_size)
                return true;

            int i = position/_size, j = position%_size;
            if (!UnsolvedGrid.Cases[i, j].Value.Equals(Case.EmptyCase))
                return Solve(position + 1);

            foreach (char symbol in UnsolvedGrid.Symbols)
            {
                if (AbsentOnColumn(symbol, j) && AbsentOnRow(symbol, i) && AbsentOnRegion(symbol, i, j))
                {
                    UnsolvedGrid.Cases[i, j].Value = symbol;

                    if (Solve(position + 1))
                        return true;
                }
            }
            UnsolvedGrid.Cases[i, j].Value = Case.EmptyCase;
            return false;
        }


        private bool AbsentOnRow(char value, int row)
        {
            for (int j = 0; j < _size; j++)
            {
                if (UnsolvedGrid.Cases[row, j].Value == value)
                    return false;
            }
            return true;
        }

        private bool AbsentOnColumn(char value, int col)
        {
            for (int i = 0; i < _size; i++)
            {
                if (UnsolvedGrid.Cases[i, col].Value == value)
                    return false;
            }
            return true;
        }

        private bool AbsentOnRegion(char value, int line, int col)
        {
            int regionSize = (int)Math.Sqrt(UnsolvedGrid.Symbols.Count());
            int i = line - (line%regionSize), j = col - (col%regionSize);
            for (line = i; line < i + regionSize; line++)
                for(col = j; col < j+regionSize; col++)
                    if (UnsolvedGrid.Cases[line, col].Value == value)
                        return false;
            return true;
        }
    }
}
