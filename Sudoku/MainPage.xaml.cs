
using Sudoku.Services;
using Sudoku.Views;

namespace Sudoku
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public MainPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }
        private async void OnNewGameClicked(object sender,EventArgs e)
        {
            await Navigation.PushAsync(new DifficultyPage(_databaseService));
        }

        private async void OnStatisticsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResultsPage());
        }
    }
}
