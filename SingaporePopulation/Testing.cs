using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SingaporePopulation
{
    public struct SubzonePop
    {
        public string Name;
        public int Population;

        public SubzonePop (string name, int population)
        {
            Name = name;
            Population = population;
        }
    }
    public class Testing
    {
        private static string directory =  Chances.GetDirectory(folder: "SubzonesData");
        private static List<string> GetParts (string source)
        {
            List<string> result = new List<string>();
            while (source.Contains("<td>") && source.Contains("</td>"))
            {
                int start = source.IndexOf("<td>") + "<td>".Length;
                int finish = source.IndexOf("</td>", start);
                result.Add(source.Substring(start, finish - start));
                source = source.Substring(finish);
            }
            return result;
                
        }
        
        public static void Rewrite()
        {
            StreamReader SR = new StreamReader(directory + "2000.txt");
            string line = SR.ReadToEnd();
            string[] parts = line.Split("<td>Subzone</td>");
            List<SubzonePop> SP = new List<SubzonePop>();
            string Out = "";
            for (int i = 1; i < parts.Length; i++)
            {
                List<string> Popul = GetParts(parts[i]);
                //SubzonePop SZP = new SubzonePop(Popul[0], Convert.ToInt32(Popul[2]));
                //SP.Add(SZP);
                Out += Popul[0] + '\t' + Popul[6] + '\t' + Popul[8] + '\t' + Popul[10] + '\t' + Popul[12] + '\t' +
                    Popul[14] + '\t' + Popul[16] + '\t' + Popul[18] + '\t' + Popul[20] + '\n';
            }
            StreamWriter SW = new StreamWriter(directory + "2000OUT.txt");
            SW.Write(Out);
            SW.Close();
        }
        

    }
}
