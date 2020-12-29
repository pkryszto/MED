using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class KNN
    {
        private List<MinMaxKNN> minMax;
        private int k;
        private DataSet dataInHyperspace;
        private AttributeCounter classesCounter;

        public KNN(DataSet data, int kn)
        {
            dataInHyperspace = data;
            k = kn;
            minMax = new List<MinMaxKNN>();
            classesCounter = new AttributeCounter();
            setMinMaxValuesAndCountClasses();
        }

        private void setMinMaxValuesAndCountClasses()
        {
            foreach(var v in dataInHyperspace.DataValues[0].Attributes)
            {
                MinMaxKNN toAdd = new MinMaxKNN(v.getValueAsDouble(),v.getValueAsDouble());
                minMax.Add(toAdd);
            }

            foreach(var v in dataInHyperspace.DataValues)
            {
                for(int i = 0; i < v.Attributes.Count; i++) minMax[i].compareX(v.Attributes[i].getValueAsDouble());
                classesCounter.addAttribute(v.DataClass, v.DataClass);
            }
        }

        private List<AnalyzedData> findNeighbours(AnalyzedData tuple)
        {
            List<Tuple<AnalyzedData, double>> dataList = new List<Tuple<AnalyzedData, double>>();
            foreach(var v in dataInHyperspace.DataValues)
            {
                dataList.Add(new Tuple<AnalyzedData, double>(v, tuple.getDistance(v, minMax)));
            }

            dataList = dataList.OrderBy(x => x.Item2).ToList();

            List<AnalyzedData> toReturn = new List<AnalyzedData>();
            for (int i = 0; i < k; i++) toReturn.Add(dataList[i].Item1);
            return toReturn;
        }

        private string doVoting(List<AnalyzedData> data)
        {
            AttributeCounter counter = new AttributeCounter();
            foreach (var v in data) counter.addAttribute(v.DataClass, v.DataClass);
            List<Tuple<string, double>> votes = new List<Tuple<string, double>>();
            foreach (var v in counter.Attributes)
            {
                double x = (double)v.Number / (double)classesCounter.returnNumberForOneClass(v.Name, v.Name);
                Tuple<string, double> toAdd = new Tuple<string, double>(v.Name, x);
                votes.Add(toAdd);
            }
            votes = votes.OrderByDescending(x => x.Item2).ToList();
            return votes[0].Item1;
        }

        public void setClass(AnalyzedData data)
        {
            var neighbours = findNeighbours(data);
            data.DataClass = doVoting(neighbours);
        }

    }
}
