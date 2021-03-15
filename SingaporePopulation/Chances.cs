using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;

namespace SingaporePopulation
{
    public enum Method { statistical, analytical }

    public struct GenderAge
    {
        public Gender gender;
        public int age;

        public GenderAge(Gender g, int a)
        {
            gender = g;
            age = a;
        }

        public int ToInt()
        {
            return age * 10 + (gender == Gender.male ? 1 : 0);
        }
    }

    public struct GenderYearAge
    {
        public Gender gender;
        public int year;
        public int age;

        public GenderYearAge(Gender Gender, int Year, int Age)
        {
            gender = Gender;
            year = Year;
            age = Age;
        }

        public static bool operator ==(GenderYearAge a1, GenderYearAge a2)
        {
            if ((a1.gender == a2.gender) && (a1.age == a2.age) && (a1.year == a2.year))
                return true;
            else
                return false;
        }

        public static bool operator !=(GenderYearAge a1, GenderYearAge a2)
        {
            if ((a1.gender != a2.gender) || (a1.age != a2.age) || (a1.year != a2.year))
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return gender.ToString() + " " + year + " " + age;
        }

        public int ToInt()
        {
            int result = 0;
            if (gender == Gender.male)
                result += 1;
            result += (year - 1960) * 10;
            result += age * 10000;
            return result;
        }

        
    }

    public class YearAge
    {
        public int year;
        public int age;

        public YearAge(int Year, int Age)
        {
            year = Year;
            age = Age;
        }

        public static bool operator ==(YearAge ya1, YearAge ya2)
        {
            if ((ya1.age == ya2.age) && (ya1.year == ya2.year))
                return true;
            else
                return false;

        }
        public static bool operator !=(YearAge ya1, YearAge ya2)
        {
            if ((ya1.age != ya2.age) || (ya1.year != ya2.year))
                return true;
            else
                return false;

        }

        public int ToInt()
        {
            int result = 0;
            result += year - 1960;
            result += age * 1000;
            return result;
        }

    }
    public static class Chances
    {
        public static string directory = GetDirectory();
        private static readonly Dictionary<YearAge, double> ChancesToBorn = GetChancesToBorn();
        private static readonly Dictionary<GenderYearAge, double> ChancesToMarryD = GetChancesToMarryFile();
        private static readonly Dictionary<GenderYearAge, double> ChancesToDivorceD = GetChancesToDivorceFile();

        

        private static readonly Dictionary<GenderYearAge, double> ChancesToDie = GetChancesToDie();
        private static readonly Dictionary<GenderYearAge, double> NumberOfPeople = ReadGendersAge();
        private static readonly Dictionary<int, int> NonREsidentalPopulaiton = GetNonResidentalPopulation();
        private static readonly List<double> ChancesToDieList = GetChancesToDieList();
        private static readonly List<double> ChancesToBornList = GetChancesToBornList();
        private static Dictionary<GenderYearAge, int> SinglesD { get; } = GetRelationships("Singles.txt");
        private static Dictionary<GenderYearAge, int> MarriedD { get; } = GetRelationships("Married.txt");
        private static Dictionary<GenderYearAge, int> DivorcedD { get; } = GetRelationships("Divorced.txt");
        private static Dictionary<GenderYearAge, int> WidowedD { get; } = GetRelationships("Widowed.txt");

        public static List<int> Singles { get; } = GetList(SinglesD);

