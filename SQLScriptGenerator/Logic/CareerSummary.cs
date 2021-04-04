using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class CareerSummary
    {
        public static BattingSeason ParseBattingData(string name, List<string> args)
        {
            Decimal.TryParse(args[7], out var average);
            
            if (name.Contains('\''))
            {
                name = args[1].Insert(name.IndexOf('\''), '\''.ToString());
            }

            return new BattingSeason
            {
                PlayerName = name,
                Matches = Int32.Parse(args[2]),
                Innings = Int32.Parse(args[3]),
                NotOuts = Int32.Parse(args[4]),
                Runs = Int32.Parse(args[5]),
                HighScore = args[6],
                Average = average,
                Fifties = Int32.Parse(args[8]),
                Hundreds = Int32.Parse(args[9])
            };
        }
        
        public static BowlingSeason ParseBowlingData(string name, List<string> args)
        {
            Decimal.TryParse(args[7], out var average);
            
            if (name.Contains('\''))
            {
                name = args[1].Insert(name.IndexOf('\''), '\''.ToString());
            }

            return new BowlingSeason
            {
                PlayerName = name,
                Matches = Int32.Parse(args[2]),
                Overs = Int32.Parse(args[3]),
                Runs = Int32.Parse(args[5]),
                Wickets = Int32.Parse(args[5]),
                Average = average,
                BestFigures = FormatBestFigures(args[8]),
            };
        }
        
        private static BestBowlFigs FormatBestFigures(string figures)
        {
            var figs = figures.Split('-');
            return new BestBowlFigs
            {
                Wickets = Int32.Parse(figs[0]),
                Runs = Int32.Parse(figs[1])
            };
        }

        public static StringBuilder CreateInsertScript(List<BattingSeason> battingSeasons, 
            List<BowlingSeason> bowlingSeasons)
        {

            return new StringBuilder();
        }
    }
}