using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class CareerSummary
    {
        public static StringBuilder GenerateCareerSummaryScript(List<string> data)
        {
            List<BattingSeason> battingSeasons = new List<BattingSeason>();
            List<BowlingSeason> bowlingSeasons = new List<BowlingSeason>();
            List<FieldingSeason> fieldingSeasons = new List<FieldingSeason>();
            string playerName = String.Empty;
            string year = String.Empty;
            List<string> playerNames = new List<string>();
            
            foreach (var line in data)
            {
                var test = line.Split(',');
                
                if (Tools.CheckNamePresent(test[0]))
                {
                    playerName = Tools.FormatPlayerName(test[0]);
                    playerNames.Add(playerName);
                    continue;
                }

                year = test[0];
                
                // Todo remove rows that are all null
                var length = test.Length;
                var r = test.Skip(1).Take(length).Where(x => (!string.IsNullOrEmpty(x) &&
                                                               !x.Equals("#DIV/0!"))).ToList();
                if (!r.Any()) continue;
                
                var bat = test.Take(9).ToList();
                var field = test.Skip(9).Take(2).ToList();
                var bowl = test.Skip(12).Take(7).Select(x => x).ToList();
                
                // Separate line into bowling and batting
                battingSeasons.Add(ParseBattingData(year, playerName, bat));
                fieldingSeasons.Add(ParseFieldingData(year, playerName, field));
                bowlingSeasons.Add(ParseBowlingData(year, playerName, bowl));
            }
            
            return CreateInsertScript(playerNames, battingSeasons, bowlingSeasons, fieldingSeasons);
        }
        
        public static BattingSeason ParseBattingData(string year, string name, List<string> args)
        {
            var average = Tools.FormatAverage(args[6]);

            return new BattingSeason
            {
                Year = year,
                PlayerName = name,
                Matches = Convert.ToInt32(args[1]),
                Innings = Convert.ToInt32(args[2]),
                NotOuts = Convert.ToInt32(args[3]),
                Runs = Convert.ToInt32(args[4]),
                HighScore = Tools.FormatHighScore(args[5]),
                Average = average,
                Fifties = Convert.ToInt32(args[7]),
                Hundreds = Convert.ToInt32(args[8])
            };
        }
        
        public static BowlingSeason ParseBowlingData(string year, string name, List<string> args)
        {
            // Just set these to 0?
            var average = Tools.FormatAverage(args[4]);
            Int32.TryParse(args[5], out var fiveWh);
            
            return new BowlingSeason
            {
                Year = year,
                PlayerName = name,
                Maidens = Convert.ToInt32(args[1]),
                Overs = Convert.ToInt32((Convert.ToDouble(args[0]))),
                Runs = Convert.ToInt32(args[2]),
                Wickets = Convert.ToInt32(args[3]),
                Average = average,
                FiveWicketHauls = fiveWh,
                BestFigures = Tools.FormatBestFigures(args[6]),
            };
        }

        private static FieldingSeason ParseFieldingData(string year, string name, List<string> args)
        {
            Int32.TryParse(args[0], out var catches);
            Int32.TryParse(args[1], out var stumpings);
            
                
            // Simple stuff
            return new FieldingSeason
            {
                Year = year,
                PlayerName = name, 
                Catches = catches, 
                Stumpings = stumpings
            };
        }
        
        // TODO
        public static StringBuilder CreateInsertScript(List<string> playerNames, List<BattingSeason> battingSeasons, 
            List<BowlingSeason> bowlingSeasons, List<FieldingSeason> fieldingSeasons)
        {
            var sb = new StringBuilder();
            var playerDetailsTable = "Players.Details";
            var battingTable = "players.batting";
            var bowlingTable = "players.Bowling";
            var fieldingTable = "players.fielding";
            
            playerNames.ForEach(x => sb.Append(Tools.CreatePlayerInsertStatement(x, playerDetailsTable)));
            battingSeasons.ForEach(x => sb.Append(CreateBattingScript(x, battingTable)));
            bowlingSeasons.ForEach(x => sb.Append(CreateBowlingScript(x, bowlingTable)));
            fieldingSeasons.ForEach(x => sb.Append(CreateFieldingScript(x, fieldingTable)));
            
            return sb;
        }
        
        private static string CreateBattingScript(BattingSeason d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Matches, Innings, NotOuts, Average, Runs, Fifties, Hundreds, HighScore, HighScoreNotOut)
SELECT PlayerId, {Tools.FormatYear(d.Year)}, {d.Matches}, {d.Innings}, {d.NotOuts}, {Tools.FormatAverage(d.Average)}, {d.Runs}, {d.Fifties}, {d.Hundreds}, {Tools.FormatHighScore(d.HighScore)}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        private static string CreateBowlingScript(BowlingSeason d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Overs, Maidens, Wickets, Runs, Average, FiveWicketHauls, BestFigsRuns, BestFigsWickets)
SELECT PlayerId, {Tools.FormatYear(d.Year)}, {d.Overs}, {d.Maidens}, {d.Wickets}, {d.Runs}, {Tools.FormatAverage(d.Average)}, {d.FiveWicketHauls}, {Tools.FormatBestFigsString(d.BestFigures)}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }

        private static string CreateFieldingScript(FieldingSeason d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Catches, Stumpings)
SELECT PlayerId, {Tools.FormatYear(d.Year)}, {d.Catches}, {d.Stumpings}
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }
    }
}