using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MED
{
    public class DecisionTree
    {
        private int nodeAttribute;
        private string nodeOperator;
        private string nodeValue;
        private string leaf;
        private DecisionTree leftChild;
        private DecisionTree rightChild;

        public int NodeAttribute { get { return nodeAttribute; } set { nodeAttribute = value; } }
        public string NodeOperator { get { return nodeOperator; } set { nodeOperator = value; } }
        public string NodeValue { get { return nodeValue; } set { nodeValue = value; } }
        public string Leaf { get { return leaf; } set { leaf = value; } }

        public DecisionTree()
        {

        }

        public DecisionTree(int att, string op, string val, string cls = null)
        {
            nodeAttribute = att;
            nodeOperator = op;
            nodeValue = val;
            leaf = cls;
            leftChild = null;
            rightChild = null;
        }

        public void addChild(char s, DecisionTree child)
        {
            switch (s)
            {
                case 'l':
                    leftChild = child;
                    break;
                case 'r':
                    rightChild = child;
                    break;
            }
            return;
        }

        private bool addChild(DecisionTree toAdd)
        {
            if (leftChild == null)
            {
                leftChild = toAdd;
                return true;
            }
            else if (leftChild.Leaf == null)
            {
                bool b = leftChild.addChild(toAdd);
                if (b) return true;
                if (rightChild == null)
                {
                    rightChild = toAdd;
                    return true;
                }
                if (rightChild.Leaf != null) return false;
                return rightChild.addChild(toAdd);
            }
            else if (rightChild == null)
            {
                rightChild = toAdd;
                return true;
            }
            else if (rightChild.Leaf == null)
            {
                return rightChild.addChild(toAdd);
            }

            return false;
        }

        private bool checkCondition(string value)
        {
            if(nodeOperator.Equals("<="))
            {
                if (Double.Parse(value) <= Double.Parse(nodeValue)) return true;
                return false;
            }
            else
            {
                if (value.Equals(nodeValue)) return true;
                return false;
            }
        }

        public void classifyTuple(AnalyzedData tuple)
        {
            if(leaf != null)
            {
                tuple.DataClass = leaf;
                return;
            }

            if (checkCondition(tuple.Attributes[nodeAttribute].getValueAsString())) leftChild.classifyTuple(tuple);
            else rightChild.classifyTuple(tuple);
        }

        public void printTree()
        {
            if (leaf != null) Console.WriteLine(leaf);
            else
            {
                Console.WriteLine(nodeAttribute + " " + nodeOperator + " " + nodeValue);
                leftChild.printTree();
                rightChild.printTree();
            }
        }

        public void printTreeToFile(StreamWriter outputFile)
        {
            if (leaf != null) outputFile.WriteLine(leaf);
            else
            {
                outputFile.WriteLine(nodeAttribute + ";" + nodeOperator + ";" + nodeValue);
                leftChild.printTreeToFile(outputFile);
                rightChild.printTreeToFile(outputFile);
            }
        }
        
        public void saveToTxt(string path, string title)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, title+".txt")))
            {
                printTreeToFile(outputFile);
            }
        }

        public void loadFromTxt(string path)
        {
            try
            {
                using (var sr = new StreamReader(path))
                {
                    string newLine = sr.ReadLine();
                    var attributes = newLine.Split(';');
                    nodeAttribute = Int32.Parse(attributes[0]);
                    nodeOperator = attributes[1];
                    nodeValue = attributes[2];

                    while (true)
                    {
                        newLine = sr.ReadLine();
                        if (newLine == null) break;
                        attributes = newLine.Split(';');
                        DecisionTree toAdd;
                        if (attributes.Count() == 1) toAdd = new DecisionTree(0, "-1", "-1", attributes[0]);
                        else toAdd = new DecisionTree(Int32.Parse(attributes[0]), attributes[1], attributes[2]);
                        addChild(toAdd);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

    }
}
