using System;
using System.Collections.Generic;
using System.Text;

namespace SingaporePopulation
{
    public class RandomAge : Random
    {
        public double[] Value { get; }
        public int[] Count { get; }

        public RandomAge (double[] value, int[] count)
        {
            Value = value;
            Count = count;
        }

        private int GetSUM()
        {
            int sum = 0;
            for (int i = 0; i < Count.Length; i++)
                sum += Count[i];
            return sum;
        }

        public double GetRandom()
        {
            int sum = GetSUM();
            int value = base.Next(sum);
            int bottom = 0;
            int top = Count[0];
            for (int i = 0; i < Count.Length; i++)
            {
                if ((value > bottom) && (value < top)) 
                    return Value[i];
                else
                {
                    try
                    {
                        bottom += Count[i];
                        top += Count[i + 1];
                    }
                    catch
                    {
                        return Value[Value.Length - 1];
                    }
                }
            }
            return -1;

        }
    }
}
