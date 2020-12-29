using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    public class MinMaxKNN
    {
        private double minValue;
        private double maxValue;

        public double MinValue { get { return minValue; } set { minValue = value; } }
        public double MaxValue { get { return maxValue; } set { maxValue = value; } }

        public MinMaxKNN(double a, double b)
        {
            minValue = a;
            maxValue = b;
        }

        public void compareX(double x)
        {
            if (x < minValue) minValue = x;
            if (x > maxValue) maxValue = x;
        }
    }
}
