using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace Sudoku.Models
{
    [Table("GameResults")]
    public class GameResults
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PuzzleId { get; set; }

        public string Difficulty { get; set; } = string.Empty;

        public int TimeSeconds { get; set; }

        public int Errors { get; set; }

        public DateTime DatePlayed { get; set; }

    }
}
