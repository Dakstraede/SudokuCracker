using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuCracker.SudokuStructure
{
    public class Optibacktracker
    {
        public Grille UnsolvedGrid;
        private readonly int _size;
        readonly int _regionSize;

        private char[,] board;
        private bool[,] rowSet;
        private bool[,] columnSet;
        private bool[, ,] blockSet;
        private Dictionary<char, int> eq = new Dictionary<char, int>(); 

        public Optibacktracker(Grille grid)
        {
            UnsolvedGrid = grid;
            _size = grid.Symbols.Count();
            _regionSize = (int) Math.Sqrt(UnsolvedGrid.Symbols.Count());
            board = new char[_size, _size];
            rowSet = new bool[_size,_size+1];
            columnSet = new bool[_size,_size+1];
            blockSet = new bool[_regionSize, _regionSize, _size+1];
            InitArrays();
            int i = 1;
            foreach (char symbol in UnsolvedGrid.Symbols)
            {
                eq.Add(symbol, i++);
            }
            for (int j = 0; j < _size; j++)
            {
                for (int k = 0; k < _size; k++)
                {
                    if(UnsolvedGrid.Cases[j,k].Value != Case.EmptyCase)
                        set(j,k, UnsolvedGrid.Cases[j,k].Value);
                }
            }
        }

        public bool Solve()
        {
            if (!_solve())
                return false;
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    UnsolvedGrid.Cases[i, j].Value = board[i, j];
                }
            }
            return true;
        }

        private  bool _solve(int at = 0)
        {
            if (at == _size * _size)
                return true;
            int r = at / _size;
            int c = at % _size;
            if (isSet(r, c))
                return _solve(at + 1);
            foreach(var elem in UnsolvedGrid.Symbols)
            {
                if (canSet(r, c, eq[elem]))
                {
                    set(r, c, elem);
                    if (_solve(at + 1))
                        return true;
                    unSet(r, c);
                }
            }
            return false;
        }

        public bool canSet(int r, int c, int value)
        {
            return !isSet(r, c) && !rowSet[r,value] && !columnSet[c,value] && !blockSet[r / _regionSize,c / _regionSize,value];
        }
        public bool isSet(int r, int c)
        {
            return board[r,c] != Case.EmptyCase;
        }
        public void set(int r, int c, char value)
        {
            if (!canSet(r, c, eq[value]))
                throw new Exception();
            board[r,c] = value;
            rowSet[r,eq[value]] = columnSet[c,eq[value]] = blockSet[r / _regionSize,c / _regionSize,eq[value]] = true;
        }
        public void unSet(int r, int c)
        {
            if (isSet(r, c))
            {
                int value = eq[board[r,c]];
                board[r,c] = Case.EmptyCase;
                rowSet[r,value] = columnSet[c,value] = blockSet[r / _regionSize,c / _regionSize,value] = false;
            }
        }

        private void InitArrays()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size+1; j++)
                {
                    rowSet[i, j] = columnSet[i, j] = false;
                }
            }

            for (int i = 0; i < _regionSize; i++)
            {
                for (int j = 0; j < _regionSize; j++)
                {
                    for (int k = 0; k < _size+1; k++)
                    {
                        blockSet[i, j, k] = false;
                    }
                }
            }
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    board[i, j] = Case.EmptyCase;
                }
            }
        }
    }
}
