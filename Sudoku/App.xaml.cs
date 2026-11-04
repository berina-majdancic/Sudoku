using Microsoft.Extensions.DependencyInjection;
using Sudoku.Services;
namespace Sudoku
{
    public partial class App : Application
    {
        public App(DatabaseService databaseService)
        {
            InitializeComponent();
            MainPage = new AppShell();
            InitializeDatabase(databaseService);
        }

        private async void InitializeDatabase(DatabaseService databaseService)
        {
            await databaseService.InitializeAsync();
        }
    }
}