using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
            return list.Where(x => !string.IsNullOrEmpty(x)).ToList();
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
            
            var enumerable1 = numbers.ToList();
            enumerable1.AddRange(new List<string>(){"Year", "Total"});
            
            if (enumerable1.Contains(data)) return false;
            
            return true;
        }
    }
}