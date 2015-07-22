using System.Collections.Generic;
using System.IO;

namespace SudokuCracker.SudokuStructure
{
    public static class FileSaver
    {
        /// <summary>
        /// Save multiple grids into a new file
        /// </summary>
        /// <param name="grids">Grids to be saved</param>
        /// <param name="file">Exact location of the file where grids will be saved</param>
        /// <returns></returns>
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

        /// <summary>
        /// Save a single grid into a new file
        /// </summary>
        /// <param name="grid">The grid to be saved</param>
        /// <param name="fileName">Exact location of the file where the grid will be saved</param>
        /// <returns></returns>
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
                
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
