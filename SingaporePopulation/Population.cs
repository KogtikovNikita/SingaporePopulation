using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SingaporePopulation
{
    public class Population
    {
        //Initial Population
        private const int MaximulAgeInInitialPopulation = 90;
        private const int MinimumMarriageAgeInitialPopultion = 20;
        private const int AgeOfOldPeopleInitialPopulation = 70;
        private const int AgeGroupSize = 10;


        //Resident - Non Resident Marriages
        private const int MinimumAgeOfCrossStatusMarriages = 20;
        private const int MaximumAgeOfCrossStatusMarriages = 89;
        private const int MaximumAgeForControlStatistics = 90;
        private const int MaximumAgeOfSpouseOverseas = 80;

        //New Citizens
        private bool UsePrivileges = true;
        private double FactorWithoutPrivilenge = 0.4;
        private bool NewCitizensLogs = false;

        //Imigration
        private bool imigrationLogs = false;
        public readonly int MaximumAgeOfMaleImigration = 100;
        public readonly int MaximumAgeOfFemaleImigration = 100;
        public int MalesAgeFix = 0;
        public int FemalesAgeFix = 0;

        //Devorced
        private bool DivorcedLogs = false;

        //Marriages 
        private bool MarriageLogs = true;
        private bool UseMarriagesFactors = true;
        private double FactorDevorceToMarry = 2.7;
        private double FactorSingleToMarry = 1;
        private double FactorWidowedToMarry = 8;
        private double FactorNonresidentToMarry = 3.4;
        private double ChanceToForeignerBride = 0.28;
        private readonly int MinimumBrideAge = 15;
        private double ChanceToMarrySingle = 0.8;
        private double ChanceToMarryDivorced = 0.95;


        public List<Person> citizens { get; }
        int CurrentId;
        private bool WrittenStat = false;
        public Dictionary<string, double> Characteristics = new Dictionary<string, double>();


        private Population()
        {
            citizens = new List<Person>();
            CurrentId = 0;
        }



        static public Population CreateInitialPopulation(int year)
        {
            //Residents
            Population result = new Population();
            for (int i = 0; i < MaximulAgeInInitialPopulation; i++)
            {
                int NumberOfPersons = Chances.GetNumberOfPeople(Gender.male, year, i);
                for (int j = 0; j < NumberOfPersons; j++)
                {
                    result.citizens.Add(Person.Initial(Gender.male, i));
                    result.CurrentId++;
                }
                
                NumberOfPersons = Chances.GetNumberOfPeople(Gender.female, year, i);
                for (int j = 0; j < NumberOfPersons; j++)
                {
                    result.citizens.Add(Person.Initial(Gender.female, i));
                    result.CurrentId++;
                }
            }
            result.ReWriteCHarasteristics(year);
            
            
            //Non-residents
            int NonResidents = Chances.GetNonResidentalPOpulation(year, method: Method.statistical);
            for (int j=0; j<NonResidents; j++)
            {
                result.citizens.Add(Person.Initial(result));
            }
            Dictionary<Tuple<int, Gender>, int> set = new Dictionary<Tuple<int, Gender>, int>();

            List<Gender> genders = new List<Gender>() { Gender.male, Gender.female };

           
                //Determine widowed ;( persons
                for (int a = MinimumMarriageAgeInitialPopultion; a < MaximulAgeInInitialPopulation; a++)
                {
                    int age = a;
                    GenderYearAge genderYearAgeM = new GenderYearAge(Gender.male, year, age);
                    GenderYearAge genderYearAgeF = new GenderYearAge(Gender.female, year, age);
                    List<Person> peopleM, peopleF;
                    if (a < AgeOfOldPeopleInitialPopulation)
                    {
                        peopleM = result.citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident && x.age == a);
                        peopleF = result.citizens.FindAll(x => x.gender == Gender.female && x.state == State.resident && x.age == a);
                    }
                    else
                    {
                        peopleM = result.citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident && (x.age == a || x.age == a + AgeGroupSize));
                        peopleF = result.citizens.FindAll(x => x.gender == Gender.female && x.state == State.resident && (x.age == a || x.age == a + AgeGroupSize));

                    }

                for (int i = 0; i < Chances.Widowed[genderYearAgeM.ToInt()]; i++)
                    {
                        peopleM[i].RelationshipStatus = MarriageStatus.widowed;
                    }
                    for (int i = 0; i < Chances.Widowed[genderYearAgeF.ToInt()]; i++)
                    {
                        peopleF[i].RelationshipStatus = MarriageStatus.widowed;
                    }
                }

                //Determine divorced people
                for (int a = MinimumMarriageAgeInitialPopultion; a < MaximulAgeInInitialPopulation; a++)
                {
                    int age = a;
                    GenderYearAge genderYearAgeM = new GenderYearAge(Gender.male, year, age);
                    GenderYearAge genderYearAgeF = new GenderYearAge(Gender.female, year, age);
                    List<Person> peopleM, peopleF;
                    if (a < AgeOfOldPeopleInitialPopulation)
                    {
                        peopleM = result.citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident && x.age == a && x.RelationshipStatus == MarriageStatus.single);
                        peopleF = result.citizens.FindAll(x => x.gender == Gender.female && x.state == State.resident && x.age == a && x.RelationshipStatus == MarriageStatus.single);
                    }
                    else
                    {
                        peopleM = result.citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident && (x.age == a || x.age == a + 10));
                        peopleF = result.citizens.FindAll(x => x.gender == Gender.female && x.state == State.resident && (x.age == a || x.age == a + 10));

                    }
                    for (int i = 0; i < Chances.Divorced[genderYearAgeM.ToInt()]; i++)
                    {
                        peopleM[i].RelationshipStatus = MarriageStatus.divorced;
                    }
                    for (int i = 0; i < Chances.Divorced[genderYearAgeF.ToInt()]; i++)
                    {
                        peopleF[i].RelationshipStatus = MarriageStatus.divorced;
                    }
                }

                //Determine Married people
                for (int a = MinimumMarriageAgeInitialPopultion; a < MaximulAgeInInitialPopulation; a++)
                {
                    int age = a;
                    GenderYearAge genderYearAgeM = new GenderYearAge(Gender.male, year, age);
                    List<Person> peopleM = result.citizens.FindAll(x => x.gender == Gender.male && x.age == a
                    && x.RelationshipStatus == MarriageStatus.single && x.state == State.resident);
                    List<Person> Spouses = new List<Person>();
                    int AgeDifference = 0;
                    do
                    {
                        Spouses = result.citizens.FindAll(x => (x.age <= a + AgeDifference
                        && x.age >= a - AgeDifference) && x.RelationshipStatus == MarriageStatus.single && x.gender == Gender.female);
                        AgeDifference++;
                    }
                    while (Spouses.Count < peopleM.Count);
                    for (int i = 0; i < Math.Min(Chances.Married[genderYearAgeM.ToInt()],peopleM.Count); i++)
                    {

                        var spouse = Spouses[i];
                        peopleM[i].Marry(spouse);
                    }
                }


            // Male Resident + Females Non-residents marriages 
            ResidentNonResidentMarriages(result, year, true);

            // Female Resident + Males Non-residents marriages 
            ResidentNonResidentMarriages(result, year, false);

            

            return result;
        }


        static private void ResidentNonResidentMarriages (Population result, int year, bool areMalesResidents)
        {
            Gender ResidentGender, SpouseGender;
            if (areMalesResidents)
            {
                ResidentGender = Gender.male;
                SpouseGender = Gender.female;
            }
            else
            {
                ResidentGender = Gender.female;
                SpouseGender = Gender.male;
            }
            var tomarry = result.citizens.FindAll(x => x.gender == ResidentGender && x.state == State.resident &&
                x.RelationshipStatus == MarriageStatus.single && x.age >= MinimumAgeOfCrossStatusMarriages && x.age <= MaximumAgeOfCrossStatusMarriages);
            int current = tomarry.Count;
            int need = 0;
            for (int age = MinimumAgeOfCrossStatusMarriages; age < MaximumAgeForControlStatistics; age++)
            {
                int y = year;
                Gender gen = ResidentGender;
                int a = age;
                GenderYearAge key = new GenderYearAge(gen, y, a);
                need += Chances.Singles[key.ToInt()];
            }
            int WorkINdex = 0;
            var spousesOverseas = result.citizens.FindAll(x => x.gender == SpouseGender
                && x.state == State.nonresident && x.RelationshipStatus == MarriageStatus.single
                && x.age >= MinimumAgeOfCrossStatusMarriages && x.age <= MaximumAgeOfSpouseOverseas);
            while (current > need)
            {
                tomarry[WorkINdex].Marry(spousesOverseas[WorkINdex]);
                WorkINdex++;
                current--;
            }
        }


        #region Statistics Collector
        public static string GenerateKeyforCharasteristicsDictionary (Gender gender, int startAge, int finishAge)
        {
            return gender + "(" + startAge + ";" + finishAge + ")";
        }

        public static string GenerateKeyforCharasteristicsDictionary (Gender gender, int startAge)
        {
            return gender + "(" + startAge + "+)";
        }

        public static string GenerateKeyforCharasteristicsDictionary (Gender gender, int startAge, int finishAge, MarriageStatus ms)
        {
            return gender + "(" + startAge + ";" + finishAge + ")" + ms;
        }

        public static string GenerateKeyforCharasteristicsDictionary(Gender gender, int startAge, MarriageStatus ms)
        {
            return gender + "(" + startAge + "+)" + ms;
        }

        private void AddCharacteristicsMarriageStatus (MarriageStatus MS)
        {
            string ms = MS.ToString();
            List<Gender> genders = new List<Gender>() { Gender.male, Gender.female };
            for (int g = 0; g < genders.Count; g++)
            {
                for (int a = 2; a < 8; a++)
                {
                    Predicate<Person> pred = new Predicate<Person>(x => x.gender == genders[g] && x.age >= a * 10 && x.age <= a*10 + 9 && x.RelationshipStatus == MS && x.state == State.resident);
                    
                    int value = citizens.FindAll(pred).Count;
                    if (MS == MarriageStatus.married && g ==0) 
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.White; 
                    }
                    string key = GenerateKeyforCharasteristicsDictionary(genders[g], a * 10, a * 10 + 9, MS);
                    Characteristics.Add(key, value);
                }
                Predicate<Person> predFinal = new Predicate<Person>(x => x.gender == genders[g] && x.age >= 80  && x.RelationshipStatus == MS && x.state == State.resident);
                int valueFinal = citizens.FindAll(predFinal).Count;
                Characteristics.Add(GenerateKeyforCharasteristicsDictionary(genders[g], 80, MS), valueFinal);
                if (MS == MarriageStatus.married && g == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        private void AddAgeCharasteristics ()
        {
            List<Gender> genders = new List<Gender>() { Gender.male, Gender.female };
            for (int g = 0; g < genders.Count; g++)
            {
                int TotalAge = 0;
                int TotalHumans = 0;
                for (int a = 0; a < 100; a++)
                {
                    Predicate<Person> pred = new Predicate<Person>(x => x.gender == genders[g] && x.age == a  && x.state == State.resident);
                    int value = citizens.FindAll(pred).Count;
                    string key = GenerateKeyforCharasteristicsDictionary(genders[g], a);
                    Characteristics.Add(key, value);
                    TotalAge += value * a;
                    TotalHumans += value;
                }
                Characteristics.Add(genders[g] + "AverageAge ", (double)TotalAge / (double)TotalHumans);
            }
        }

        public void ReWriteCHarasteristics (int year)
        {
            Characteristics.Clear();
            Characteristics.Add("year", year);
            Characteristics.Add("Residents", citizens.FindAll(x => x.state == State.resident).Count);
            Characteristics.Add("Non-residetns", citizens.FindAll(x => x.state == State.nonresident).Count);
            AddGenderCharacteristics();
            AddAgeCharasteristics();
            AddCharacteristicsMarriageStatus(MarriageStatus.divorced);
            AddCharacteristicsMarriageStatus(MarriageStatus.married);
            AddCharacteristicsMarriageStatus(MarriageStatus.single);
            AddCharacteristicsMarriageStatus(MarriageStatus.widowed);
            Characteristics.Add("AdultMales", citizens.FindAll(x => x.state == State.resident && x.gender == Gender.male && x.age >=20).Count);
            Characteristics.Add("AdultFemales", citizens.FindAll(x => x.state == State.resident && x.gender == Gender.female && x.age >= 20).Count);
        }

        private void AddGenderCharacteristics()
        {
            Characteristics.Add("Males", citizens.FindAll(x => x.state == State.resident && x.gender == Gender.male).Count);
            Characteristics.Add("Females", citizens.FindAll(x => x.state == State.resident && x.gender == Gender.female).Count);
        }

        public void PrintOverallPopulation(int year)
        {
            StreamWriter sw = new StreamWriter(Chances.GetDirectory(folder: "Results") +  @"Peoples.txt", true);
            if (!WrittenStat)
            {
                WrittenStat = true;
                string TitleLine = "";
                foreach (var key in Characteristics.Keys)
                {
                    TitleLine += key + "\t";
                }
                sw.WriteLine(TitleLine);
            }
            string line = "";
            ReWriteCHarasteristics(year);
            foreach (var key in Characteristics.Keys)
            {
                line += Characteristics[key] + "\t";
            }
            sw.WriteLine(line);
            sw.Close();
        }
        public void PrintGenders (Gender gender)
        {
            Console.WriteLine("Males: " + citizens.FindAll(x => x.gender == gender).Count);
        }
        #endregion




        public int GetNonCitizensCount ()
        {
            return citizens.FindAll(x => x.state == State.nonresident).Count;
        }

        private void ModelingDeaths (int year)
        {
            Stopwatch SW = new Stopwatch();
            SW.Start();
            Parallel.For(0, citizens.Count, (i, state) =>
            {
                // Die
                bool died = false;
                citizens[i].Die(year, ref died);
            });
            SW.Stop();
            Console.WriteLine("Die " + SW.ElapsedMilliseconds);
            SW.Reset();
            SW.Start();

            citizens.RemoveAll(x => x.GoingToDie == true);

            SW.Stop();
            Console.WriteLine("Removing " + SW.ElapsedMilliseconds);
            SW.Reset();
        }

        private void ModelingNewBorns (int year)
        {
            int newkids = 0;
            object locker = new object();
            Stopwatch SW = new Stopwatch();
            SW.Start();
            Parallel.For(0, citizens.Count, (i, state) =>
            {

                // Born new child
                if (citizens[i].gender == Gender.female)
                {
                    bool newchild = false;
                    Person child = citizens[i].BornNewChild(Chances.GetChanceToBorn(year, citizens[i].age, Method.analytical), ref newchild);
                    if (child != null)
                    {
                        lock(locker)
                        {
                            newkids++;
                        }
                    }
                }
            });
            SW.Stop();
            Console.WriteLine("Born " + SW.ElapsedMilliseconds);
            SW.Reset();
            SW.Start();
            for (int i = 0; i < newkids; i++)
                citizens.Add(Person.Born());
            SW.Stop();
            Console.WriteLine("Adding " + SW.ElapsedMilliseconds);
            SW.Reset();
        }

        private void Birthdays ()
        {
            Stopwatch SW = new Stopwatch();
            SW.Start();
            Parallel.For(0, citizens.Count, (i, state) =>
            {

                //Celebrate birthday
                citizens[i].CelebrateBirthday();
            });

            SW.Stop();
            Console.WriteLine("Celebrations: " + SW.ElapsedMilliseconds);


            SW.Reset();
        }

        private void NewCitizens (int year)
        {
            Stopwatch SW = new Stopwatch();
            SW.Start();
            int[] ageGroups = new int[15];
            for (int i = 0; i < ageGroups.Length; i++)
                ageGroups[i] = 0;
            double ChanceInitial = Chances.GetChancesToBecomeRP(year, Method.analytical);
            int Total = citizens.FindAll(x => x.state == State.nonresident).Count;
            int AproximateNew = Convert.ToInt32(Math.Round(Total * ChanceInitial));
            
            Predicate<Person> privilege = x => x.state == State.nonresident
                && (x.RelationshipStatus == MarriageStatus.married);
            Predicate<Person> nonprivilege = x => x.state == State.nonresident
                && x.RelationshipStatus != MarriageStatus.married;
            int married = 0;
            int notmarried = 0;
            int WithPrivilege = citizens.FindAll(privilege).Count;
            int WithoutPrivilege = citizens.FindAll(nonprivilege).Count;
            double ChanceWithoutPrivilege = ChanceInitial * FactorWithoutPrivilenge;
            double ChanceWithPrivilege = (AproximateNew - WithoutPrivilege * ChanceWithoutPrivilege) / WithPrivilege;
            //GetCitizen
            for (int i = 0; i < citizens.Count; i++)
            {
                if (citizens[i].state == State.nonresident)
                {
                    if (UsePrivileges)
                    {
                        if (privilege(citizens[i]))
                            citizens[i].BecomeCitizen(ChanceWithPrivilege);
                        else
                            citizens[i].BecomeCitizen(ChanceWithoutPrivilege);
                    }
                    else
                    {
                        citizens[i].BecomeCitizen(ChanceInitial);
                    }
                    if (NewCitizensLogs)
                    {
                        if (citizens[i].state == State.resident)
                        {
                            ageGroups[Convert.ToInt32((citizens[i].age - citizens[i].age % 10) / 10)]++;
                            if (citizens[i].RelationshipStatus == MarriageStatus.married)
                                married++;
                            else
                                notmarried++;

                        }
                    }
                }
            }
            SW.Stop();
            Console.WriteLine("New passports: " + SW.ElapsedMilliseconds);
            if (NewCitizensLogs)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < ageGroups.Length; i++)
                    Console.WriteLine("from " + i * 10 + " to " + (i * 10 + 9) + " " + ageGroups[i]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("MArried " + married);
                Console.WriteLine("Not Married " + notmarried);
                Console.ForegroundColor = ConsoleColor.White;
            }
            SW.Reset();
        }


        private void Immigration (int year)
        {
            int[] ageGroups = new int[10];
            for (int i = 0; i < 10; i++)
                ageGroups[i] = 0;
            Stopwatch SW = new Stopwatch();
            SW.Start();
            var noncit = GetNonCitizensCount();
            //Immigration
            if (Chances.GetNonResidentalPOpulation(year + 1, Method.statistical) - noncit > 0)
            {
                for (int i = 0; i < Chances.GetNonResidentalPOpulation(year + 1, Method.analytical) - noncit; i++)
                {
                    Person newPerson = Person.InitialNewPerson(this);
                    citizens.Add(newPerson);
                    if (imigrationLogs)
                    {
                        ageGroups[Convert.ToInt32(newPerson.age / 10)]++;
                    }
                }
            }
            else
            {
                int removed = 0;
                for (int i = citizens.Count - 1; i > 0; i--)
                {
                    if (citizens[i].state == State.nonresident)
                    {
                        citizens.RemoveAt(i);
                        removed++;
                        if (removed >= noncit - Chances.GetNonResidentalPOpulation(year + 1))
                            break;
                    }
                }
            }
            SW.Stop();
            Console.WriteLine("New Immigrants: " + SW.ElapsedMilliseconds);
            SW.Reset();
            if (imigrationLogs)
            {
                for (int i = 0; i < 10; i++)
                    Console.WriteLine("Age Group " + i * 10 + " - " + (i * 10 + 9) + " : " + ageGroups[i]);
            }
        }

        private void Divorces (int year)
        {
            Stopwatch SW = new Stopwatch();
            if (DivorcedLogs)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                for (int i = 0; i < 6; i++)
                {
                    int startAge = 5 * i + 20;
                    int endAge = startAge + 4;
                    int personos = citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident &&
                        x.RelationshipStatus == MarriageStatus.married && x.age >= startAge && x.age <= endAge).Count;
                    double chances = Chances.GetChancesToDivorce(Gender.male, startAge, year);
                    Console.WriteLine("From " + startAge + " to " + endAge + " : " + personos +
                        " Chances: " + chances + " Approximate Divorces: " + (personos * chances));

                }
                int personosOld = citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident &&
                    x.RelationshipStatus == MarriageStatus.married && x.age >= 50).Count;
                double chancesOld = Chances.GetChancesToDivorce(Gender.male, 50, year);
                Console.WriteLine("From " + 50 + " : " + personosOld +
                    " Chances: " + chancesOld + " Approximate Divorces: " + (personosOld * chancesOld));
                Console.ForegroundColor = ConsoleColor.White;
            }
            //Divorces
            SW.Start();
            if (DivorcedLogs)
            {
                Console.WriteLine("Total Divorced Before: " + citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident &&
                        x.RelationshipStatus == MarriageStatus.divorced).Count);
            }
            Parallel.For(0, citizens.Count, i => {

                if (citizens[i].RelationshipStatus == MarriageStatus.married && citizens[i].gender == Gender.male)
                {
                    bool IsGoingToDivorce = Randomizer.GetRandom(Chances.GetChancesToDivorce(Gender.male, citizens[i].age, year, Method.analytical));
                    if (IsGoingToDivorce)
                    {

                        citizens[i].Devorce();
                    }
                }
            });
            if (DivorcedLogs)
            {
                Console.WriteLine("Total Divorced After: " + citizens.FindAll(x => x.gender == Gender.male && x.state == State.resident &&
                    x.RelationshipStatus == MarriageStatus.divorced).Count);
            }
            SW.Stop();
            Console.WriteLine("Divorces: " + SW.ElapsedMilliseconds);
            SW.Reset();
        }

        private void CalculateFactors()
        {

        }

        private void Marriages (int year)
        {

            Stopwatch SW = new Stopwatch();
            SW.Start();
            var brideLocalsSingle = citizens.FindAll(x => x.RelationshipStatus != MarriageStatus.married && x.state == State.resident &&
                x.gender == Gender.female && x.age >= MinimumBrideAge && x.RelationshipStatus == MarriageStatus.single);
            var brideLocalsDivorced = citizens.FindAll(x => x.RelationshipStatus != MarriageStatus.married && x.state == State.resident &&
                x.gender == Gender.female && x.age >= MinimumBrideAge && x.RelationshipStatus == MarriageStatus.divorced);
            var brideLocalsWidowed = citizens.FindAll(x => x.RelationshipStatus != MarriageStatus.married && x.state == State.resident &&
                x.gender == Gender.female && x.age >= MinimumBrideAge && x.RelationshipStatus == MarriageStatus.widowed);
            var brideForeigners = citizens.FindAll(x => x.RelationshipStatus != MarriageStatus.married && x.state == State.nonresident &&
                x.gender == Gender.female && x.age >= MinimumBrideAge);
            Dictionary<int, int> MarriesPersons = new Dictionary<int, int>();
            Dictionary<int, int> AllPersons = new Dictionary<int, int>();
            for (int i = 0; i <= 110; i++)
            {
                MarriesPersons.Add(i, 0);
                AllPersons.Add(i, 0);
            }
            object AccessToSingles = new object();
            int singles = 0;
            int divorced = 0;
            int widowed = 0;
            if (MarriageLogs)
            {
                Console.WriteLine("Singles: " + citizens.FindAll(x => x.RelationshipStatus == MarriageStatus.single && x.state == State.resident && x.age >= 15).Count);
                Console.WriteLine("Divorced: " + citizens.FindAll(x => x.RelationshipStatus == MarriageStatus.divorced && x.state == State.resident && x.age >= 15).Count);
                Console.WriteLine("Widowed: " + citizens.FindAll(x => x.RelationshipStatus == MarriageStatus.widowed && x.state == State.resident && x.age >= 15).Count);
            }
            Parallel.For(0, citizens.Count, (i, state) =>
            {
                if (citizens[i].gender == Gender.male && citizens[i].RelationshipStatus != MarriageStatus.married
                   && citizens[i].age >= MinimumBrideAge)
                {
                    double factor = 1;
                    if (UseMarriagesFactors)
                    {
                        switch (citizens[i].RelationshipStatus)
                        {
                            case (MarriageStatus.divorced):
                                factor = FactorDevorceToMarry;
                                break;
                            case (MarriageStatus.single):
                                factor = FactorSingleToMarry;
                                break;
                            case (MarriageStatus.widowed):
                                factor = FactorWidowedToMarry;
                                break;

                        }
                    }
                    if (citizens[i].state == State.nonresident)
                        factor = FactorNonresidentToMarry;
                    bool IsGoingToMarry = Randomizer.GetRandom(Chances.GetChancesToMarry(year, Gender.male, citizens[i].age, Method.analytical)*factor);

                    if (IsGoingToMarry)
                    {
                        if (citizens[i].state == State.resident)
                        {
                            if (citizens[i].RelationshipStatus == MarriageStatus.divorced)
                                divorced++;
                            if (citizens[i].RelationshipStatus == MarriageStatus.widowed)
                                widowed++;
                            if (citizens[i].RelationshipStatus == MarriageStatus.single)
                                singles++;
                        }
                        lock (AccessToSingles)
                        {
                            Random rand = new Random();
                            double chance = rand.NextDouble();
                            Person Cbride;
                            if (chance < (citizens[i].state == State.resident ? ChanceToForeignerBride : 0))
                            {
                                Cbride = brideForeigners[0];
                                brideForeigners.RemoveAt(0);
                            }
                            else
                            {
                                if (rand.NextDouble() <  ChanceToMarrySingle)
                                {
                                    Cbride = brideLocalsSingle[0];
                                    brideLocalsSingle.RemoveAt(0);
                                }
                                else if (rand.NextDouble() < ChanceToMarryDivorced)
                                {
                                    Cbride = brideLocalsDivorced[0];
                                    brideLocalsDivorced.RemoveAt(0);
                                }
                                else

                                {
                                    Cbride = brideLocalsWidowed[0];
                                    brideLocalsWidowed.RemoveAt(0);
                                }
                            }
                            citizens[i].Marry(Cbride);
                        }
                    }
                }
            });
            if (MarriageLogs)
            {
                Console.WriteLine("Singles Married: " + singles);
                Console.WriteLine("Divorced Married: " + divorced);
                Console.WriteLine("Widowed Married: " + widowed);
            }
            SW.Stop();
            Console.WriteLine("Marriages: " + SW.ElapsedMilliseconds);

            SW.Reset();
        }




        public void ModelYear(int year)
        {
            Console.WriteLine("__________________" + year + "______________________________");
            ModelingNewBorns(year);
            ModelingDeaths(year);
            Birthdays();

            NewCitizens(year);
            
            Immigration(year);
            Divorces(year);
            Marriages(year);


        }

    }
}
