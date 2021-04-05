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
            if (average.Equals("#DIV/0!"))
            {
                av = null;
            }
            else
            {
                av = Decimal.Parse(average);    
            }

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
    }
}