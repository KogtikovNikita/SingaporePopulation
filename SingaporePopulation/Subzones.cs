using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SingaporePopulation
{
    public class Subzones
    {
        private static string directory = Chances.GetDirectory(folder: "Data");
        private const string subzonesFileName = "Subzone Population.txt";
        private static List<Subzone> Areas = GetSubzones();
        public static int TotalSubzones = Areas.Count;

        private static List<Subzone> GetSubzones ()
        {
            List<Subzone> result = new List<Subzone>();
            StreamReader sr = new StreamReader(directory + subzonesFileName);
            string line;
            while((line = sr.ReadLine())!=null)
            {
                string[] parts = line.Split('\t');
                Subzone sz = new Subzone(parts[0], Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]),
                    Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]), Convert.ToInt32(parts[5]), parts[6]);
                result.Add(sz); 
            }
            return result;
        }

        public static string GetSubzoneName (int index)
        {
            return Areas[index].Name;
        }

        public static int GetSubzoneDataPopulation (int index, int year)
        {
            switch(year)
            {
                case 2000:
                    return Areas[index].y2000;
                case 2010:
                    return Areas[index].y2010;
                case 2015:
                    return Areas[index].y2015;
                case 2019:
                    return Areas[index].y2019;
            }
            return -1;

        }

        private static int GetDwellingTotal (int index, DwellingYear year)
        {
            string name = GetSubzoneName(index);
            return Dwellings.GetTotalDwellings(name, year);
        }

        public static string GetShape (int index)
        {
            return Areas[index].Shape;
        }

        public static void WriteSubzoneStat ()
        {
            StreamWriter SW = new StreamWriter(directory + "SubzoneOUT.txt");
            string StringToWrite = "";
            //string StringToWrite = "Subzone\tValue\tShape\n";
            for (int i = 0; i < Areas.Count; i++)
                StringToWrite += (Areas[i].Name) + "\t" + (GetDwellingTotal(i, DwellingYear.y2015)) + "\t" + 
                    (Areas[i].y2015) + "\n";// + (GetDwellingTotal(i, DwellingYear.y2015) - GetDwellingTotal(i, DwellingYear.y2010)) +  '\n';
            SW.Write(StringToWrite);
            SW.Close();
        }
    }
    public class Subzone
    {
        public string Name { get; }
        public int y2000 { get; }
        public int y2005 { get; }
        public int y2010 { get; }
        public int y2015 { get; }
        public int y2019 { get; }
        public string Shape { get; }

        public Subzone(string name, int y00, int y05, int y10, int y15, int y19, string shape)
        {
            Name = name;
            y2000 = y00;
            y2005 = y05;
            y2010 = y10;
            y2015 = y15;
            y2019 = y19;
            Shape = shape;
        }
    }

}
