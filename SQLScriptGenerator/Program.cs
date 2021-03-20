using System;
using System.Collections.Generic;
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
            string inputFilePath = "./BattingSummary.csv";
            string outputFilePath = "./SQLScript.txt";
            
            var sb = GenerateBattingSummaryScript(inputFilePath, outputFilePath);
            
            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.WriteLine(sb.ToString());
            }
        }
        
        private static StringBuilder GenerateBattingSummaryScript(string inputFilePath, string outputFilePath)
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
            list = list.Where(x => numbers.Contains(x[0].ToString())).ToList();
            List<BattingSummary> dataList = new List<BattingSummary>();
            
            foreach (var line in list)
            {
                var test = line.Split(',');
                dataList.Add(BatSummary.ParseData(test));
            }
            
            // Here we need to then create the insert statements
            return BatSummary.CreateInsertScript(dataList);
        }
    }
}