using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    public class AnalyzedData
    {
        private int rid;
        private List<DataAttribute> attributes;
        private string dataClass;

        public int Rid { get { return rid; } set { rid = value; } }
        public List<DataAttribute> Attributes { get { return attributes; } set { attributes = value; } }
        public string DataClass { get { return dataClass; } set { dataClass = value; } }

        public AnalyzedData(int setRid, List<DataAttribute> setAttributes, string setClass = null)
        {
            Rid = setRid;
            Attributes = setAttributes;
            DataClass = setClass;
        }

        public AnalyzedData(int setRid, string[] readLine, List<string> headers, bool hasLabel)
        {
            Rid = setRid;
            DataClass = null;
            int lab = 0;
            if (hasLabel)
            {
                DataClass = readLine[readLine.Length - 1].Trim();
                lab = 1;
            }
            List<DataAttribute> setAttributes = new List<DataAttribute>();
            for(int i = 0; i < readLine.Length - lab; i++)
            {
                double a;
                if(Double.TryParse(readLine[i].Trim().Replace('.', ','), out a))
                {
                    NumericalDataAttribute toAdd = new NumericalDataAttribute(headers[i], a, DataClass);
                    setAttributes.Add(toAdd);
                }
                else
                {
                    StringDataAttribute toAdd = new StringDataAttribute(headers[i], readLine[i].Trim(), DataClass);
                    setAttributes.Add(toAdd);
                }
            }
            Attributes = setAttributes;
        }

        public void printData()
        {
            Console.Write(Rid + "\t");
            foreach (var s in Attributes) Console.Write(s.getValueAsString() + "\t");
            Console.WriteLine(DataClass);
        }

        public double getDistance(AnalyzedData tuple, List<MinMaxKNN> minMax)
        {
            double result = 0;
            int limit = Math.Min(Attributes.Count, Math.Min(tuple.attributes.Count, minMax.Count));
            for (int i = 0; i < limit; i++)
            {
                result += Math.Pow(Attributes[i].getDistance(tuple.Attributes[i], minMax[i].MinValue, minMax[i].MaxValue),2);
            }
            return Math.Sqrt(result);
        }


    }
}
