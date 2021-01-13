using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class Program
    {
        static void Main(string[] args)
        {
            string pulpit = "D:\\Studia\\MED\\Projekt\\Git\\MED\\data\\"; // Paweu: C:\\Users\\user\\Desktop\\
            char separator = ';';
            SprintClassification(pulpit, "Decision_Tree_klasy",pulpit,"klasySprint", separator); //separator?
            Console.ReadKey();
        }


        static void KNNClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';', int k = 3)
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            KNN knn = new KNN(trainingSet, k);
            foreach (var v in dataSet.DataValues) knn.setClass(v);
            dataSet.saveToFile(dataPath, dataName, separator);
        }

        static void BayesClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';')
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            NaiveBayes bayes = new NaiveBayes(trainingSet);
            foreach (var v in dataSet.DataValues) bayes.findMostLikelyClass(v);
            dataSet.saveToFile(dataPath, dataName, separator);
        }

        static void SprintClassification(string treePath, string treeName, string dataPath, string dataName, char separator = ';')
        {
            DecisionTree tree = new DecisionTree();
            tree.loadFromTxt(treePath + "\\" + treeName + ".txt");
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            foreach (var v in dataSet.DataValues) tree.classifyTuple(v);
            dataSet.saveToFile(dataPath, dataName, separator);
        }

        static void createTree(string trainingPath, string trainingName, char separator)
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            Sprint sprint = new Sprint(trainingSet);
            DecisionTree tree = sprint.createTree();
            tree.saveToTxt(trainingPath, "Decision_Tree_" + trainingName);
        }
    }
}
