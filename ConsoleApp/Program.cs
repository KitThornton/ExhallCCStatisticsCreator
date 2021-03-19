using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Entities;

namespace ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "";
            
            var reader = new StreamReader(File.OpenRead(filePath));
            // var csvTable = new DataTable();
            List<string> list = new List<string>();
            
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                
                list.Add(line);
            }

            var newlist = list.Where(x => x.StartsWith("1")).ToList();

            List<BattingSummary> dataList = new List<BattingSummary>();
            
            foreach (var line in newlist)
            {
                //ForeignKeyConstrain
            }
        }
    }
}