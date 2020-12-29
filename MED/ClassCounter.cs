using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MED
{
    class ClassCounter
    {
        private List<string> classes;
        private List<int> counters;

        public List<string> Classes { get { return classes; } set { classes = value; } }
        public List<int> Counters { get { return counters; } set { counters = value; } }

        public ClassCounter(List<string> cls)
        {
            classes = cls;
            counters = new List<int>();
            foreach (var v in classes) counters.Add(0);
        }

        public ClassCounter()
        {
            classes = new List<string>();
            counters = new List<int>();
        }

        public ClassCounter(AttributeTable table)
        {
            classes = new List<string>();
            counters = new List<int>();
            foreach (var v in table.Attributes) addClass(v.AttributeClass);
        }

        public void addClass(string cls)
        {
            for (int i = 0; i < classes.Count; i++)
            {
                if (classes[i].Equals(cls))
                {
                    counters[i]++;
                    return;
                }
            }
            classes.Add(cls);
            counters.Add(1);
        }

        private double calculateGini()
        {
            double sum = (double)counters.Sum();
            double result = 1;
            foreach (var v in counters) result -= Math.Pow((double)v / sum, 2);
            return result;
        }

        public double calculateGiniSplit(ClassCounter globalTable)
        {
            double nominator = (double)counters.Sum();
            double denominator = (double)globalTable.counters.Sum();
            return calculateGini() * nominator / denominator;
        }

    }
}