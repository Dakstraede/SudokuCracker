using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SudokuCracker.SudokuStructure
{
    public static class FileSaver
    {
        public static bool SaveSolvedGrids(IList<Grille> grids, string file)
        {
            try
            {
                var stream = File.CreateText(file);

                var solved = from grille in grids
                    where grille.IsSolved
                    select grille;
                foreach (var grid in solved)
                {
                    int count = grid.Cases.GetLength(1);
                    stream.WriteLine(grid.Comment);
                    stream.WriteLine(grid.Name);
                    stream.WriteLine(grid.Date);
                    stream.WriteLine(grid.Symbols);
                    for (int i = 0; i < count; i++)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            stream.Write(grid.Cases[i,j].Value);
                        }
                        stream.WriteLine();
                    }
                }
                stream.Close();
                return true;
            }
            catch (IOException)
            {
                return false;
            }

        }
    }
}
