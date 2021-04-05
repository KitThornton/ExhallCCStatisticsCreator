namespace Entities
{
    public class BowlingSeason
    {
        public string PlayerName { get; set; }
        public string Year { get; set; }
        public int Overs { get; set; }
        public int Maidens { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public decimal? Average { get; set; }
        public int? FiveWicketHauls { get; set; }
        public BestBowlingFigures BestFigures { get; set; }
    }
}