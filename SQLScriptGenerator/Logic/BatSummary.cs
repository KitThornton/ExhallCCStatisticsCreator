using System;
using System.Collections.Generic;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class BatSummary
    {
        public static StringBuilder GenerateBattingSummaryScript(List<string> data)
        {
            List<BattingSummary> dataList = new List<BattingSummary>();
            
            foreach (var line in data)
            {
                var test = line.Split(',');
                dataList.Add(BatSummary.ParseData(test));
            }
            
            // Here we need to then create the insert statements
            return CreateInsertScript(dataList);
        }
        
        public static StringBuilder CreateInsertScript(List<BattingSummary> dataList)
        {
            var sb = new StringBuilder();
            var battingTableName = "Summary.Batting";
            var playerTableName = "Players.Details";
            var fieldingTableName = "Summary.Fielding";

            foreach (var d in dataList)
            {
                sb.Append(Tools.CreatePlayerInsertStatement(d.PlayerName, playerTableName));
                sb.Append(CreateStatsInsertStatement(battingTableName, d));
                sb.Append(CreateFieldingStatsInsertStatement(fieldingTableName, d));
            }

            return sb;
        }

        public static string CreateStatsInsertStatement(string tableName, BattingSummary d)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Matches, innings, notouts, runs, average, fifties, hundreds, highscore, highscorenotout)
SELECT PlayerId, {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {d.Average}, {d.Fifties}, {d.Hundreds}, {Tools.FormatHighScore(d.HighScore)}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";}

        public static string CreateFieldingStatsInsertStatement(string tableName, BattingSummary d)
        {
            return $@"
INSERT INTO {tableName} (""PlayerId"", ""Catches"", ""Stumpings"")
SELECT PlayerId, {d.Catches}, {d.Stumpings}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        public static BattingSummary ParseData(string[] args)
        {
            Decimal.TryParse(args[7], out var average);
            
            var name = Tools.FormatPlayerName(args[1]);
            
            return new BattingSummary
            {
                PlayerName = name,
                Matches = Int32.Parse(args[2]),
                Innings = Int32.Parse(args[3]),
                NotOuts = Int32.Parse(args[4]),
                Runs = Int32.Parse(args[5]),
                HighScore = Tools.FormatHighScore(args[6]),
                Average = average,
                Fifties = Int32.Parse(args[8]),
                Hundreds = Int32.Parse(args[9]),
                Catches = Int32.Parse(args[10]),
                Stumpings = Int32.Parse(args[11])
            };
        }
    }
}