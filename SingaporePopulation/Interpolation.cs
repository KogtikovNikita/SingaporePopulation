using System;
using System.Collections.Generic;
using System.Text;

namespace SingaporePopulation
{
    public class Interpolation
    {
        List<double> X = new List<double>();
        List<double> Y = new List<double>();
        public Interpolation(List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
                throw new Exception("Inteprolation Error");
            X = x;
            Y = y;
        }

        private double l (int i, double value)
        {
            double result = 1;
            for (int j=0; j<X.Count; j++)
            {
                if (j != i)
                    result *= (value - X[j]) / (X[i] - X[j]);

            }
            return result;
        }

        public double InterpolateValue (double value)
        {
            double result = 0;
            for (int i=0; i < X.Count; i++)
            {
                result += Y[i] * l(i, value);
            }
            return result;
        }
    }
}
