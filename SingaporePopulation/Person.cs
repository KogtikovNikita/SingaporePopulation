using System;
using System.Collections.Generic;
using System.Text;

namespace SingaporePopulation
{
    static class Randomizer
    {
        static public bool GetRandom(double probability)
        {
            Random random = new Random();
            double chance = random.NextDouble();
            if (chance < probability)
                return true;
            else
                return false;
        }
    
    }



    public enum Gender { male, female }
    public enum State { resident, nonresident }

    public enum MarriageStatus {single, married, widowed, divorced }

    public class Person
    {
        public  int age { get; private set; }
        public  Gender gender { get; private set; }
        public Person partner { get;  set; }
        public Person[] parents { get; private set; }
        public List<Person> children { get; private set; }
        public State state { get; private set; }
        public bool GoingToDie { get; set; } = false;
        public MarriageStatus RelationshipStatus { get; set; } = MarriageStatus.single;

        public static int Widowed = 0;



        private void SpouceDied ()
        {
            partner = null;
            RelationshipStatus = MarriageStatus.widowed;
        }


        private void ParentDied (Person parent)
        {
            if (parents[0] == parent) parents[0] = null;
            if (parents[1] == parent) parents[1] = null;
        }

        private void ChildDied (Person child)
        {
            children.Remove(child);
        }

        private Person(State State = State.resident)
        {
            age = 0;
            gender = Gender.male;
            partner = null;
            parents = new Person[2];
            children = new List<Person>();
            state = State;
        }


        static private int CreateAge(Gender gender, Population pop, int UpAge = 100)
        {
            int[] count = new int[UpAge];
            double[] value = new double[UpAge];
            for (int i=0; i<UpAge; i++)
            {
                string key = Population.GenerateKeyforCharasteristicsDictionary(gender, i);
                count[i] = Convert.ToInt32(pop.Characteristics[key]);
                value[i] = i;
            }
            RandomAge randomAge = new RandomAge(value, count);
            return Convert.ToInt32(randomAge.GetRandom());
        }

        static private Gender CreateGender (double MaleProb = 0.48)
        {
            bool isMale = Randomizer.GetRandom(MaleProb);
            if (isMale)
                return Gender.male;
            else
                return Gender.female;
        }

        static public Person Born (Person parent1 = null, Person parent2 = null)
        {
            Person person = new Person();
            person.parents[0] = parent1;
            person.parents[1] = parent2;
            person.gender = CreateGender(MaleProb: 0.46);
            return person;
        }


        static public Person Imigration(int age, Gender gender)
        {
            Person person = new Person();
            person.gender = gender;
            person.age = age;
            return person;
        }
        static public Person Initial (Population population)
        {
            Person person = new Person(State.nonresident);
            person.gender = CreateGender();
            person.age = CreateAge(person.gender, population);
            return person;
        }

        static public Person Initial (Gender Gender, int Age)
        {
            Person person = new Person();
            person.gender = Gender;
            person.age = Age;
            return person;
        }

        public void CelebrateBirthday()
        {
            age += 1;
        }

        public void BecomeCitizen(double probability)
        {
            bool GoingToBECitizen = Randomizer.GetRandom(probability);
            if (GoingToBECitizen)
                state = State.resident;
        }

        public void SetRelationshipStatus (double MarriedChance, double WidowedChance, double DivorcedChance, Person spouse)
        {
            if (Randomizer.GetRandom(WidowedChance))
            {
                RelationshipStatus = MarriageStatus.widowed;
                return;
            }
            if (Randomizer.GetRandom(DivorcedChance))
            {
                RelationshipStatus = MarriageStatus.divorced;
                spouse.RelationshipStatus = MarriageStatus.divorced;
                return;
            }
            if (Randomizer.GetRandom(MarriedChance))
            {
                Marry(spouse);
                return;
            }
        }

        public void Marry(Person spouse)
        {
            if (partner == null)
            {
                partner = spouse;
                RelationshipStatus = MarriageStatus.married;
                partner.partner = this;
                partner.RelationshipStatus = MarriageStatus.married;
            }
        }




        public void Devorce()
        {
            if (partner != null)
            {
                RelationshipStatus = MarriageStatus.divorced;
                try
                {
                    partner.RelationshipStatus = MarriageStatus.divorced;
                    partner.partner = null;
                    partner = null;
                }
                catch
                {
                    
                }
            }
        }

        public void NewChild (Person child)
        {
            children.Add(child);
        }

        public Person BornNewChild(double probability, ref bool newborn)
        {
            bool GoingToBorn = Randomizer.GetRandom(probability);
            if (GoingToBorn)
            {
                Person child = Person.Born();
                children.Add(child);
                if (partner != null)
                    partner.NewChild(child);
                newborn = true;
                return child;
            }
            return null;
        }

        public void Die(int year, ref bool died)
        {
            GoingToDie = Randomizer.GetRandom(Chances.GetChanceToDie(gender, year, age, Method.analytical));
            if (GoingToDie)
            {
                foreach (Person child in children)
                    child.ParentDied(this);
                foreach (Person parent in parents)
                    if (parent != null)
                        parent.ChildDied(this);
                if (partner != null)
                    partner.SpouceDied();
                died = true;
            }
        }

        internal static Person InitialNewPerson(Population population)
        {
            Person person = new Person(State.nonresident);
            person.gender = CreateGender(MaleProb: 0.43);
            int UpAge = person.gender == Gender.male? population.MaximumAgeOfMaleImigration : population.MaximumAgeOfFemaleImigration;
            int AgeFix = person.gender == Gender.male ? population.MalesAgeFix : population.FemalesAgeFix;
            person.age = CreateAge(person.gender, population, UpAge) + AgeFix;
            return person;
        }
    }
}
