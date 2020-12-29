using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class AttributeTable
    {
        private List<DataAttribute> attributes;
        private bool isNumerical;

        public List<DataAttribute> Attributes { get { return attributes; } set { attributes = value; } }

        public AttributeTable(List<DataAttribute> att, bool n)
        {
            attributes = att;
            isNumerical = n;
        }

        private void sortTable()
        {
            if (isNumerical) attributes = attributes.OrderBy(x => x.getValueAsDouble()).ToList();
            else attributes = attributes.OrderBy(x => x.getValueAsString()).ToList();
        }

        private double rateSplit(AttributeTable first, AttributeTable second, ClassCounter allTable)
        {
            ClassCounter firstCounter = new ClassCounter(first);
            ClassCounter secondCounter = new ClassCounter(second);
            return firstCounter.calculateGiniSplit(allTable) + secondCounter.calculateGiniSplit(allTable);
        }

        private Tuple<AttributeTable, AttributeTable> splitTableNumerical(int i)
        {
            AttributeTable first = new AttributeTable(attributes.GetRange(0, i), isNumerical);
            AttributeTable second = new AttributeTable(attributes.GetRange(i, attributes.Count - i), isNumerical);
            Tuple<AttributeTable, AttributeTable> toReturn = new Tuple<AttributeTable, AttributeTable>(first, second);
            return toReturn;
        }

        private Tuple<AttributeTable, AttributeTable> splitTableString(string s)
        {
            int a = 0;
            int b = 0;
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].getValueAsString().Equals(s))
                {
                    a = i;
                    break;
                }
            }

            for (int i = a; i < attributes.Count; i++)
            {
                if (attributes[i].getValueAsString().Equals(s)) b++;
                else break;
            }

            List<DataAttribute> leftList = new List<DataAttribute>();
            List<DataAttribute> rightList = new List<DataAttribute>();

            rightList.AddRange(attributes.GetRange(0, a));
            leftList.AddRange(attributes.GetRange(a, b));
            rightList.AddRange(attributes.GetRange(a + b, attributes.Count - a - b));

            AttributeTable leftTable = new AttributeTable(leftList, isNumerical);
            AttributeTable rightTable = new AttributeTable(rightList, isNumerical);

            Tuple<AttributeTable, AttributeTable> toReturn = new Tuple<AttributeTable, AttributeTable>(leftTable, rightTable);
            return toReturn;
        }

        public Tuple<string, double> findBestSplit()
        {
            if (isNumerical) return findBestSplitNumerical();
            else return findBestSplitString();
        }

        public Tuple<string, double> findBestSplitNumerical()
        {
            sortTable();
            ClassCounter allTable = new ClassCounter(this);
            string splitValue = attributes[0].getValueAsString();
            double bestGini = 999;
            double currentGini;
            int j=0;

            for(int i = 0; i < attributes.Count-1; i++)
            {
                if (!attributes[i].getValueAsString().Equals(attributes[i + 1].getValueAsString()))
                {
                    var tuple = splitTableNumerical(i + 1);
                    currentGini = rateSplit(tuple.Item1, tuple.Item2, allTable);
                    if(currentGini < bestGini)
                    {
                        bestGini = currentGini;
                        splitValue = attributes[i].getValueAsString();
                        j = i;
                    }
                }
            }

            bestGini = (attributes[j].getValueAsDouble() + attributes[j + 1].getValueAsDouble()) / 2;
            Tuple<string, double> toReturn = new Tuple<string, double>(splitValue, bestGini);
            return toReturn;
        }

        public Tuple<string, double> findBestSplitString()
        {
            sortTable();
            ClassCounter allTable = new ClassCounter(this);
            string splitValue = attributes[0].getValueAsString();
            double bestGini = 999;
            double currentGini;

            for (int i = 0; i < attributes.Count - 1; i++)
            {
                if (!attributes[i].getValueAsString().Equals(attributes[i + 1].getValueAsString()))
                {
                    var tuple = splitTableString(attributes[i].getValueAsString());
                    currentGini = rateSplit(tuple.Item1, tuple.Item2, allTable);
                    if (currentGini < bestGini)
                    {
                        bestGini = currentGini;
                        splitValue = attributes[i].getValueAsString();
                    }
                }
            }

            if(!attributes[attributes.Count-1].getValueAsString().Equals(attributes[attributes.Count-2].getValueAsString()))
            {
                var tuple = splitTableString(attributes[attributes.Count - 1].getValueAsString());
                currentGini = rateSplit(tuple.Item1, tuple.Item2, allTable);
                if (currentGini < bestGini)
                {
                    bestGini = currentGini;
                    splitValue = attributes[attributes.Count - 1].getValueAsString();
                }
            }
            
            Tuple<string, double> toReturn = new Tuple<string, double>(splitValue, bestGini);
            return toReturn;
        }

        public void printTable()
        {
            Console.WriteLine("Is numerical: " + isNumerical);
            foreach (var v in attributes) Console.WriteLine(v.getValueAsString() + " " + v.AttributeClass);
        }

    }
}
