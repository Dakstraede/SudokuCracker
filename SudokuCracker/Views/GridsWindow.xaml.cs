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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {     
            if (e.AddedItems.Count == 0)
            {
                ClearSudokuGridView();
                return;
            }
            RefreshGridLayout();
        }

        private FrameworkElement CreateCaseView(Case @case, int i, int j)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
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

        private void ClearSudokuGridView()
        {
            SolveButton.IsEnabled = false;
            SudokuViewGrid.Children.Clear();
            SudokuViewGrid.RowDefinitions.Clear();
            SudokuViewGrid.ColumnDefinitions.Clear();
            MessageLogBlock.Text = String.Empty;
            CommentBlock.Text = String.Empty;
        }

        private void DeleteGridButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GridListBox.SelectedIndex == -1)
                return;
            GridList.RemoveAt(GridListBox.SelectedIndex);
        }

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

            MessageLogBlock.Text = String.Format("Sudoku solved in : {0} ms -> {1} valid : {2}",time.ElapsedMilliseconds, ((Grille)GridListBox.SelectedItem).Name, validator.ExecuteTests());
        }

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
                    FrameworkElement elem = CreateCaseView(grid.Cases[i, j], i, j);
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
            if (grid.ErrorMessagesList.Count > 0)
            {
                foreach (var msg in grid.ErrorMessagesList)
                {
                    MessageLogBlock.Text += msg + '\n';
                }
            }
        }

        private void Gen9Button_OnClick(object sender, RoutedEventArgs e)
        {
            Generate(Generator.Sizes.Size9);
        }

        private void Gen16Button_OnClick(object sender, RoutedEventArgs e)
        {
            Generate(Generator.Sizes.Size16);
        }

        private void Generate(Generator.Sizes size)
        {
            var grid = Generator.Generate(size);
            GridList.Add(grid);
            GridListBox.SelectedIndex = GridListBox.Items.Count - 1;
            GridListBox.ScrollIntoView(GridListBox.SelectedItem);
        }

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
    }
}
