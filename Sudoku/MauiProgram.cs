using Microsoft.Extensions.Logging;
using Sudoku.Services;
namespace Sudoku
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Sansation-Regular.ttf", "SansationRegular");
                    fonts.AddFont("Sansation-Bold.ttf", "SansationBold");
                });
            builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
