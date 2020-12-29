using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class NaiveBayes
    {
        private List<string> headers;
        private List<AttributeCounter> counters;
        private List<string> classes;

        public List<string> Headers { get { return headers; } set { headers = value; } }
        public List<AttributeCounter> Counters { get { return counters; } set { counters = value; } }
        public List<string> Classes { get { return classes; } set { classes = value; } }

        public NaiveBayes(DataSet dataSet)
        {
            Headers = new List<string>();
            Classes = new List<string>();
            Counters = new List<AttributeCounter>();
            foreach (var v in dataSet.Headers)
            {
                Headers.Add(v);
                AttributeCounter toAdd = new AttributeCounter();
                Counters.Add(toAdd);
            }
            Headers.Add("Class");
            Counters.Add(new AttributeCounter());
            
            foreach (var v in dataSet.DataValues)
            {
                for (int i = 0; i < v.Attributes.Count; i++) Counters[i].addAttribute(v.Attributes[i].getValueAsString(), v.DataClass);
                Counters[Counters.Count - 1].addAttribute(v.DataClass, v.DataClass);
            }
            Counters[Counters.Count - 1].DiffValues = Counters[Counters.Count - 1].Attributes.Count;
            foreach (var v in Counters[Counters.Count - 1].Attributes) Classes.Add(v.Name);
        }

        public void printCounters()
        {
            for (int i = 0; i < Headers.Count; i++)
            {
                Counters[i].printAttribute(Headers[i]);
            }
        }

        public double checkProbabilityForClass(AnalyzedData analyzedData, string analyzedClass)
        {
            double result = 1;
            double classDenominator = 0;
            double classNominator = Counters[Counters.Count - 1].returnNumberForOneClass(analyzedClass, analyzedClass);
            foreach (var v in Counters[Counters.Count - 1].Attributes) classDenominator += v.Number;
            for(int i = 0; i < analyzedData.Attributes.Count; i++)
            {
                double nominator = Counters[i].returnNumberForOneClass(analyzedData.Attributes[i].getValueAsString(), analyzedClass) + 1;
                double denominator = classNominator + Counters[i].DiffValues;
                result = result * nominator / denominator;
            }
            result = result * classNominator / classDenominator;
            return result;
        }

        public void findMostLikelyClass(AnalyzedData analyzedData)
        {
            string bestClass = Classes[0];
            double bestProbability = 0;
            foreach (var c in Classes)
            {
                double r = checkProbabilityForClass(analyzedData, c);
                if(r > bestProbability)
                {
                    bestProbability = r;
                    bestClass = c;
                }
            }
            analyzedData.DataClass = bestClass;
        }

    }
}
