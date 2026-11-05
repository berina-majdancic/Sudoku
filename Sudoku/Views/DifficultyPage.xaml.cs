namespace Sudoku.Views;

public partial class DifficultyPage : ContentPage
{
	public DifficultyPage()
	{
		InitializeComponent();
	}
    private async void OnEasyClicked(object sender, EventArgs e)
    {
        await StartGame("Easy");
    }
    private async void OnMediumClicked(object sender, EventArgs e)
    {
        await StartGame("Medium");
    }
    private async void OnHardClicked(object sender, EventArgs e)
    {
        await StartGame("Hard");
    }

    }
    private async Task StartGame(string difficulty)
    {
        await Navigation.PushAsync(new GamePage(difficulty));
    }
}