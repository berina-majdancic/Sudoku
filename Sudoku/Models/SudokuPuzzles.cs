using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace Sudoku.Models
{
    [Table("SudokuPuzzles")]
    public class SudokuPuzzles
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Puzzle { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;

    }
}
