using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SingaporePopulation
{
    public enum DwellingYear { y2000, y2005, y2010, y2015 }
    public enum DwellingType { HDB, r2, r3, r4, r5, Condo, Landed, Other }
    public static class Dwellings
    {
        private static string directory = Chances.GetDirectory(folder: "Dwellings");
        private static readonly List<Dwelling> AllDwellings = GetDwellings();


        private static int[] ConvertArray (string[] array)
        {
            int[] result = new int[array.Length - 1];
            for (int i=1; i<array.Length; i++)
            {
                result[i - 1] = Convert.ToInt32(array[i]);
            }
            return result;
        }

        private static List<Dwelling> GetDwellings()
        {
            List<Dwelling> dwellings = new List<Dwelling>();
            List<string[]> Data = new List<string[]>();
            for (int i = 2000; i < 2020; i += 5)
            {
                StreamReader SR = new StreamReader(directory + "Dwelling" + i + ".txt", Encoding.ASCII);
                string data = SR.ReadToEnd();
                SR.Close();
                Data.Add(data.Split('\n'));
            }
            for (int i=0; i< Data[0].Length; i++)
            {
                string name = Data[0][i].Split('\t')[0];
                int[] s00 = ConvertArray(Data[0][i].Trim('\r').Split('\t'));
                int[] s05 = ConvertArray(Data[1][i].Trim('\r').Split('\t'));
                int[] s10 = ConvertArray(Data[2][i].Trim('\r').Split('\t'));
                int[] s15 = ConvertArray(Data[3][i].Trim('\r').Split('\t'));
                dwellings.Add(new Dwelling(name, s00, s05, s10, s15));
            }
            return dwellings;
        }

        private static Dwelling GetDwellingUsingName(string name)
        {
            return Dwellings.AllDwellings.Find(x => x.Name == name);
        }

        private static int[] GetDwelling(string name, DwellingYear year)
        {
            return year switch
            {
                DwellingYear.y2000 => GetDwellingUsingName(name).Dwellings2000,
                DwellingYear.y2005 => GetDwellingUsingName(name).Dwellings2005,
                DwellingYear.y2010 => GetDwellingUsingName(name).Dwellings2010,
                DwellingYear.y2015 => GetDwellingUsingName(name).Dwellings2015,
                _ => new int[0],
            };
        }

        public static int GetNumberDwellingType (string name, DwellingYear year, DwellingType type)
        {
            int[] Dwelling = GetDwelling(name, year);
            return type switch
            {
                DwellingType.HDB => Dwelling[0],
                DwellingType.r2 => Dwelling[1],
                DwellingType.r3 => Dwelling[2],
                DwellingType.r4 => Dwelling[3],
                DwellingType.r5 => Dwelling[4],
                DwellingType.Condo => Dwelling[5],
                DwellingType.Landed => Dwelling[6],
                DwellingType.Other => Dwelling[7],
                _ => -1,
            };
        }

        public static int GetTotalDwellings (string name, DwellingYear year)
        {
            int[] result = GetDwelling(name, year);
            return result[0] + result[5] + result[6] + result[7];
        }
    }


    class Dwelling
    {
        public string Name { get; }
        public int[] Dwellings2000 { get; } = new int[8];
        public int[] Dwellings2005 { get; } = new int[8];
        public int[] Dwellings2010 { get; } = new int[8];
        public int[] Dwellings2015 { get; } = new int[8];

        public Dwelling (string name, int[] D00, int[] D05, int[] D10, int[] D15)
        {
            Name = name;
            Dwellings2000 = D00;
            Dwellings2005 = D05;
            Dwellings2010 = D10;
            Dwellings2015 = D15;
        }
    }
}
