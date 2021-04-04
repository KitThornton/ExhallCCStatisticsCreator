namespace Entities
{
    public class BattingSeason
    {
        public string PlayerName { get; set; }
        public int Year { get; set; }
        public int Matches { get; set; }
        public int Innings { get; set; }
        public int NotOuts { get; set; }
        public int Runs { get; set; }
        public string HighScore { get; set; }
        public decimal Average { get; set; }
        public int Fifties { get; set; }
        public int Hundreds { get; set; }
    }
}