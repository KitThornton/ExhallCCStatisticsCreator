using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Entities;
using SQLScriptGenerator.Logic;

namespace SQLScriptGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            string fileType = "battingSummary";
            string inputFilePath;
            string outputFilePath = "./SQLScript.txt";
            StringBuilder sb = new StringBuilder();
            List<string> data = new List<string>();
            
            switch (fileType)
            {
                case "battingSummary":
                     inputFilePath = "./BattingSummary.csv";
                     data = PrepareData(inputFilePath);
                    break;
                case "bowlingSummary":
                    inputFilePath = "./BowlingSummary.csv";
                    data = PrepareData(inputFilePath);
                    break;
                case "careerAverages":
                    inputFilePath = "./CareerAverages.csv";
                    data = PrepareDataCareer(inputFilePath);
                    break;
                default:
                    throw new Exception("invalid file type");
            }

            switch (fileType)
            {
                case "battingSummary":
                    sb = GenerateBattingSummaryScript(data);
                    break;
                case "bowlingSummary":
                    sb = GenerateBowlingSummaryScript(data);
                    break;
                case "careerAverages":
                    sb = GenerateCareerSummaryScript(data);
                    break;
                default:
                    throw new Exception("invalid file type");
            }
            
            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.WriteLine(sb.ToString());
            }
        }

        private static List<string> PrepareData(string inputFilePath)
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

        private static List<string> PrepareDataCareer(string inputFilePath)
        {
            var reader = new StreamReader(File.OpenRead(inputFilePath));
            List<string> list = new List<string>();
            
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                list.Add(line);
            }
            
            // Filter out bad rows
            return list.Where(x => !string.IsNullOrEmpty(x)).ToList();
            
        }
        
        private static StringBuilder GenerateBattingSummaryScript(List<string> data)
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

        private static StringBuilder GenerateBowlingSummaryScript(List<string> data)
        {
            List<BowlingSummary> dataList = new List<BowlingSummary>();
            
            foreach (var line in data)
            {
                var test = line.Split(',');
                dataList.Add(BowlSummary.ParseData(test));
            }
            
            // Here we need to then create the insert statements
            return BowlSummary.CreateInsertScript(dataList);
        }

        private static StringBuilder GenerateCareerSummaryScript(List<string> data)
        {
            List<BattingSeason> battingSeasons = new List<BattingSeason>();
            List<BowlingSeason> bowlingSeasons = new List<BowlingSeason>();
            string playerName = String.Empty;
            
            foreach (var line in data)
            {
                var test = line.Split(',');

                if (CheckNamePresent(test[0])) playerName = line[0].ToString();
                
                // Isolate batting and bowling data
                var bat = test.Take(9).ToList();
                var bowl = test.Take(1).Skip(9).Take(7).ToList();
                
                // Separate line into bowling and batting
                battingSeasons.Add(CareerSummary.ParseBattingData(playerName, bat));
                bowlingSeasons.Add(CareerSummary.ParseBowlingData(playerName, bowl));
            }
            
            return CareerSummary.CreateInsertScript(battingSeasons, bowlingSeasons);

        }

        private static Boolean CheckNamePresent(string data)
        {
            // If data is not "year", "Total" or starts with a numbers, that's the name
            var numbers = Enumerable
                .Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            
            var enumerable1 = numbers.ToList();
            enumerable1.AddRange(new List<string>(){"Year", "Total"});
            
            if (enumerable1.Contains(data)) return false;
            
            return true;
        }
    }
}