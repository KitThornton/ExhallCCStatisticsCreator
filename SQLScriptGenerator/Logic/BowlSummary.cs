using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class BowlSummary
    {
        public static StringBuilder CreateInsertScript(List<BowlingSummary> dataList)
        {
            var sb = new StringBuilder();
            var statsTableName = "\"Summary\".\"Bowling\"";
            
            // Here we assume that the players table has already been created
            // TODO: We should add in a MERGE statement to add in data to the 
            // players table of an entry does not exist.
            foreach (var d in dataList)
            {
                
                sb.Append(CreateStatsInsertStatement(d, statsTableName));
            }
            
            return sb;
        }

        public static string CreateStatsInsertStatement(BowlingSummary d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (""PlayerId"", ""Matches"", ""Overs"", ""Runs"", ""Wickets"",
                ""FiveWicketHauls"", ""BestFigsRuns"", ""BestFigsWickets"",
                ""Economy"", ""Average"")
SELECT PlayerID, {d.Matches}, {d.Overs}, {d.Runs}, {d.Wickets}, {d.FiveWicketHauls}, {d.BestFigures.Runs}, 
            {d.BestFigures.Wickets}, {d.Economy}, {d.Average}
FROM ""Players"".""Details""
WHERE playername = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        public static BowlingSummary ParseData(string[] args)
        {
            Decimal.TryParse(args[6], out var average);
            Decimal.TryParse(args[9], out var economy);
            
            var name = args[1];
            if (name.Contains('\''))
            {
                name = name.Insert(name.IndexOf('\''), '\''.ToString());
            }

            var sum = new BowlingSummary();
            
            sum.PlayerName = name;
            sum.Matches = Int32.Parse(args[3]);
            sum.Overs = Convert.ToInt32((Convert.ToDouble(args[2])));
            sum.Runs = Int32.Parse(args[4]);
            sum.Wickets = Int32.Parse(args[5]);
            sum.Average = average;
            sum.FiveWicketHauls = Int32.Parse(args[7]);
            sum.BestFigures = FormatBestFigures(args[8]);
            sum.Economy = economy;

            return sum;
        }

        private static BowlingSummary.BestBowlFigs FormatBestFigures(string figures)
        {
            var figs = figures.Split('-');
            return new BowlingSummary.BestBowlFigs
            {
                Wickets = Int32.Parse(figs[0]),
                Runs = Int32.Parse(figs[1])
            };
        }
    }
}