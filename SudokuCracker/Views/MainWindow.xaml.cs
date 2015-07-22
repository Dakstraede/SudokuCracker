using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using SudokuCracker.SudokuStructure;

namespace SudokuCracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prompts the file choosing dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileChoose_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = App.DefaultSupportedExtension,
                Filter = App.SupportedExtensionFilter
            };

            var result = dlg.ShowDialog();


            if (result != true) return;

            SudFileBox.Text = dlg.FileName;
        }

        /// <summary>
        /// Detects if a file has been chosen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SudFileBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            GenerateButton.IsEnabled = SudFileBox.Text.Length != 0;
        }

        /// <summary>
        /// Launch the parsing of the grids and open the Sudoku views window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            string fileName = SudFileBox.Text;
            if (!File.Exists(fileName))
            {
                MessageBox.Show("The file you specified is not reachable anymore",
                    "File not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            List<Grille> grids;
            try
            {
                grids = ParseGridsFromFile(fileName);
            }
            catch (Exception)
            {
                MessageBox.Show("The file you specified contains format errors that can not be handled",
                    "Invalid file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            GridsWindow gridWindow = new GridsWindow(grids);
            gridWindow.Show();
            Close();
        }

        /// <summary>
        /// Create grids objects from a file, init them and check if they are valid
        /// </summary>
        /// <param name="filename">The exact location of the file containing the grids</param>
        /// <returns>The list of parsed grids</returns>
        public static List<Grille> ParseGridsFromFile(string filename)
        {
            List<Grille> grids = new List<Grille>();
            var file = new StreamReader(filename);

            string line;

            while ((line = file.ReadLine()) != null)
            {
                var grid = new Grille(line, file.ReadLine(), file.ReadLine(), file.ReadLine().ToCharArray());

                string[] tempLines = new string[grid.Symbols.Length];
                for (int j = 0; j < grid.Symbols.Length; j++)
                {
                    tempLines[j] = file.ReadLine();
                }

                var validator = new SudokuValidator(grid.Symbols, tempLines, ref grid);
                validator.ExecuteTests();
                grids.Add(grid);
            }
            return grids;
        }
    }
}
