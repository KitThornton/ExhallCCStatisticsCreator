using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class BatSummary
    {
        public static StringBuilder CreateInsertScript(List<BattingSummary> dataList)
        {
            var sb = new StringBuilder();
            var battingTableName = "\"Summary\".\"Batting\"";
            var playerTableName = "\"Players\".\"Details\"";
            var fieldingTableName = "\"Summary\".\"Fielding\"";

            foreach (var d in dataList)
            {
                // sb.Append(CreatePlayerInsertStatement(d.PlayerName, playerTableName));
                // sb.Append(CreateStatsInsertStatement(battingTableName, d, FormatHighScore(d.HighScore)));
                sb.Append(CreateFieldingStatsInsertStatement(fieldingTableName, d));
            }

            return sb;
        }

        public static string CreateStatsInsertStatement(string tableName, BattingSummary d, Tuple<int, int> hs)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Matches, innings, notouts, runs, highscore, average, fifties, hundreds, 
highscorenotout)
SELECT ""PlayerId"", {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {hs.Item1}, {d.Average}, {d.Fifties}, 
{d.Hundreds}, CAST({hs.Item2} AS BIT)
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
                HighScore = args[6],
                Average = average,
                Fifties = Int32.Parse(args[8]),
                Hundreds = Int32.Parse(args[9]),
                Catches = Int32.Parse(args[10]),
                Stumpings = Int32.Parse(args[11])
            };
        }

        private static Tuple<int, int> FormatHighScore(string highScore)
        {
            var notOut = highScore.Contains('*') ? 1 : 0;
            var score = Int32.Parse(highScore.Replace('*', ' '));

            return new Tuple<int, int>(score, notOut);
        }
    }
}