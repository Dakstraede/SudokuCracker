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

                
                foreach (var grid in grids)
                {
                    stream.WriteLine(grid.Comment);
                    stream.WriteLine(grid.Name);
                    stream.WriteLine(grid.Date);
                    stream.WriteLine(grid.Symbols);
                    stream.WriteLine(grid);
                }
                stream.Close();
                return true;
            }
            catch (IOException)
            {
                return false;
            }

        }

        public static bool SaveGrid(Grille grid, string fileName)
        {
            try
            {
                var stream = File.CreateText(fileName);
                stream.WriteLine(grid.Comment);
                stream.WriteLine(grid.Name);
                stream.WriteLine(grid.Date);
                stream.WriteLine(grid.Symbols);
                stream.WriteLine(grid);
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
