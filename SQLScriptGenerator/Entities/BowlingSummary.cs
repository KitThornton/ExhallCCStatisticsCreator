namespace Entities
{
    public class BowlingSummary
    {
        public string PlayerName { get; set; }
        public int Matches { get; set; }
        public int Overs { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public decimal Average { get; set; }
        public int FiveWicketHauls { get; set; }
        public BestBowlFigs BestFigures { get; set; }
        public decimal Economy { get; set; }

        public class BestBowlFigs
        {
            public int Runs { get; set; }
            public int Wickets { get; set; }
        }
    }
}