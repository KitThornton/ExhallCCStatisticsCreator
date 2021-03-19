namespace ExhallCCStats.Entities
{
    // Here is the entity that we will be parsing into from the BattingSummary 
    // tab from the excel/csv file.
    public class BattingSummary
    {
        public string PlayerName { get; set; }
        
        public int Matches { get; set; }
        
        public int Innings { get; set; }
        
        public int Runs { get; set; }
        
        public int HighScore { get; set; }
        
        public decimal Average { get; set; }
        
        public int Fifties { get; set; }
        
        public int Hundereds { get; set; }
        
        public int Catches { get; set; }
        
        public int Stumpings { get; set; }
    }
}