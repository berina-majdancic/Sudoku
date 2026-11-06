using SQLite;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private bool _isInitialized;
        public DatabaseService()
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "sudoku.db3");
            _database = new SQLiteAsyncConnection(databasePath);
        }
        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initLock.WaitAsync();

            try
            {
                if (_isInitialized)
                    return;

                await _database.CreateTableAsync<SudokuPuzzles>();
                await _database.CreateTableAsync<GameResults>();

                await SeedPuzzlesAsync();

                _isInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task SeedPuzzlesAsync()
        {
            var existingPuzzles = await _database.Table<Models.SudokuPuzzles>().ToListAsync();
            if (existingPuzzles.Count == 0)
            {
                var puzzles = new List<Models.SudokuPuzzles>
                {
                    new Models.SudokuPuzzles
                    {
                        Difficulty = "Easy",
                        Puzzle = "530070000600195000098000060800060003400803001700020006060000280000419005000080079",
                        Solution = "534678912672195348198342567859761423426853791713924856961537284287419635345286179"
                    },
                    new Models.SudokuPuzzles
                    {
                        Difficulty = "Medium",
                        Puzzle = "600120384008459072000006005000264030070080006940003000310000050089700000502000190",
                        Solution = ""
                    },
                    new Models.SudokuPuzzles
                    {
                        Difficulty = "Hard",
                        Puzzle = "300200000000107000706030500070009080900020004010800050009040301000702000000008006",
                        Solution = ""
                    }
                };
                await _database.InsertAllAsync(puzzles);
            }
        }
        public async Task<Models.SudokuPuzzles?> GetRandomPuzzleAsync(string difficulty)
        {
            await InitializeAsync();
            var puzzles = await _database.Table<Models.SudokuPuzzles>()
                .Where(p=> p.Difficulty == difficulty)
                .ToListAsync();
            if (puzzles.Count == 0)
                return null;
            
            var random = new Random();
            return puzzles[random.Next(puzzles.Count)];
        }
    }
}
