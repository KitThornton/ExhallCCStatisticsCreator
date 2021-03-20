using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Entities;

namespace ConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "./BattingSummary.csv";
            string outputFilePath = "./SQLScript.txt";
            
            var reader = new StreamReader(File.OpenRead(filePath));
            
            var numbers = Enumerable
                .Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            
            List<string> list = new List<string>();
            
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                
                list.Add(line);
            }
            
            list = list.Where(x => numbers.Contains(x[0].ToString())).ToList();
            
            List<BattingSummary> dataList = new List<BattingSummary>();
            
            foreach (var line in list)
            {
                var test = line.Split(',');
                dataList.Add(ParseData(test));
            }
            
            // Here we need to then create the insert statements
            var sb = CreateInsertScript(dataList);
            
            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.WriteLine(sb.ToString());
            }
        }
        
        private static StringBuilder CreateInsertScript(List<BattingSummary> dataList)
        {
            var sb = new StringBuilder();
            var statsTableName = "\"Summary\".\"Batting\"";
            var playerTableName = "\"Players\".\"Details\"";
            var count = 1;
            
            foreach (var d in dataList)
            {
                sb.Append(CreatePlayerInsertStatement(count, d.PlayerName, playerTableName));
                sb.Append(CreateStatsInsertStatement(count, d, statsTableName, FormatHighScore(d.HighScore)));
                count += 1;
            }
            
            return sb;
        }
        
        private static string CreateStatsInsertStatement(int playerId, BattingSummary d, string tableName, 
            Tuple<int, int> hs)
        {
            return $@"
INSERT INTO {tableName} VALUES ({playerId}, {d.Matches}, {d.Innings}, {d.NotOuts}, {d.Runs}, {hs.Item1}, 
{d.Average}, {d.Fifties}, {d.Hundreds}, CAST({hs.Item2} AS BIT)); {Environment.NewLine}";
        }
        
        private static string CreatePlayerInsertStatement(int playerId, string playerName, string tableName)
        {
            return $@"
INSERT INTO {tableName} VALUES ({playerId}, '{playerName}'); {Environment.NewLine}";
        }
        
        private static BattingSummary ParseData(string[] args)
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