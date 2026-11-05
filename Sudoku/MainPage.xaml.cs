
using Sudoku.Views;

namespace Sudoku
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnNewGameClicked(object sender,EventArgs e)
        {
            await Navigation.PushAsync(new DifficultyPage());
        }

        private async void OnStatisticsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResultsPage());
        }
    }
}
