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
        }

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

        public void InitializeCases()
        {
            int count = Symbols.Count();
            Cases= new Case[count, count];
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Cases[i,j] = new Case(Case.EmptyCase);
                }
            }
        }

        public void InitAllPossibleValues()
        {
            int count = Symbols.Count();
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Cases[i, j].Hypothesies = new List<char>(Symbols);
                }
            }
        }

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
            return result.ToString();
        }
    }
}
