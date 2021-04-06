using System;
using System.Collections.Generic;
using System.Linq;
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
            return BatSummary.CreateInsertScript(dataList);
        }
        
        public static StringBuilder CreateInsertScript(List<BattingSummary> dataList)
        {
            var sb = new StringBuilder();
            var battingTableName = "\"Summary\".\"Batting\"";
            var playerTableName = "\"Players\".\"Details\"";
            var fieldingTableName = "\"Summary\".\"Fielding\"";

            foreach (var d in dataList)
            {
                sb.Append(CreatePlayerInsertStatement(d.PlayerName, playerTableName));
                sb.Append(CreateStatsInsertStatement(battingTableName, d));
                sb.Append(CreateFieldingStatsInsertStatement(fieldingTableName, d));
            }

            return sb;
        }

        public static string CreateStatsInsertStatement(string tableName, BattingSummary d)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Matches, innings, notouts, runs, highscore, average, fifties, hundreds, 
highscorenotout)
SELECT ""PlayerId"", {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {d.HighScore.Runs}, {d.Average}, {d.Fifties}, 
{d.Hundreds}, CAST({Convert.ToInt16(d.HighScore.NotOut)} AS BIT)
FROM ""Players"".""Details""
WHERE ""PlayerName"" = '{d.PlayerName}'; {Environment.NewLine}";}
        
        private static string CreatePlayerInsertStatement(string playerName, string tableName)
        {
            return $@"
INSERT INTO {tableName} (""PlayerName"") VALUES ('{playerName}')
ON CONFLICT DO NOTHING; {Environment.NewLine}";
        }
        
        public static string CreateFieldingStatsInsertStatement(string tableName, BattingSummary d)
        {
            return $@"
INSERT INTO {tableName} (""PlayerId"", ""Catches"", ""Stumpings"")
SELECT ""PlayerId"", {d.Catches}, {d.Stumpings}
FROM ""Players"".""Details""
WHERE ""PlayerName"" = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        public static BattingSummary ParseData(string[] args)
        {
            Decimal.TryParse(args[7], out var average);

            var name = args[1];
            if (name.Contains('\''))
            {
                name = args[1].Insert(name.IndexOf('\''), '\''.ToString());
            }
            
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