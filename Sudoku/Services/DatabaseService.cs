using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        public DatabaseService()
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "sudoku.db3");
            _database = new SQLiteAsyncConnection(databasePath);
        }
        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<Models.SudokuPuzzles>();
            await _database.CreateTableAsync<Models.GameResults>();
        }
    }
}
