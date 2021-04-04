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
                     data = Tools.PrepareData(inputFilePath);
                    break;
                case "bowlingSummary":
                    inputFilePath = "./BowlingSummary.csv";
                    data = Tools.PrepareData(inputFilePath);
                    break;
                case "careerAverages":
                    inputFilePath = "./CareerAverages.csv";
                    data = Tools.PrepareDataCareer(inputFilePath);
                    break;
                default:
                    throw new Exception("invalid file type");
            }

            switch (fileType)
            {
                case "battingSummary":
                    sb = BatSummary.GenerateBattingSummaryScript(data);
                    break;
                case "bowlingSummary":
                    sb = BowlSummary.GenerateBowlingSummaryScript(data);
                    break;
                case "careerAverages":
                    sb = CareerSummary.GenerateCareerSummaryScript(data);
                    break;
                default:
                    throw new Exception("invalid file type");
            }
            
            using (StreamWriter outputFile = new StreamWriter(outputFilePath))
            {
                outputFile.WriteLine(sb.ToString());
            }
        }
    }
}