        public static List<int> Married { get; } = GetList(MarriedD);
        public static List<int> Divorced { get; } = GetList(DivorcedD);
        public static List<int> Widowed { get; } = GetList(WidowedD);
        public static string GetDirectory (string folder = "Data")
        {
            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.IndexOf(@"\bin")) + @"\" + folder + @"\";
            return dir;
        }
        public static double GetChancesToDivorce(Gender gender, int age, int year, Method method = Method.analytical)
        {
            if (method == Method.statistical)
            {
                try
                {
                    return ChancesToDivorceD[new GenderYearAge(gender, year, age)];
                }
                catch
                {
                    return 0;
                }
            }
            if (method == Method.analytical)
            {
                int a = year - 1989;
                if (gender == Gender.male && age >= 20 && age <= 24)
                    return (18.261 * Math.Exp(0.0214 * a)) / 1000;
                if (gender == Gender.male && age >= 25 && age <= 29)
                    return (13.32 * Math.Exp(0.0146 * a)) / 1000;
                if (gender == Gender.male && age >= 30 && age <= 34)
                    return (10.217 * Math.Exp(0.0128 * a)) / 1000;
                if (gender == Gender.male && age >= 35 && age <= 39)
                    return (7.6158 * Math.Exp(0.0194 * a)) / 1000;
                if (gender == Gender.male && age >= 40 && age <= 44)
                    return (5.8175 * Math.Exp(0.0205 * a)) / 1000;
                if (gender == Gender.male && age >= 45 && age <= 49)
                    return (4.3679 * Math.Exp(0.024 * a)) / 1000;
                if (gender == Gender.male && age >= 50)
                    return (1.8557 * Math.Exp(0.0269 * a)) / 1000;
                if (gender == Gender.female && age >= 20 && age <= 24)
                    return (16.262 * Math.Exp(0.0225 * a)) / 1000;
                if (gender == Gender.female && age >= 25 && age <= 29)
                    return (12.151 * Math.Exp(0.0119 * a)) / 1000;
                if (gender == Gender.female && age >= 30 && age <= 34)
                    return (8.783 * Math.Exp(0.0151 * a)) / 1000;
                if (gender == Gender.female && age >= 35 && age <= 39)
                    return (6.465 * Math.Exp(0.0188 * a)) / 1000;
                if (gender == Gender.female && age >= 40 && age <= 44)
                    return (4.7679 * Math.Exp(0.0216 * a)) / 1000;
                if (gender == Gender.female && age >= 45 && age <= 49)
                    return (3.43 * Math.Exp(0.0241 * a)) / 1000;
                if (gender == Gender.female && age >= 50)
                    return (1.2092 * Math.Exp(0.0318 * a)) / 1000;

                return 0;
            }
            return 0;



        }


