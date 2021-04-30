using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class BowlSummary
    {
        public static StringBuilder GenerateBowlingSummaryScript(List<string> data)
        {
            List<BowlingSummary> dataList = new List<BowlingSummary>();
            
            foreach (var line in data)
            {
                var test = line.Split(',');
                dataList.Add(BowlSummary.ParseData(test));
            }
            
            // Here we need to then create the insert statements
            return CreateInsertScript(dataList);
        }
        
        public static StringBuilder CreateInsertScript(List<BowlingSummary> dataList)
        {
            var sb = new StringBuilder();
            var statsTableName = "Summary.Bowling";
            var playersTableName = "Players.Details";
            
            foreach (var d in dataList)
            {
                sb.Append(CreatePlayerInsertStatement(d.PlayerName, playersTableName));
                sb.Append(CreateStatsInsertStatement(d, statsTableName));
            }
            
            return sb;
        }
        
        private static string CreatePlayerInsertStatement(string playerName, string tableName)
        {
            return $@"
INSERT INTO {tableName} VALUES ('{playerName}')
ON CONFLICT DO NOTHING; {Environment.NewLine}";
        }

        public static string CreateStatsInsertStatement(BowlingSummary d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Matches, Overs, Runs, Wickets, FiveWicketHauls, BestFigsRuns, BestFigsWickets, Economy, Average)
SELECT PlayerId, {d.Matches}, {d.Overs}, {d.Runs}, {d.Wickets}, {d.FiveWicketHauls}, {d.BestFigures.Runs}, {d.BestFigures.Wickets}, {d.Economy}, {d.Average}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
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

        private static BestBowlingFigures FormatBestFigures(string figures)
        {
            var figs = figures.Split('-');
            return new BestBowlingFigures
            {
                Wickets = Int32.Parse(figs[0]),
                Runs = Int32.Parse(figs[1])
            };
        }
    }
}