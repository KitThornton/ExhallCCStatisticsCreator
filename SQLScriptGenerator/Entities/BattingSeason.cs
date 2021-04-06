using System;

namespace Entities
{
    public class BattingSeason
    {
        public string PlayerName { get; set; }
        public string Year { get; set; }
        public int Matches { get; set; }
        public int Innings { get; set; }
        public int NotOuts { get; set; }
        public int Runs { get; set; }
        public HighScore HighScore { get; set; }
        public decimal? Average { get; set; }
        public int Fifties { get; set; }
        public int Hundreds { get; set; }
    }
}