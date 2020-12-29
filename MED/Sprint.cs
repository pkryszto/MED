using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class Sprint
    {
        private DataSet data;

        public DataSet Data { get { return data; } set { data = value; } }

        public Sprint(DataSet dataSet)
        {
            data = dataSet;
        }

        public DecisionTree createTree()
        {
            if(data.checkIfPlain())
            {
                DecisionTree toReturn = new DecisionTree(0, "-1", "-1", data.DataValues[0].DataClass);
                return toReturn;
            }

            Tuple<int, string> split = data.findBestSplit();
            string nodeSign;
            if (data.DataValues[0].Attributes[split.Item1].isNumerical()) nodeSign = "<=";
            else nodeSign = "=";
            DecisionTree node = new DecisionTree(split.Item1, nodeSign, split.Item2);

            Tuple<DataSet, DataSet> newTables = data.splitTable(split.Item1, split.Item2);
            Sprint left = new Sprint(newTables.Item1);
            Sprint right = new Sprint(newTables.Item2);

            node.addChild('l', left.createTree());
            node.addChild('r', right.createTree());

            return node;
        }
        
    }
}
