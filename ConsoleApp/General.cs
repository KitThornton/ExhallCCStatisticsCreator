using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExhallCCStats.Entities;

namespace ExhallCCStats
{
    public class General
    {
        public void CreateScripts()
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

/*
    So, let's lay out a project structure that we want:
    
    1. Read data from csv file
    
    2. Parse the data from CSV to class
    
    3. For each of these entries, create an insert w/ check statement
    
    Also, will the csv files be in the correct format we want (single source of data?)
*/

