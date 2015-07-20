using System.Collections.Generic;

namespace SudokuCracker.SudokuStructure
{
    public class Case
    {
        public const char EmptyCase = '.';
        public char Value { get; set; }
        public int NumberHypothesis { get; set; }
        public List<char> Hypothesies { get; set; }
        public bool IsSolvedCase { get; set; }
        public bool IsCheckedForPossibilities { get; set; }

        public Case(char c)
        {
            Value = c;
            IsCheckedForPossibilities = false;
        }
    }
}
