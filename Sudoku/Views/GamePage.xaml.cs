using static SQLite.TableMapping;

namespace Sudoku.Views;
public partial class GamePage : ContentPage
{
    private readonly string _difficulty;
    private int[,] _board= new int[9,9];
    private Button[,] _cells= new Button[9, 9];

    private int _selectedRow = -1;
    private int _selectedColumn = -1;

    public GamePage(string difficulty)
    {
        InitializeComponent();
        _difficulty = difficulty;
        DifficultyLabel.Text = $"Difficulty:{_difficulty}";
        CreateSudokuBoard();
        CreateNumberPad();
    }

    private void CreateSudokuBoard()
    {

        for (int i=0;i<9;i++)
        {
            SudokuBoard.RowDefinitions.Add(
                new RowDefinition(GridLength.Star));
            SudokuBoard.ColumnDefinitions.Add(
                new ColumnDefinition(GridLength.Star));
        }

        for (int row=0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Button cell = new Button
                {
                    Text = "",
                    FontSize = 17,
                    Padding = 0,
                    CornerRadius = 0,
                    BackgroundColor = Colors.White,
                    TextColor = Color.FromArgb("#252525"),
                    Margin = new Thickness(
                        col % 3 == 0 ? 2 : 0,
                        row % 3 == 0 ? 2 : 0,
                        col == 8 ? 2 : 0,
                        row == 8 ? 2 : 0)
                };
                int currentRow = row;
                int currentColumn = col;

                cell.Clicked +=(sender,args)=>
                {
                    _selectedRow = currentRow;
                    _selectedColumn = currentColumn;
                    HighlightSelectedCell();
                };
                _cells[row, col] = cell;
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);

            }
        }
        SudokuBoard.SizeChanged += (sender, args) =>
        {
            SudokuBoard.HeightRequest = SudokuBoard.Width;
        };
    }
    private void CreateNumberPad()
    {
        for (int i = 1; i <= 9; i++)
        {
            Button numberButton = new Button
            {
                Text = i.ToString(),
                FontSize = 17,
                Padding = 0,
                BackgroundColor = Color.FromArgb("#E5E5DD"),
                TextColor = Color.FromArgb("#252525"),
                CornerRadius = 8
            };

            int number = i;

            numberButton.Clicked += (sender, args) =>
            {
                EnterNumber(number);
            };

            NumberPad.Add(numberButton);
            Grid.SetColumn(numberButton, i - 1);
        }
    }
    private void SelectCell(int row, int column)
    {
        // Reset previous selection
        if (_selectedRow != -1)
        {
            _cells[_selectedRow, _selectedColumn]
                .BackgroundColor = Colors.White;
        }

        _selectedRow = row;
        _selectedColumn = column;

        // Highlight selected cell
        _cells[row, column].BackgroundColor =
            Color.FromArgb("#C9D9E8");
    }
    private void EnterNumber(int number)
    {
        if (_selectedRow == -1)
            return;

        _board[_selectedRow, _selectedColumn] = number;

        _cells[_selectedRow, _selectedColumn].Text =
            number.ToString();
    }
    private void HighlightSelectedCell()
    {
    }

   
    private void OnEraseClicked(object sender, EventArgs e)
    {
        if (_selectedRow == -1)
            return;

        _board[_selectedRow, _selectedColumn] = 0;

        _cells[_selectedRow, _selectedColumn].Text = "";
    }

    // Hint
    private async void OnHintClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync(
            "Hint",
            "Hint functionality will be implemented later.",
            "OK");
    }
}