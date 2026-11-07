using Sudoku.Services;
using System.Diagnostics;
using static SQLite.TableMapping;

namespace Sudoku.Views;

public partial class GamePage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly int[,] _solution = new int[9, 9];
    private readonly bool[,] _fixedCells = new bool[9, 9];
    private readonly string _difficulty;
    private int[,] _board = new int[9, 9];
    private Button[,] _cells = new Button[9, 9];
    private bool _isGameOver = false;
    private bool _isGameWon = false;
    private bool _mistakeMade = false;
    private short _mistakesCount = 0;
    private int _selectedRow = -1;
    private int _selectedColumn = -1;
    private int[] _mistakePosition = new int[2];
    private  System.Diagnostics.Stopwatch _stopwatch= new();
    private IDispatcherTimer _timer;

    public GamePage(string difficulty, DatabaseService databaseService)
    {
        InitializeComponent();
        _difficulty = difficulty;
        _databaseService = databaseService;
        DifficultyLabel.Text = $"Difficulty:{_difficulty}";
        CreateSudokuBoard();
        CreateNumberPad();
        _ = LoadPuzzleAsync();
        StartTimer();
    }

    private void CreateSudokuBoard()
    {
        SudokuBoard.RowDefinitions.Clear();
        SudokuBoard.ColumnDefinitions.Clear();
        for (int i = 0; i < 11; i++)
        {
            SudokuBoard.RowDefinitions.Add(
              new RowDefinition(i == 3 || i == 7
           ? new GridLength(3)
           : GridLength.Star));

            SudokuBoard.ColumnDefinitions.Add(
                new ColumnDefinition(i == 3 || i == 7
                    ? new GridLength(3)
                    : GridLength.Star));
        }

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Button cell = new()
                {
                    Text = "",
                    FontSize = 17,
                    Padding = 0,
                    CornerRadius = 0,
                    BackgroundColor = Color.FromArgb("#252525"),
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    BorderColor = Colors.DarkGrey,
                    BorderWidth = 1,
                    Margin = 0
                };

                int gridRow = row + (row / 3);
                int gridColumn = col + (col / 3);

                SudokuBoard.Add(cell, gridColumn, gridRow);
                int currentRow = row;
                int currentColumn = col;

                cell.Clicked += (sender, args) =>
                {
                    HighlightSelectedCell(currentRow, currentColumn);
                };
                _cells[row, col] = cell;

            }
        }
        AddThickLines();

        SudokuBoard.SizeChanged += (sender, args) =>
        {
            SudokuBoard.HeightRequest = SudokuBoard.Width;
        };
    }
    private void CreateNumberPad()
    {
        for (int i = 1; i <= 9; i++)
        {
            Button numberButton = new()
            {
                Text = i.ToString(),
                FontSize = 17,
                Padding = 0,
                BackgroundColor = Color.FromArgb("#252525"),
                TextColor = Color.FromArgb("#E5E5DD"),
                CornerRadius = 8,
                BorderWidth = 2,
                BorderColor = Colors.DarkGrey,

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
    private void HighlightSelectedCell(int row, int column)
    { 
        bool selectedMistake= (_selectedRow == _mistakePosition[0] && _selectedColumn == _mistakePosition[1])? true : false ;
        
        if (_selectedRow != -1 && !selectedMistake)
        {
            _cells[_selectedRow, _selectedColumn].BackgroundColor = Color.FromArgb("#252525");
        }
       
        _selectedRow = row;
        _selectedColumn = column;
        if (!(_mistakePosition[0] == row && _mistakePosition[1] == column))
            _cells[_selectedRow, _selectedColumn].BackgroundColor = Colors.Grey;
    }
    private async Task EnterNumber(int number)
    {
        if (_selectedRow == -1 || _mistakeMade || _fixedCells[_selectedRow, _selectedColumn])
            return;

        _board[_selectedRow, _selectedColumn] = number;

        _cells[_selectedRow, _selectedColumn].Text =
            number.ToString();
        if (!CheckSolution())
        {
            _cells[_selectedRow, _selectedColumn].BackgroundColor = Colors.Red;
            _mistakesCount++;
            MistakesLabel.Text = _mistakesCount.ToString();
            _mistakeMade = true;
            _mistakePosition[0] = _selectedRow;
            _mistakePosition[1] = _selectedColumn;
        }
        else
            CheckSolved();
       
    }
    private bool CheckSolution()
    {
        if (_board[_selectedRow, _selectedColumn]==_solution[_selectedRow, _selectedColumn])
            return true;
        return false;
    }

    private async void CheckSolved()
    {
        for(int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (_board[row, col] != _solution[row, col])
                    return;
            }
        }
        await DisplayAlertAsync("Congratulations!", "You solved the puzzle!", "OK");
        SendBackButtonPressed();
    }
    private void OnEraseClicked(object sender, EventArgs e)
    {
        if (_selectedRow == -1 || _fixedCells[_selectedRow, _selectedColumn])
            return;
        if (_mistakePosition[0] == _selectedRow && _mistakePosition[1] == _selectedColumn)
        {
            _cells[_selectedRow, _selectedColumn].BackgroundColor = Color.FromArgb("#252525");
            _mistakeMade = false;
            _mistakePosition = [-1, -1];
        }

        _board[_selectedRow, _selectedColumn] = 0;

        _cells[_selectedRow, _selectedColumn].Text = "";
    }

    // Hint
    private async void OnHintClicked(object sender, EventArgs e)
    {
        if (_selectedRow == -1 || _fixedCells[_selectedRow, _selectedColumn] || _mistakeMade)
            return;
        int number = _solution[_selectedRow, _selectedColumn];
        _board[_selectedRow, _selectedColumn] = number;
        _cells[_selectedRow, _selectedColumn].Text= number.ToString();
        CheckSolved();
    }
    private async Task LoadPuzzleAsync()
    {
        Models.SudokuPuzzles? puzzle = await _databaseService.GetRandomPuzzleAsync(_difficulty);

        if (puzzle == null)
        {
            await DisplayAlertAsync(
                "Error",
                "No puzzle found for the selected difficulty.",
                "OK");
            return;
        }

        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int column = i % 9;
            int puzzleValue = puzzle.Puzzle[i] - '0';
            int solutionValue = puzzle.Solution[i] - '0';
            _board[row, column] = puzzleValue;
            _solution[row, column] = solutionValue;
            if (puzzleValue != 0)
            {
                _fixedCells[row, column] = true;
                _cells[row, column].Text = puzzleValue.ToString();
                _cells[row, column].TextColor = Color.FromArgb("#E5E5DD");
                _cells[row, column].BackgroundColor = Color.FromArgb("#252525");
            }
        }

    }
    private void AddThickLines()
    {
        for (int i = 3; i <= 7; i += 4)
        {
            BoxView line = new BoxView
            {
                Color = Colors.White,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                InputTransparent = true
            };

            SudokuBoard.Add(line, i, 0);
            Grid.SetRowSpan(line, 11);
        }
        for (int i = 3; i <= 7; i += 4)
        {
            BoxView line = new BoxView
            {
                Color = Colors.White,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                InputTransparent = true
            };
            SudokuBoard.Add(line, 0, i);
            Grid.SetColumnSpan(line, 11);
        }
    }
    private void StartTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) =>
        {

                TimeSpan ts = _stopwatch.Elapsed;
                TimerLabel.Text = string.Format("{0:00}:{1:00}",ts.Minutes, ts.Seconds );

        };
        _stopwatch.Start();
        _timer.Start();
    }
}
