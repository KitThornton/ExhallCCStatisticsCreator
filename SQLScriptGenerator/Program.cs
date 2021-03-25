using System;
using System.Collections.Generic;
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
            
            switch (fileType)
            {
                case "battingSummary":
                     inputFilePath = "./BattingSummary.csv";
                    break;
                case "bowlingSummary":
                    inputFilePath = "./BowlingSummary.csv";
                    break;
                default:
                    throw new Exception("invalid file type");
            }
            
            var data = PrepareData(inputFilePath);
            
            switch (fileType)
            {
                case "battingSummary":
                    sb = GenerateBattingSummaryScript(data);
                    break;
                case "bowlingSummary":
                    sb = GenerateBowlingSummaryScript(data);
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
    }
}