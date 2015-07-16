using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
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
            var dlg = new OpenFileDialog
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
                MessageBox.Show("The file you specified is not reachable anymore",
                    "File not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            List<Grille> grids = new List<Grille>();

            try
            {
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
    }
}
