using System;
using System.Collections.Generic;
using System.Data;
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
            var tableName = "Batting.Summary";
            
            foreach (var data in dataList)
            {
                sb.Append(CreateInsertStatement(data, tableName));
            }

            return sb;
        }

        private static string CreateInsertStatement(BattingSummary d, string tableName)
        {
            string test =  $@"
INSERT INTO {tableName} VALUES ({d.PlayerName}, {d.Matches}, {d.Innings}, {d.Average}) 
+ {System.Environment.NewLine}";

            return test;
        }
        
        private static BattingSummary ParseData(string[] args)
        {
            decimal output;
            Decimal.TryParse(args[7], out output);
            
            BattingSummary test = new BattingSummary();

            test.PlayerName = args[1];
            test.Matches = Int32.Parse(args[2]);
            test.Innings = Int32.Parse(args[3]);
            test.NotOuts = Int32.Parse(args[4]);
            test.Runs = Int32.Parse(args[5]);
            test.HighScore = args[6];
            test.Average = output;
            test.Fifties = Int32.Parse(args[8]);
            test.Hundereds = Int32.Parse(args[9]);
            test.Catches = Int32.Parse(args[10]);
            test.Stumpings = Int32.Parse(args[11]);
            
            return test;
        }
    }
}