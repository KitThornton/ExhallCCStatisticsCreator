using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Entities;

namespace SQLScriptGenerator.Logic
{
    public class Tools
    {
        public static List<string> PrepareDataCareer(string inputFilePath)
        {
            var reader = new StreamReader(File.OpenRead(inputFilePath));
            List<string> list = new List<string>();
            
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                list.Add(line);
            }
            
            // Filter out bad rows
            return list.Where(x => !(x.StartsWith(",") || x.StartsWith("Year") )).ToList();
        }
        
        public static List<string> PrepareData(string inputFilePath)
        {
            var reader = new StreamReader(File.OpenRead(inputFilePath));
            List<string> list = new List<string>();
            var numbers = Enumerable
                .Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));

            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                list.Add(line);
            }
            
            // Filter out bad rows
            return list.Where(x => numbers.Contains(x[0].ToString())).ToList();
        }

        public static List<string> PrepareAwardsData(string inputFilePath)
        {
            var reader = new StreamReader(File.OpenRead(inputFilePath));
            List<string> list = new List<string>();
            var numbers = Enumerable
                .Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));

            var test = numbers.Append("1ST");

            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                list.Add(line);
            }
            
            // filter out bad rows
            // return list.Where(x => test.Contains(x[0].ToString()) && test.Contains(x[1].ToString())).ToList();
            return list;
        }
        
        public static Boolean CheckNamePresent(string data)
        {
            // If data is not "year", "Total" or starts with a numbers, that's the name
            var numbers = Enumerable
                .Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            
            var defaults = new List<string>() {"Year", "Total"};

            if (defaults.Contains(data) || numbers.Contains(data.First().ToString())) return false;

            return true;
        }
        
        public static BestBowlingFigures FormatBestFigures(string figures)
        {
            if (string.IsNullOrEmpty(figures)) return new BestBowlingFigures();
            
            var figs = figures.Split('-');
            return new BestBowlingFigures
            {
                Wickets = Int32.Parse(figs[0]),
                Runs = Int32.Parse(figs[1])
            };
        }

        public static decimal? FormatAverage(string average)
        {
            decimal? av;
            av = average.Equals("#DIV/0!") ? (decimal?) null : Decimal.Parse(average);
            return av;
        }

        public static string FormatPlayerName(string name)
        {
            if (name.Contains('\''))
            {
                name = name.Insert(name.IndexOf('\''), '\''.ToString());
            }

            return name;
        }

        public static HighScore FormatHighScore(string score)
        {
            if (string.IsNullOrEmpty(score)) return new HighScore();
            
            Boolean notOut = score.Contains('*');
            var runs = Int32.Parse(score.Replace('*', ' '));

            return new HighScore{ NotOut = notOut, Runs = runs};
        }
        
        public static string CreatePlayerInsertStatement(string playerName, string tableName)
        {
            return $@"
INSERT INTO {tableName} (PlayerName) VALUES ('{playerName}')
ON CONFLICT DO NOTHING; {Environment.NewLine}";
        }

        public static string FormatBestFigsString(BestBowlingFigures bestBowling)
        {
            return bestBowling.Runs is null ? "NULL, NULL" : $@"{bestBowling.Runs}, {bestBowling.Wickets}";
        }
        
        public static string FormatHighScore(HighScore highScore)
        {
            return highScore.Runs is null ? "NULL, NULL" : $@"{highScore.Runs}, CAST({Convert.ToInt16(highScore.NotOut)} AS BIT)";
        }
        
        public static string FormatAverage(decimal? average)
        {
            return average is null ? "NULL" : $@"{average}";
        }

        public static string FormatYear(string year)
        {
            return year.Equals("total", StringComparison.InvariantCultureIgnoreCase) ? "NULL" : $"{year}";
        }
    }
}