using System;
using System.Collections.Generic;
using System.IO;

namespace SingaporePopulation
{
    class Program
    {
        public static void Modelling ()
        {
            //Console.WriteLine (Dwellings.GetNumberDwellingType("Tukang", DwellingYear.y2005, DwellingType.HDB));
            //Subzones.WriteSubzoneStat();
            //Subzones.WriteSubzoneStat();
            //Testing.Rewrite();
            int InitialYear = 1990;
            int year = InitialYear;
            int FinalYear = 2019;
            Population Initial = Population.CreateInitialPopulation(InitialYear);
            Initial.PrintOverallPopulation(year);
            for (int i = year; i < FinalYear; i++)
            {
                Initial.ModelYear(i);
                Initial.PrintOverallPopulation(i+1);
            }
        }
        static void Main(string[] args)
        {

            //for (int i=20; i<100; i++)
            //{
            //    Console.WriteLine(i + " " + Chances.GetChancesToMarry(1990, Gender.male, i, Method.analytical));
            //}
            string PathToResultFile = Chances.GetDirectory(folder: "Results") + "Peoples.txt";
            StreamWriter SW = new StreamWriter(PathToResultFile);
            SW.Write("");
            SW.Close();
            Modelling();

        }
    }
}
