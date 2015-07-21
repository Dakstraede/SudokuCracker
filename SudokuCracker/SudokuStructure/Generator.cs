using System;
using System.Collections.Generic;
using System.Linq;


namespace SudokuCracker.SudokuStructure
{
    public static class Generator
    {
        public static readonly List<char> Tab9 = new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9'};
        public static readonly List<char> Tab16 = new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9','A','B','C','D','E','F','G'};
        public static readonly List<char> Tab25 = new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P'};
        public static int NbGeneratedGrids = 1;
        public enum Sizes{ Size9, Size16, Size25 }

        public static Grille Generate(Sizes size = Sizes.Size9)
        {
            List<char> chosen;
            if (size == Sizes.Size9)
                chosen = Tab9;
            else
                chosen = Tab16;

            Grille grid = new Grille("This is a generated grid", "GenGrid "+NbGeneratedGrids, DateTime.Now.ToShortDateString(), chosen.Shuffle().ToArray());
            grid.InitializeCases();
            


            var solver = new Optibacktracker(grid);
            solver.Solve();
            NbGeneratedGrids++;
            var res = MakeHoles(solver.UnsolvedGrid);
            res.RefreshDifficulty();
            return res;

        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        private static Grille MakeHoles(Grille grid)
        {
            Random rd = new Random();
            int count = grid.Symbols.Count();
            int nbHoles = (int)(count*count*0.6);

            int making = 0;
            while (making < nbHoles)
            {
                int i = rd.Next(0, count);
                int j = rd.Next(0, count);
                if (grid.Cases[i,j].Value == Case.EmptyCase)
                {
                    continue;
                }
                grid.Cases[i, j].Value = Case.EmptyCase;
                making++;
            }
            return grid;
        }
    }
}
