using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCracker.SudokuStructure
{

    public class Backtracker
    {

        public Grille _unSolvedGrid;
        private readonly int _size;

        public Backtracker(Grille grid)
        {
            _unSolvedGrid = grid;
            _size = grid.Symbols.Count();
        }

        private class Node
        {
            private int _i, _j;
            private int _possibleValues;
        }

        public bool Solve(int position = 0)
        {
            if (position == _size*_size)
                return true;

            int i = position/_size, j = position%_size;

            if (!_unSolvedGrid.Cases[i, j].Value.Equals(Case.EmptyCase))
                return Solve(position + 1);

            foreach (char symbol in _unSolvedGrid.Symbols)
            {
                if (AbsentOnColumn(symbol, j) && AbsentOnRow(symbol, i) && AbsentOnRegion(symbol, i, j))
                {
                    _unSolvedGrid.Cases[i, j].Value = symbol;

                    if (Solve(position + 1))
                        return true;
                }
            }
            _unSolvedGrid.Cases[i, j].Value = Case.EmptyCase;
            return false;
        }


        private bool AbsentOnRow(char value, int row)
        {
            for (int j = 0; j < _size; j++)
            {
                if (_unSolvedGrid.Cases[row, j].Value == value)
                    return false;
            }
            return true;
        }

        private bool AbsentOnColumn(char value, int col)
        {
            for (int i = 0; i < _size; i++)
            {
                if (_unSolvedGrid.Cases[i, col].Value == value)
                    return false;
            }
            return true;
        }

        private bool AbsentOnRegion(char value, int line, int col)
        {
            int regionSize = (int)Math.Sqrt(_unSolvedGrid.Symbols.Count());
            int _i = line - (line%regionSize), _j = col - (col%regionSize);

            for (line = _i; line < (_i + regionSize); line++)
                for(col = _j; col < _j+regionSize; col++)
                    if (_unSolvedGrid.Cases[line, col].Value == value)
                        return false;
            return true;
        }
    }
}