        private static Dictionary<GenderYearAge, double> GetChancesToDivorceFile()
        {
            Dictionary<GenderYearAge, double> result = new Dictionary<GenderYearAge, double>();
            StreamReader SR = new StreamReader(directory + "Chances to divorce.txt");
            string line;
            int linenumber = 0;
            while ((line = SR.ReadLine()) != null)
            {
                Gender currentGender;
                int startAge;
                if (linenumber < 7)
                {
                    currentGender = Gender.male;
                    startAge = linenumber * 5 + 20;
                }
                else
                {
                    currentGender = Gender.female;
                    startAge = (linenumber - 7) * 5 + 20;
                }


                string[] parts = line.Split('\t');
                if (startAge != 50)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        for (int j = 0; j < 5; j++)
                            result.Add(new GenderYearAge(currentGender, i + 1990, startAge + j), Convert.ToDouble(parts[i]) / 1000);
                    }
                }
                else
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        for (int j = 0; j < 40; j++)
                            result.Add(new GenderYearAge(currentGender, i + 1990, startAge + j), Convert.ToDouble(parts[i]) / 1000);
                    }
                }
                linenumber++;
            }
            return result;
        }

        private static Dictionary<GenderYearAge, double> GetChancesToMarryFile()
        {
            Dictionary<GenderYearAge, double> result = new Dictionary<GenderYearAge, double>();
            StreamReader SR = new StreamReader(directory + "Chances to marry.txt");
            string line;
            int linenumber = 0;
            while ((line = SR.ReadLine()) != null)
            {
                Gender currentGender;
                int startAge;
                if (linenumber < 11)
                {
                    currentGender = Gender.male;
                    startAge = linenumber * 5 + 15;
                }
                else
                {
                    currentGender = Gender.female;
                    startAge = (linenumber - 11) * 5 + 15;
                }


                string[] parts = line.Split('\t');
                if (startAge != 65)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        for (int j = 0; j < 5; j++)
                            result.Add(new GenderYearAge(currentGender, i + 1990, startAge + j), Convert.ToDouble(parts[i])/1000);
                    }
                }
                else
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        for (int j = 0; j < 40; j++)
                            result.Add(new GenderYearAge(currentGender, i + 1990, startAge + j), Convert.ToDouble(parts[i]) / 1000);
                    }
                }
                linenumber++;
            }
            return result;
        }

        private static List<int> GetList(Dictionary<GenderYearAge, int> dict)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 2000000; i++)
                list.Add(0);
            foreach (GenderYearAge item in dict.Keys)
                list[item.ToInt()] = Convert.ToInt32(Math.Round((decimal)dict[item], 0));
            return list;
        }

        private static Dictionary<GenderYearAge, int> GetRelationships(string v)
        {
            Dictionary<GenderYearAge, int> result = new Dictionary<GenderYearAge, int>();
            List<List<int>> data = ReadTable(v);
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    int year = 0;
                    if (j == 0)
                        year = j + 1980;
                    else
                        year = j + 1982;
                    Gender gender = i % 2 == 0 ? Gender.male : Gender.female;
                    int age = 20 + 10 * Convert.ToInt32(Math.Floor((decimal)i / 2));
                    for (int k = 0; k < 10; k++)
                    {
                        GenderYearAge GYA = new GenderYearAge(gender, year, age + k);
                        result.Add(GYA, Convert.ToInt32(Math.Round((decimal)data[i][j] / 10, 0)));
                    }
                }

            }

            

            for (int i = 0; i < 10; i++)
            {
                Dictionary<GenderYearAge, int> mixer = new Dictionary<GenderYearAge, int>();
                foreach (GenderYearAge key in result.Keys)
                {
                    if (key.age == 20)
                    {
                        GenderYearAge next = new GenderYearAge(key.gender, key.year, key.age + 1);
                        GenderYearAge current = new GenderYearAge(key.gender, key.year, key.age);
                        int value = Convert.ToInt32(Math.Round((double)(result[next] + result[current]) / 2.00));
                        mixer.Add(key, value);
                    }
                    else if (key.age == 79)
                    {
                        GenderYearAge previous = new GenderYearAge(key.gender, key.year, key.age - 1);
                        GenderYearAge current = new GenderYearAge(key.gender, key.year, key.age);
                        int value = Convert.ToInt32(Math.Round((double)(result[previous] + result[current]) / 2.00));
                        mixer.Add(key, value);
                    }
                    else
                    {
                        GenderYearAge next = new GenderYearAge(key.gender, key.year, key.age + 1);
                        GenderYearAge previous = new GenderYearAge(key.gender, key.year, key.age - 1);
                        GenderYearAge current = new GenderYearAge(key.gender, key.year, key.age);
                        int value = Convert.ToInt32(Math.Round((double)(result[previous] + result[current] + result[next]) / 3.00));
                        mixer.Add(key, value);
                    }
                }
                result = mixer;
            }
            List<List<int>> DataAfterMixing = new List<List<int>>();
            for (int i=0; i<data.Count; i++)
            {
                DataAfterMixing.Add(new List<int>());
                for (int j=0; j<data[i].Count; j++)
                {
                    Gender ControlGender;
                    int ControlYear;
                    int ControlMinimumAge = 0;
                    int ControlMaximumAge = 0;
                    if (i % 2 == 0)
                        ControlGender = Gender.male;
                    else
                        ControlGender = Gender.female;
                    if (j == 0)
                        ControlYear = j + 1980;
                    else
                        ControlYear = j + 1982;
                    if (i ==0 || i == 1)
                    {
                        ControlMinimumAge = 20;
                        ControlMaximumAge = 29;
                    }
                    if (i == 2 || i == 3)
                    {
                        ControlMinimumAge = 30;
                        ControlMaximumAge = 39;
                    }
                    if (i == 4 || i == 5)
                    {
                        ControlMinimumAge = 40;
                        ControlMaximumAge = 49;
                    }
                    if (i == 6 || i == 7)
                    {
                        ControlMinimumAge = 50;
                        ControlMaximumAge = 59;
                    }
                    if (i == 8 || i == 9)
                    {
                        ControlMinimumAge = 60;
                        ControlMaximumAge = 69;
                    }
                    if (i == 10 || i == 11)
                    {
                        ControlMinimumAge = 70;
                        ControlMaximumAge = 79;
                    }
                    int ControlPOpulation = 0;
                    for (int age = ControlMinimumAge; age < ControlMaximumAge + 1; age++)
                    {
                        GenderYearAge key = new GenderYearAge(ControlGender, ControlYear, age);
                        ControlPOpulation += result[key];
                    }
                    DataAfterMixing[i].Add(ControlPOpulation);
                }
            }
            for (int i=0; i<DataAfterMixing.Count; i++)
            {
                for (int j=0; j<DataAfterMixing[i].Count; j++)
                {
                    Gender ControlGender;
                    int ControlYear;
                    int ControlMinimumAge = 0;
                    int ControlMaximumAge = 0;
                    if (i % 2 == 0)
                        ControlGender = Gender.male;
                    else
                        ControlGender = Gender.female;
                    if (j == 0)
                        ControlYear = j + 1980;
                    else
                        ControlYear = j + 1982;
                    if (i == 0 || i == 1)
                    {
                        ControlMinimumAge = 20;
                        ControlMaximumAge = 29;
                    }
                    if (i == 2 || i == 3)
                    {
                        ControlMinimumAge = 30;
                        ControlMaximumAge = 39;
                    }
                    if (i == 4 || i == 5)
                    {
                        ControlMinimumAge = 40;
                        ControlMaximumAge = 49;
                    }
                    if (i == 6 || i == 7)
                    {
                        ControlMinimumAge = 50;
                        ControlMaximumAge = 59;
                    }
                    if (i == 8 || i == 9)
                    {
                        ControlMinimumAge = 60;
                        ControlMaximumAge = 69;
                    }
                    if (i == 10 || i == 11)
                    {
                        ControlMinimumAge = 70;
                        ControlMaximumAge = 79;
                    }
                    for (int age = ControlMinimumAge; age < ControlMaximumAge + 1; age++)
                    {
                        GenderYearAge key = new GenderYearAge(ControlGender, ControlYear, age);
                        int difference = data[i][j] - DataAfterMixing[i][j];
                        double multiplier = 0;
                        if (ControlMinimumAge == 20 && age < 25)
                        {
                            multiplier = (double)(5 - age % 5)/15.0;
                        }
                        else if (ControlMaximumAge == 79 && age > 74)
                        {
                            multiplier = (double)(age % 5 + 1) / 15.0;
                        }
                        else if (ControlMinimumAge!=20 && ControlMaximumAge != 79 && age % 10 < 5)
                        {
                            multiplier = (double)(5 - age % 5) / 30.0;

                        }
                        else if (ControlMinimumAge != 20 && ControlMaximumAge != 79 && age % 10 >= 5)
                        {
                            multiplier = (double)(age % 5 + 1) / 30.0;
                        }
                        result[key] +=  Convert.ToInt32(Math.Round(difference * multiplier));
                        if (key.age >= 70 && key.age <= 79)
                        {
                            double part = (double)(101 - 5 * (age - 69)) / (double)100;
                            int rest = Convert.ToInt32(result[key] * ( 1 - part));
                            result[key] = Convert.ToInt32(result[key] * part);
                            int newage = 159 - age;
                            GenderYearAge keynew = new GenderYearAge(key.gender, key.year, newage);
                            result.Add(keynew, rest);

                        }

                    }
                }
            }
            return result;
        }

        private static List<List<int>> ReadTable(string v)
        {
            List<List<int>> result = new List<List<int>>();
            StreamReader SR = new StreamReader(directory + v);
            string line;
            while ((line = SR.ReadLine()) != null)
            {
                string[] parts = line.Split('\t');
                int[] nums = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    nums[i] = Convert.ToInt32(parts[i]);
                List<int> row = new List<int>(nums);
                result.Add(row);
            }
            return result;
        }





        //Private Methods
        private static List<double> GetChancesToBornList()
        {
            List<double> list = new List<double>();
            for (int i = 0; i < 200000; i++)
                list.Add(0);
            foreach (YearAge item in ChancesToBorn.Keys)
                list[item.ToInt()] = ChancesToBorn[item];
            return list;
        }


        private static List<double> GetChancesToDieList()
        {
            List<double> list = new List<double>();
            for (int i = 0; i < 2000000; i++)
                list.Add(0);
            foreach (GenderYearAge item in ChancesToDie.Keys)
                list[item.ToInt()] = ChancesToDie[item] / 1000;
            return list;
        }


        private static Dictionary<int, int> GetNonResidentalPopulation()
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            StreamReader SR = new StreamReader(directory + "Non-residental population.txt");
            string line;
            while ((line = SR.ReadLine()) != null)
            {
                string[] parts = line.Split('\t');
                result.Add(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1].Trim('\n')));
            }
            return result;
        }
        private static Dictionary<YearAge, double> GetChancesToBorn()
        {
            Dictionary<YearAge, double> chances = new Dictionary<YearAge, double>();
            StreamReader sr = new StreamReader(directory + "Chances to born new child.txt");
            string line;
            int RowNumber = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split('\t');
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        chances.Add(new YearAge(1960 + i, 15 + RowNumber * 5 + j), Convert.ToDouble(data[i]) / 1000);
                    }

                }
                RowNumber++;
            }
            sr.Close();
            return chances;
        }

        private static Dictionary<GenderYearAge, double> ReadFile(string Name, bool BabyTogether = false, int StartYear = 1960)
        {
            Dictionary<GenderYearAge, double> chances = new Dictionary<GenderYearAge, double>();
            StreamReader sr = new StreamReader(directory + Name);
            string line;
            int RowNumber = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split('\t');
                for (int i = 0; i < data.Length; i++)
                {
                    if (RowNumber == 0)
                    {
                        if (BabyTogether)
                        {
                            for (int j = 0; j < 5; j++)
                                chances.Add(new GenderYearAge(Gender.male, StartYear + i, j), Convert.ToDouble(data[i]));

                        }
                        else
                            chances.Add(new GenderYearAge(Gender.male, StartYear + i, 0), Convert.ToDouble(data[i]));
                    }
                    else if (RowNumber == 1)
                    {
                        for (int j = 1; j < 5; j++)
                        {
                            chances.Add(new GenderYearAge(Gender.male, StartYear + i, j), Convert.ToDouble(data[i]));
                        }
                    }
                    else if (RowNumber == 19)
                    {
                        if (BabyTogether)
                        {
                            for (int j = 0; j < 5; j++)
                                chances.Add(new GenderYearAge(Gender.female, StartYear + i, j), Convert.ToDouble(data[i]));

                        }
                        chances.Add(new GenderYearAge(Gender.female, StartYear + i, 0), Convert.ToDouble(data[i]));
                    }
                    else if ((RowNumber == 20))
                    {
                        for (int j = 1; j < 5; j++)
                        {
                            chances.Add(new GenderYearAge(Gender.female, StartYear + i, j), Convert.ToDouble(data[i]));
                        }
                    }
                    else if (RowNumber < 19)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            chances.Add(new GenderYearAge(Gender.male, StartYear + i, RowNumber * 5 + j - 5), Convert.ToDouble(data[i]));
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            chances.Add(new GenderYearAge(Gender.female, StartYear + i, RowNumber * 5 + j - 20 * 5), Convert.ToDouble(data[i]));
                        }
                    }

                }
                RowNumber++;
                if ((RowNumber == 1) && (BabyTogether))
                    RowNumber++;
                if ((RowNumber == 20) && (BabyTogether))
                    RowNumber++;
            }
            sr.Close();
            return chances;
        }

        private static Dictionary<GenderYearAge, double> GetChancesToDie()
        {
            Dictionary<GenderYearAge, double> current = ReadFile("Chances to die.txt");
            return current;
        }


        private static Dictionary<GenderYearAge, double> ReadGendersAge()
        {
            Dictionary<GenderYearAge, double> males = ReadAgesFile(Gender.male);
            Dictionary<GenderYearAge, double> females = ReadAgesFile(Gender.female);
            Dictionary<GenderYearAge, double> people = males.Concat(females).ToDictionary(x => x.Key, x => x.Value);
            return people;
        }

        private static Dictionary<GenderYearAge, double> ReadAgesFile(Gender gender)
        {
            Dictionary<GenderYearAge, double> result = new Dictionary<GenderYearAge, double>();
            string fileName = gender == Gender.male ? "Males.txt" : "Females.txt";
            StreamReader People = new StreamReader(directory + fileName);
            string peopleData = People.ReadToEnd().Trim('\n');
            string[] CurrentAge = peopleData.Split('\n');
            for (int i = 0; i < CurrentAge.Length; i++)
            {
                string[] CurrentNumber = CurrentAge[i].Trim('\t').Split('\t');
                for (int j = 0; j < CurrentNumber.Length; j++)
                {
                    GenderYearAge genderYearAge = new GenderYearAge(gender, 1990 + j, i);
                    result.Add(genderYearAge, Convert.ToDouble(CurrentNumber[j]));
                }
            }
            return result;
        }

        //Public Methods

        public static int GetNonResidentalPOpulation(int year, Method method = Method.analytical)
        {
            if (method == Method.statistical)
                return NonREsidentalPopulaiton[year];
            if (method == Method.analytical)
            {
                int a = year - 1989;
                //double pre = 0.2103 * Math.Pow(a, 6) - 20.192 * Math.Pow(a, 5) +
                //    727.26 * Math.Pow(a, 4) - 12126 * Math.Pow(a, 3) + 93740 * Math.Pow(a, 2) - 249927 * a + 514309;
                //double pre = 52274 * a + 184778;
                double pre = (51786 * a + 189821) * (0.96 + 0.15 * Math.Sin(0.31 * a + 0.1));
                double apr = Math.Round(pre, 0);

                return Convert.ToInt32(apr);
            }
            return -1;
        }

        public static double GetChancesToBecomeRP(int year, Method method = Method.statistical)
        {
            double[] probabilities = new double[] {
                0.07, 0.07, 0.06, 0.06, 0.05,
                0.05, 0.05, 0.04, 0.04, 0.04,
                0.03, 0.03, 0.03, 0.03, 0.02,
                0.02, 0.02, 0.02, 0.01, 0.01,
                0.01, 0.01, 0.01, 0.01, 0.009,
                0.008, 0.007, 0.006, 0.005
            };
            if (method == Method.statistical)
                return probabilities[year - 1990];
            else if (method == Method.analytical)
            {
                int a = year - 1989;
                double pre = 0.0805 * Math.Exp(-0.087 * a) - 0.0022;
                return pre;
            }
            return 0;
        }

        public static double GetChancesToMarry(int year, Gender gender, int age, Method method = Method.statistical)
        {
            if (method == Method.statistical)
            {
                try
                {
                    return ChancesToMarryD[new GenderYearAge(gender, year, age)];
                }
                catch
                {
                    return 0;
                }
            }
            if (method == Method.analytical)
            {
                int a = year - 1989;
                if (gender == Gender.male && age >= 15 && age <= 19)
                    return 1.9132 * Math.Exp(-0.046 * a) / 1000;
                if (gender == Gender.male && age >= 20 && age <= 24)
                    return 40.923 * Math.Exp(-0.054 * a) / 1000;
                if (gender == Gender.male && age >= 25 && age <= 29)
                    return 120.36 * Math.Exp(-0.018 * a) / 1000;
                if (gender == Gender.male && age >= 30 && age <= 34)
                    return 100.57 * Math.Exp(0.0068 * a) / 1000;
                if (gender == Gender.male && age >= 35 && age <= 39)
                    return 68.695 * Math.Exp(0.0097 * a) / 1000;
                if (gender == Gender.male && age >= 40 && age <= 44)
                    return 46.201 * Math.Exp(0.0082 * a) / 1000;
                if (gender == Gender.male && age >= 45 && age <= 49)
                    return 33.564 * Math.Exp(0.0039 * a) / 1000;
                if (gender == Gender.male && age >= 50 && age <= 54)
                    return 22.647 * Math.Exp(0.0051 * a) / 1000;
                if (gender == Gender.male && age >= 55 && age <= 59)
                    return 15.376 * Math.Exp(0.009 * a) / 1000;
                if (gender == Gender.male && age >= 60 && age <= 64)
                    return 8.33351 * Math.Exp(0.0202 * a) / 1000;
                if (gender == Gender.male && age >= 65)
                    return 2.363 * Math.Exp(0.022 * a) / 1000;
                return 0;
            }
            return 0;
        }


        public static double GetChanceToDie(Gender gender, int year, int age, Method method = Method.analytical)
        {
            if (method == Method.statistical)
            {
                GenderYearAge input = new GenderYearAge(gender, year, age);
                int key = input.ToInt();
                return ChancesToDieList[key];
            }
            if (method == Method.analytical)
            {
                double result = 0;
                int a = year - 1989;
                if (gender == Gender.male && age == 0)
                    result = -1.507 * Math.Log(a) + 6.8696;
                if (gender == Gender.male && (age >= 1 && age <= 4))
                    result = -0.091 * Math.Log(a) + 0.4523;
                if (gender == Gender.male && (age >= 5 && age <= 9))
                    result = -0.046 * Math.Log(a) + 0.2429;
                if (gender == Gender.male && (age >= 10 && age <= 14))
                    result = -0.076 * Math.Log(a) + 0.369;
                if (gender == Gender.male && (age >= 15 && age <= 19))
                    result = -0.106 * Math.Log(a) + 0.608;
                if (gender == Gender.male && (age >= 20 && age <= 24))
                    result = -0.152 * Math.Log(a) + 0.9565;
                if (gender == Gender.male && (age >= 25 && age <= 29))
                    result = -0.155 * Math.Log(a) + 0.9186;
                if (gender == Gender.male && (age >= 30 && age <= 34))
                    result = -0.185 * Math.Log(a) + 1.0377;
                if (gender == Gender.male && (age >= 35 && age <= 39))
                    result = -0.237 * Math.Log(a) + 1.5282;
                if (gender == Gender.male && (age >= 40 && age <= 44))
                    result = -0.365 * Math.Log(a) + 2.441;
                if (gender == Gender.male && (age >= 45 && age <= 49))
                    result = -0.713 * Math.Log(a) + 4.2964;
                if (gender == Gender.male && (age >= 50 && age <= 54))
                    result = -1.326 * Math.Log(a) + 7.7206;
                if (gender == Gender.male && (age >= 55 && age <= 59))
                    result = -2.518 * Math.Log(a) + 13.957;
                if (gender == Gender.male && (age >= 60 && age <= 64))
                    result = -4.742 * Math.Log(a) + 24.841;
                if (gender == Gender.male && (age >= 65 && age <= 69))
                    result = -6.697 * Math.Log(a) + 38.041;
                if (gender == Gender.male && (age >= 70 && age <= 74))
                    result = -9.03 * Math.Log(a) + 56.477;
                if (gender == Gender.male && (age >= 75 && age <= 79))
                    result = -11.66 * Math.Log(a) + 83.901;
                if (gender == Gender.male && (age >= 80 && age <= 84))
                    result = -12.69 * Math.Log(a) + 116.78;
                if (gender == Gender.male && (age >= 85))
                    result = -4.116 * Math.Log(a) + 148.27;
                if (gender == Gender.female && age == 0)
                    result = -1.313 * Math.Log(a) + 5.9082;
                if (gender == Gender.female && (age >= 1 && age <= 4))
                    result = -0.06 * Math.Log(a) + 0.3257;
                if (gender == Gender.female && (age >= 5 && age <= 9))
                    result = -0.018 * Math.Log(a) + 0.1573;
                if (gender == Gender.female && (age >= 10 && age <= 14))
                    result = -0.053 * Math.Log(a) + 0.2716;
                if (gender == Gender.female && (age >= 15 && age <= 19))
                    result = -0.042 * Math.Log(a) + 0.3145;
                if (gender == Gender.female && (age >= 20 && age <= 24))
                    result = -0.067 * Math.Log(a) + 0.4171;
                if (gender == Gender.female && (age >= 25 && age <= 29))
                    result = -0.077 * Math.Log(a) + 0.4547;
                if (gender == Gender.female && (age >= 30 && age <= 34))
                    result = -0.111 * Math.Log(a) + 0.6547;
                if (gender == Gender.female && (age >= 35 && age <= 39))
                    result = -0.185 * Math.Log(a) + 1.016;
                if (gender == Gender.female && (age >= 40 && age <= 44))
                    result = -0.27 * Math.Log(a) + 1.5852;
                if (gender == Gender.female && (age >= 45 && age <= 49))
                    result = -0.413 * Math.Log(a) + 2.5501;
                if (gender == Gender.female && (age >= 50 && age <= 54))
                    result = -0.826 * Math.Log(a) + 4.7064;
                if (gender == Gender.female && (age >= 55 && age <= 59))
                    result = -1.571 * Math.Log(a) + 8.2739;
                if (gender == Gender.female && (age >= 60 && age <= 64))
                    result = -2.533 * Math.Log(a) + 13.43;
                if (gender == Gender.female && (age >= 65 && age <= 69))
                    result = -4.353 * Math.Log(a) + 22.917;
                if (gender == Gender.female && (age >= 70 && age <= 74))
                    result = -7.263 * Math.Log(a) + 38.687;
                if (gender == Gender.female && (age >= 75 && age <= 79))
                    result = -9.578 * Math.Log(a) + 59.762;
                if (gender == Gender.female && (age >= 80 && age <= 84))
                    result = -11.49 * Math.Log(a) + 87.708;
                if (gender == Gender.female && age >= 85)
                    result = -8.574 * Math.Log(a) + 139.41;
                return result / 1000;


            }
            return 0;
        }

        public static int GetNumberOfPeople(Gender gender, int year, int age)
        {
            GenderYearAge input = new GenderYearAge(gender, year, age);
            foreach (GenderYearAge key in NumberOfPeople.Keys)
            {
                if (key == input)
                    return Convert.ToInt32(Math.Round(NumberOfPeople[key]));
            }
            throw new Exception("There are not this data " + gender + " " + year + " " + age);
        }

        public static double GetChanceToBorn(int year, int age, Method method = Method.analytical)
        {
            if (method == Method.statistical)
            {
                YearAge input = new YearAge(year, age);
                int key = input.ToInt();
                return ChancesToBornList[key];
            }
            double result = 0;
            if (method == Method.analytical)
            {
                int a = year - 1989;
                
                if (age >= 15 && age <= 19)
                    result = 10.635 * Math.Exp(-0.041 * a);
                if (age >= 20 && age <= 24)
                    result = 62.307 * Math.Exp(-0.046 * a);
                if (age >= 25 && age <= 29)
                    result = 140.84 * Math.Exp(-0.03 * a);
                if (age >= 30 && age <= 34)
                    result = 107.51 * Math.Exp(-0.006 * a);
                if (age >= 35 && age <= 39)
                    result = 39.185 * Math.Exp(0.0051 * a);
                if (age >= 40 && age <= 44)
                    result = 6.2259 * Math.Exp(0.0084 * a);
                if (age >= 45 && a <= 49)
                    result = 0.142 * Math.Exp(0.0329 * a);

            }
            return result / 1000;
        }

    }
}
