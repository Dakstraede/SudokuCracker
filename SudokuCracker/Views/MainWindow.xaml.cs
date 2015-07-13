using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void SudFileBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            GenerateButton.IsEnabled = SudFileBox.Text.Length != 0;
        }

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            string fileName = SudFileBox.Text;
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File not found",
                    "The file you specified is not reachable anymore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            List<Grille> grids = new List<Grille>();

            try
            {

                var lineCount = File.ReadLines(fileName).Count();

                var file = new StreamReader(fileName);
                string line;
                
                while ((line = file.ReadLine()) != null)
                {
                    var grid = new Grille(line, file.ReadLine(), file.ReadLine(), file.ReadLine().ToCharArray());

                    string[] tempLines = new string[grid.Symbols.Length];
                    for (int j = 0; j < grid.Symbols.Length; j++)
                    {
                        tempLines[j] = file.ReadLine();
                    }

                    SudokuValidator validator = new SudokuValidator(grid.Symbols, tempLines, ref grid);
                    validator.ExecuteTests();
                    grids.Add(grid);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return;
            }

            GridsWindow gridWindow = new GridsWindow(grids);
            gridWindow.Show();
            Close();
        }
    }
}
