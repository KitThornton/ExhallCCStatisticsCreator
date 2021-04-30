using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLScriptGenerator.Logic;

namespace SQLScriptGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            string fileType = "careerAverages";
            string inputFilePath;
            string outputFilePath = "./SQLScript.txt";
            StringBuilder sb = new StringBuilder();
            List<string> data = new List<string>();
            
            switch (fileType)
            {
                case "battingSummary":
                     inputFilePath = "./BattingSummary.csv";
                     data = Tools.PrepareData(inputFilePath);
                     sb = BatSummary.GenerateBattingSummaryScript(data);
                    break;
                case "bowlingSummary":
                    inputFilePath = "./BowlingSummary.csv";
                    data = Tools.PrepareData(inputFilePath);
                    sb = BowlSummary.GenerateBowlingSummaryScript(data);
                    break;
                case "careerAverages":
                    inputFilePath = "./CareerAverages.csv";
                    data = Tools.PrepareDataCareer(inputFilePath);
                    sb = CareerSummary.GenerateCareerSummaryScript(data);
                    break;
                case "awards":
                    inputFilePath = "./awards";
                    data = Tools.PrepareAwardsData(inputFilePath);
                    // sb = 
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