using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuCracker.SudokuStructure
{
    public class Grille
    {
        public char[] Symbols { get;  private set; }
        public string Name { get;  private set; }
        public string Date { get;  private set; }
        public string Comment { get;  private set; }
        public Case[,] Cases { get; private set; }
        public int Size { get; set; }
        public int RegionSize { get; set; }
        public int Difficulty { get; private set; }

        public List<string> ErrorMessagesList = new List<string>();

        public bool IsValid { get; set; }
        public bool IsSolved { get; set; }

        public Grille(string comment, string name, string date, char[] symbols)
        {
            Name = name;
            Comment = comment;
            Date = date;
            Symbols = symbols;
            IsValid = true;
            Size = Symbols.Count();
            RegionSize = (int) Math.Sqrt(Size);
            Difficulty = 0;
        }

        /// <summary>
        /// Set all cases from a set of lines
        /// </summary>
        /// <param name="lines">Enumerable of strings, each string being a row</param>
        public void InitializeCases(IEnumerable<string> lines )
        {
            int count = Symbols.Count();
            Cases = new Case[count, count];
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Cases[i,j] = new Case(lines.ElementAt(i).ElementAt(j));
                }
            }
        }

        /// <summary>
        /// Init all cases with empty value
        /// </summary>
        public void InitializeCases()
        {
            Cases= new Case[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Cases[i,j] = new Case(Case.EmptyCase);
                }
            }
        }

        /// <summary>
        /// Recalculate the difficulty (number of setted values in the grid)
        /// </summary>
        public void RefreshDifficulty()
        {
            Difficulty = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Cases[i, j].Value != Case.EmptyCase)
                    {
                        Difficulty++;
                    }
                }
            }
        }

        /// <summary>
        /// Init all hypothesies on each empty case
        /// </summary>
        public void InitAllPossibleValues()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Cases[i, j].Value == Case.EmptyCase)
                    {
                        Cases[i, j].Hypothesies = new List<char>(Symbols);
                    }
                    else
                    {
                        Cases[i, j].Hypothesies = new List<char>();
                    }
                }
            }
        }

        /// <summary>
        /// Make all cases to a string
        /// </summary>
        /// <returns>The string containing all cases values, each line separated with NewLine</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < Symbols.Count(); i++)
            {
                for (int j = 0; j < Symbols.Count(); j++)
                {
                    result.Append(Cases[i, j].Value);
                }
                result.Append("\n");
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        /// <summary>
        /// Remove an hypothesis from a specific place, including line, column and block
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public void ClearPossibleValue(char value, int line, int col)
        {
            ClearPossibilityOnLine(value, line);
            ClearPossibilityOnColumn(value, col);
            ClearPossibilityOnBlock(value, line, col);
        }

        /// <summary>
        /// Remove hypothesis from a line
        /// </summary>
        /// <param name="value">The hypothesis value to be removed</param>
        /// <param name="line">The line where the hypothesis will be removed</param>
        private void ClearPossibilityOnLine(char value, int line)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Cases[line, i].Hypothesies.Count > 0)
                {
                    Cases[line, i].Hypothesies.Remove(value);
                }
            }
        }

        /// <summary>
        /// Remove hypothesis from a column
        /// </summary>
        /// <param name="value">The hypothesis value to be removed</param>
        /// <param name="col">The column where the hypothesis will be removedz</param>
        private void ClearPossibilityOnColumn(char value, int col)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Cases[i, col].Hypothesies.Count > 0)
                {
                    Cases[i, col].Hypothesies.Remove(value);
                }
            }
        }

        /// <summary>
        /// Remove an hypothesis from a block
        /// </summary>
        /// <param name="value">Tthe hypothesis value to be removed</param>
        /// <param name="line">Index of calling line</param>
        /// <param name="col">Index of calling ccolumn</param>
        private void ClearPossibilityOnBlock(char value, int line, int col)
        {
            int startLine = line - (line%RegionSize);
            int startCol = col - (col%RegionSize);

            for (int i = startLine; i < startLine + RegionSize; i++)
            {
                for (int j = startCol; j < startCol + RegionSize; j++)
                {
                    if (Cases[i, j].Hypothesies.Count > 0)
                    {
                        Cases[i, j].Hypothesies.Remove(value);
                    }
                }
            }
        }
    }
}
