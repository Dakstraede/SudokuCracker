using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SudokuCracker.SudokuStructure;

namespace SudokuCracker.Views
{
    /// <summary>
    /// Interaction logic for GridsWindow.xaml
    /// </summary>
    public partial class GridsWindow : Window
    {
        public ObservableCollection<Grille> GridList { get; private set; }

        public GridsWindow(List<Grille> list )
        {
            GridList = new ObservableCollection<Grille>(list);
            InitializeComponent();
            DataContext = this;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearSudokuGridView();
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            Grille selected = (Grille)(e.AddedItems[0]);
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
            SudokuViewGrid.Children.Clear();
            SudokuViewGrid.RowDefinitions.Clear();
            SudokuViewGrid.ColumnDefinitions.Clear();
            MessageLogBlock.Text = String.Empty;
            CommentBlock.Text = String.Empty;
        }

        private void DeleteGridButton_OnClick(object sender, RoutedEventArgs e)
        {
            GridList.Remove((Grille) GridListBox.SelectedItem);
        }

        private void SolveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Backtracker solver = new Backtracker((Grille)GridListBox.SelectedItem);
            Stopwatch time = new Stopwatch();
            time.Start();
            bool res = solver.Solve();
            time.Stop();
            GridListBox.SelectedItem = solver._unSolvedGrid;
            RefreshGridLayout();
            MessageLogBlock.Text = time.ElapsedMilliseconds.ToString();
        }

        private void RefreshGridLayout()
        {
            ClearSudokuGridView();
            Grille grid = (Grille) GridListBox.SelectedItem;
            if(grid == null)
                return;

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

            CommentBlock.Text = grid.Comment;
            if (grid.ErrorMessagesList.Count > 0)
            {
                foreach (var msg in grid.ErrorMessagesList)
                {
                    MessageLogBlock.Text += msg + '\n';
                }
            }
        }
    }
}
