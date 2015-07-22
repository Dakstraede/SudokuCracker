using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SudokuCracker.SudokuStructure;

namespace SudokuCracker.Views
{
    /// <summary>
    /// Interaction logic for GridsWindow.xaml
    /// </summary>
    public partial class GridsWindow : Window
    {
        public ObservableCollection<Grille> GridList { get; private set; }
        public Grille SelectedItem { get; set; }

        public GridsWindow(List<Grille> list )
        {
            GridList = new ObservableCollection<Grille>(list);
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Detects when selected grid changes, in order to refresh the layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {     
            if (e.AddedItems.Count == 0)
            {
                ClearSudokuGridView();
                return;
            }
            RefreshGridLayout();
        }

        /// <summary>
        /// Create a case view in a specific location in the grid
        /// </summary>
        /// <param name="case">Case object where value is retrieved</param>
        /// <param name="i">Row index</param>
        /// <param name="j">Column index</param>
        /// <param name="regionSize">The size of region side, for calculating purposes</param>
        /// <returns>Case view object</returns>
        private FrameworkElement CreateCaseView(Case @case, int i, int j, int regionSize)
        {
            int regionNb = ((i/regionSize)*regionSize) + (j/regionSize);
            
            Border border = new Border
            {
                BorderBrush = Brushes.Black, 
                BorderThickness = new Thickness(1)
            };
            if ((regionSize%2 == 0 && (((regionNb/regionSize)%2 == 0 && regionNb%2 == 0) || ((regionNb/regionSize)%2 == 1 && regionNb%2 == 1)))
                || (regionSize % 2 == 1 && (((regionNb / regionSize) % 2 == 0 && regionNb % 2 == 0) || (regionNb/regionSize)%2 ==1 && regionNb%2 == 0)))
                border.Background = Brushes.LightGray;
            string text = (@case.Value.Equals(Case.EmptyCase) ? "" : @case.Value.ToString());
            border.Child = new TextBlock()
            {
                Text = text, 
                TextAlignment = TextAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Center
                
            };
            Grid.SetColumn(border, j);
            Grid.SetRow(border, i);
            return border;
        }

        /// <summary>
        /// Clean the sudoku grid view
        /// </summary>
        private void ClearSudokuGridView()
        {
            SolveButton.IsEnabled = false;
            SudokuViewGrid.Children.Clear();
            SudokuViewGrid.RowDefinitions.Clear();
            SudokuViewGrid.ColumnDefinitions.Clear();
            MessageLogBlock.Text = String.Empty;
            CommentBlock.Text = String.Empty;
        }

        /// <summary>
        /// Delete the current selected grid from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGridButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GridListBox.SelectedIndex == -1)
                return;
            GridList.RemoveAt(GridListBox.SelectedIndex);
        }

        /// <summary>
        /// Solve the current selected grid and display it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SolveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var solver = new Optibacktracker((Grille)GridListBox.SelectedItem);
            Stopwatch time = new Stopwatch();
            time.Start();
            bool res = solver.Solve();

            time.Stop();
            
            if (res)
            {
                Grille solving = solver.UnsolvedGrid;
                solving.IsSolved = true;
                GridListBox.SelectedItem = solver.UnsolvedGrid;
            }
            
            RefreshGridLayout();
            var validator = new SudokuValidator(solver.UnsolvedGrid);

            MessageLogBlock.Text += String.Format("Sudoku solved in : {0} ms -> {1} valid : {2}",time.ElapsedMilliseconds, ((Grille)GridListBox.SelectedItem).Name, validator.ExecuteTests());
        }

        /// <summary>
        /// Refresh the sudoku grid view with current selected grid informations
        /// </summary>
        private void RefreshGridLayout()
        {
            ClearSudokuGridView();
            Grille grid = (Grille) GridListBox.SelectedItem;
            if(grid == null)
                return;
            SolveButton.IsEnabled = grid.IsValid && !grid.IsSolved;
            int gridSize = grid.Symbols.Count();
            for (int i = 0; i < gridSize; i++)
            {
                SudokuViewGrid.RowDefinitions.Add(new RowDefinition());
                SudokuViewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    FrameworkElement elem = CreateCaseView(grid.Cases[i, j], i, j, grid.RegionSize);
                    SudokuViewGrid.Children.Add(elem);
                }
            }
            int count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (grid.Cases[i, j].Value != Case.EmptyCase)
                        count++;
                }
            }
            if(!grid.IsSolved)
                grid.RefreshDifficulty();
            CommentBlock.Text = String.Format("{0} \n Difficulty : {1} / {2}", grid.Comment, grid.Difficulty, grid.Size*grid.Size);
            MessageLogBlock.Text += String.Format("Title : {0} \n Date : {1} \n Character set : {2} \n", grid.Name,
                grid.Date, new string(grid.Symbols));
            if (grid.ErrorMessagesList.Count > 0)
            {
                foreach (var msg in grid.ErrorMessagesList)
                {
                    MessageLogBlock.Text += msg + '\n';
                }
            }
        }

        /// <summary>
        /// Generate a 9 size Sudoku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen9Button_OnClick(object sender, RoutedEventArgs e)
        {
            Generate(Generator.Sizes.Size9);
        }

        /// <summary>
        /// Generate a 16 size Sudoku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen16Button_OnClick(object sender, RoutedEventArgs e)
        {
            Generate(Generator.Sizes.Size16);
        }

        /// <summary>
        /// Generate a grid of the given size and add it to the list
        /// </summary>
        /// <param name="size"></param>
        private void Generate(Generator.Sizes size)
        {
            var grid = Generator.Generate(size);
            GridList.Add(grid);
            GridListBox.SelectedIndex = GridListBox.Items.Count - 1;
            GridListBox.ScrollIntoView(GridListBox.SelectedItem);
        }

        /// <summary>
        /// Save all solved grids to a single file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Onclick(object sender, RoutedEventArgs e)
        {
            var solved = from grille in GridList
                         where grille.IsSolved
                         select grille;
            if (!solved.Any())
                return;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Solved Sudokus",
                DefaultExt = App.DefaultSupportedExtension,
                Filter = App.SupportedExtensionFilter
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                FileSaver.SaveSolvedGrids(solved.ToList(), dlg.FileName);
            }
        }

        /// <summary>
        /// Save the selected grid to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Onclick_Save(object sender, RoutedEventArgs e)
        {
            if (GridListBox.SelectedIndex == -1)
                return;
            var grid = (Grille) GridListBox.SelectedItem;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = String.Format("Sudoku {0} - {1}", grid.Name, grid.Date),
                DefaultExt = App.DefaultSupportedExtension,
                Filter = App.SupportedExtensionFilter
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                FileSaver.SaveGrid(grid, dlg.FileName);
            }
        }

        /// <summary>
        /// Import new grids to the list from a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGridsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = App.DefaultSupportedExtension,
                Filter = App.SupportedExtensionFilter
            };

            var result = dlg.ShowDialog();


            if (result != true) 
                return;

            try
            {
                var newGrids = MainWindow.ParseGridsFromFile(dlg.FileName);
                foreach (var newGrid in newGrids)
                {
                    GridList.Add(newGrid);
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
            GridListBox.SelectedIndex = GridListBox.Items.Count - 1;
            GridListBox.ScrollIntoView(GridListBox.SelectedItem);
        }
    }
}
