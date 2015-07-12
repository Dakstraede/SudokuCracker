namespace SudokuCracker.SudokuStructure
{
    public class Case
    {
        public const string EmptyCase = ".";
        public char Value { get; private set; }
        public int NumberHypothesis { get; private set; }
        public int[] Hypothesies { get; private set; }
        public bool IsSolvedCase { get; set; }

        public Case(char c)
        {
            Value = c;
            NumberHypothesis = 0;
        }
    }
}
