using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuCracker.SudokuStructure
{
    public class Optibacktracker
    {
        public Grille UnsolvedGrid;

        private char[,] board;
        private bool[,] rowSet;
        private bool[,] columnSet;
        private bool[, ,] blockSet;
        private Dictionary<char, int> eq = new Dictionary<char, int>(); 

        public Optibacktracker(Grille grid)
        {
            UnsolvedGrid = grid;
            board = new char[UnsolvedGrid.Size, UnsolvedGrid.Size];
            rowSet = new bool[UnsolvedGrid.Size,UnsolvedGrid.Size];
            columnSet = new bool[UnsolvedGrid.Size,UnsolvedGrid.Size];
            blockSet = new bool[UnsolvedGrid.RegionSize, UnsolvedGrid.RegionSize, UnsolvedGrid.Size];
            InitArrays();
            int i = 0;
            foreach (char symbol in UnsolvedGrid.Symbols)
            {
                eq.Add(symbol, i++);
            }

        }

        /// <summary>
        /// Solve a given grid, previously initialized
        /// </summary>
        /// <returns>True if solved, False otherwise</returns>
        public bool Solve()
        {
            if (ClearTrivialPossibilities())
                return true;
            for (int j = 0; j < UnsolvedGrid.Size; j++)
            {
                for (int k = 0; k < UnsolvedGrid.Size; k++)
                {
                    if (UnsolvedGrid.Cases[j, k].Value != Case.EmptyCase)
                        set(j, k, UnsolvedGrid.Cases[j, k].Value);
                }
            }
            if (!_solve())
                return false;
            for (int i = 0; i < UnsolvedGrid.Size; i++)
            {
                for (int j = 0; j < UnsolvedGrid.Size; j++)
                {
                    UnsolvedGrid.Cases[i, j].Value = board[i, j];
                }
            }
            return true;
        }

        /// <summary>
        /// Internal function containing backtracking algorithm
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private  bool _solve(int at = 0)
        {
            if (at == UnsolvedGrid.Size * UnsolvedGrid.Size)
                return true;
            int r = at / UnsolvedGrid.Size;
            int c = at % UnsolvedGrid.Size;
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

        /// <summary>
        /// Check whether a value is set
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="c">Column index</param>
        /// <param name="value">The value to check</param>
        /// <returns>True if set is possible, False otherwise</returns>
        public bool canSet(int r, int c, int value)
        {
            return !isSet(r, c) && !rowSet[r,value] && !columnSet[c,value] && !blockSet[r / UnsolvedGrid.RegionSize,c / UnsolvedGrid.RegionSize,value];
        }

        /// <summary>
        /// Check whether a value is already set
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="c">Column index</param>
        /// <returns>True if a value is already set, False otherwise</returns>
        public bool isSet(int r, int c)
        {
            return board[r,c] != Case.EmptyCase;
        }

        /// <summary>
        /// Set a value in a given case
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="c">Column index</param>
        /// <param name="value">The value to set</param>
        public void set(int r, int c, char value)
        {
            if (!canSet(r, c, eq[value]))
                throw new Exception();
            board[r,c] = value;
            rowSet[r,eq[value]] = columnSet[c,eq[value]] = blockSet[r / UnsolvedGrid.RegionSize,c / UnsolvedGrid.RegionSize,eq[value]] = true;
        }

        /// <summary>
        /// Unset a value in a given case
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="c">Column index</param>
        public void unSet(int r, int c)
        {
            if (isSet(r, c))
            {
                int value = eq[board[r,c]];
                board[r,c] = Case.EmptyCase;
                rowSet[r,value] = columnSet[c,value] = blockSet[r / UnsolvedGrid.RegionSize,c / UnsolvedGrid.RegionSize,value] = false;
            }
        }

        /// <summary>
        /// Init the arrays being used in the solver core
        /// </summary>
        private void InitArrays()
        {
            for (int i = 0; i < UnsolvedGrid.Size; i++)
            {
                for (int j = 0; j < UnsolvedGrid.Size; j++)
                {
                    rowSet[i, j] = columnSet[i, j] = false;
                }
            }

            for (int i = 0; i < UnsolvedGrid.RegionSize; i++)
            {
                for (int j = 0; j < UnsolvedGrid.RegionSize; j++)
                {
                    for (int k = 0; k < UnsolvedGrid.Size; k++)
                    {
                        blockSet[i, j, k] = false;
                    }
                }
            }
            for (int i = 0; i < UnsolvedGrid.Size; i++)
            {
                for (int j = 0; j < UnsolvedGrid.Size; j++)
                {
                    board[i, j] = Case.EmptyCase;
                }
            }
        }

        /// <summary>
        /// Set trivial values when there are some
        /// </summary>
        /// <returns>True if the grid is solved this way, false otherwise</returns>
        private bool ClearTrivialPossibilities()
        {
            UnsolvedGrid.InitAllPossibleValues();
             bool setValue = true;
            int count = 0;
            while (setValue)
            {
                setValue = false;
                for (int i = 0; i < UnsolvedGrid.Size; i++)
                {
                    for (int j = 0; j < UnsolvedGrid.Size; j++)
                    {
                        if (!UnsolvedGrid.Cases[i, j].IsCheckedForPossibilities && UnsolvedGrid.Cases[i, j].Value != Case.EmptyCase)
                        {
                            UnsolvedGrid.ClearPossibleValue(UnsolvedGrid.Cases[i, j].Value, i, j);
                        }
                        else if (UnsolvedGrid.Cases[i, j].Value == Case.EmptyCase && UnsolvedGrid.Cases[i, j].Hypothesies.Count == 1)
                        {
                            UnsolvedGrid.Cases[i, j].Value = UnsolvedGrid.Cases[i, j].Hypothesies.First();
                            UnsolvedGrid.ClearPossibleValue(UnsolvedGrid.Cases[i, j].Value, i, j);
                            setValue = true;
                            count++;
                        }
                    }
                }
            }
            return (count + UnsolvedGrid.Difficulty) == UnsolvedGrid.Size;
        }
    }
}
