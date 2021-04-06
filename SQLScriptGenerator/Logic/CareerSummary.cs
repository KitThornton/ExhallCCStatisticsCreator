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
            
            foreach (var line in data)
            {
                var test = line.Split(',');
                
                if (Tools.CheckNamePresent(test[0]))
                {
                    playerName = Tools.FormatPlayerName(test[0]);
                    
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
            
            return CreateInsertScript(battingSeasons, bowlingSeasons);
        }
        
        // TODO
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
            
            // string.IsNullOrEmpty(args[5]) ? (int?) null : Convert.ToInt32(args[5]),
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
        public static StringBuilder CreateInsertScript(List<BattingSeason> battingSeasons, 
            List<BowlingSeason> bowlingSeasons)
        {
            var sb = new StringBuilder();
            var battingTable = "Summary.BattingSeason";
            var bowlingTable = "Summary.BowlingSeason";

            foreach (var item in battingSeasons)
            {
                sb.Append(CreateBattingScript(battingTable, item));
            }

            foreach (var item in bowlingSeasons)
            {
                sb.Append(CreateBowlingScript(bowlingTable, item));
            }
            
            return sb;
        }

        public static string CreateBattingScript(string tableName, BattingSeason d)
        {
            return $@"
INSERT INTO {tableName} (PlayerId, Year, Matches, Innings, NotOuts, Runs, HighScore, HighScoreNotOut, Fifties, Hundreds)
SELECT ""PlayerId"", {d.Year}, {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {d.HighScore.Runs}, {d.HighScore.NotOut}, 
{d.Fifties}, {d.Hundreds})
FROM ""Players"".""Details""
WHERE ""PlayerName"" = '{d.PlayerName}'; {Environment.NewLine}";
        }
        
        public static string CreateBowlingScript(string tableName, BowlingSeason bowlingSeason)
        {
            return string.Empty;
        }
    }
}