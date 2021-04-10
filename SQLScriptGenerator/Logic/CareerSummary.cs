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
                var bowl = test.Skip(12).Take(7).Select(x => x).ToList();
                
                // Separate line into bowling and batting
                battingSeasons.Add(ParseBattingData(year, playerName, bat));
                bowlingSeasons.Add(ParseBowlingData(year, playerName, bowl));
            }
            
            return CreateInsertScript(playerNames, battingSeasons, bowlingSeasons);
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
        
        // TODO
        public static StringBuilder CreateInsertScript(List<string> playerNames, List<BattingSeason> battingSeasons, 
            List<BowlingSeason> bowlingSeasons)
        {
            var sb = new StringBuilder();
            var playerDetailsTable = "Players.Details";
            var battingTable = "Summary.BattingSeason";
            var bowlingTable = "Summary.BowlingSeason";
            
            playerNames.ForEach(x => sb.Append(Tools.CreatePlayerInsertStatement(x, playerDetailsTable)));
            battingSeasons.ForEach(x => sb.Append(CreateBattingScript(x, battingTable)));
            bowlingSeasons.ForEach(x => sb.Append(CreateBowlingScript(x, bowlingTable)));
            
            return sb;
        }
        
        private static string CreateBattingScript(BattingSeason d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Matches, Innings, NotOuts, Runs, Fifties, Hundreds, HighScore, HighScoreNotOut)
SELECT (PlayerId, {Tools.FormatYear(d.Year)}, {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {d.Fifties}, {d.Hundreds}, {Tools.FormatHighScore(d.HighScore)})
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        private static string CreateBowlingScript(BowlingSeason d, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Overs, Maidens, Wickets, Runs, Average, FiveWicketHauls, BestFigsRuns, BestFigsWickets)
SELECT (PlayerId, {Tools.FormatYear(d.Year)}, {d.Overs}, {d.Maidens}, {d.Wickets}, {d.Runs}, {Tools.FormatAverage(d.Average)}, {d.FiveWicketHauls}, {Tools.FormatBestFigsString(d.BestFigures)})
FROM Players.Details
WHERE PlayerName = '{d.PlayerName}'; {Environment.NewLine}";
        }
    }
}