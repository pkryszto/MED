using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class Globals {
        public static Stopwatch stopWatch = new Stopwatch();
        public static TimeSpan ts = new TimeSpan();
        public static string elapsedTime;
        public static void printElapsedTimeResetWatch(TimeSpan ts, string activity) {
            Globals.elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Console.WriteLine(activity + " - finished in time: " + Globals.elapsedTime + " (hours:minutes:seconds)");
            Globals.stopWatch.Reset();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string pulpit = "D:\\Studia\\MED\\Projekt\\Git\\MED\\data\\"; // Paweu: C:\\Users\\user\\Desktop\\
            char separator = ';';
            // SprintClassification(pulpit, "Decision_Tree_klasy",pulpit,"klasySprint", separator);
            // KNNClassification(pulpit, "NEW_KDD99Train_100k", pulpit, "NEW_KDD99Test_10k", separator);
            BayesClassification(pulpit, "NEW_KDDTrain+_5k", pulpit, "NEW_KDDTest+_500", separator);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        static void KNNClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';', int k = 3)
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            Globals.stopWatch.Start();
            KNN knn = new KNN(trainingSet, k);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Training data set");
            Globals.stopWatch.Start();
            foreach (var v in dataSet.DataValues) knn.setClass(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Test data set");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
        }

        static void BayesClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';')
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            Globals.stopWatch.Start();
            NaiveBayes bayes = new NaiveBayes(trainingSet);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Training data set");
            Globals.stopWatch.Start();
            foreach (var v in dataSet.DataValues) bayes.findMostLikelyClass(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Test data set");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
        }

        static void SprintClassification(string treePath, string treeName, string dataPath, string dataName, char separator = ';')
        {
            DecisionTree tree = new DecisionTree();
            Globals.stopWatch.Start();
            tree.loadFromTxt(treePath + "\\" + treeName + ".txt");
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Loading tree");
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, false);
            Globals.stopWatch.Start();
            foreach (var v in dataSet.DataValues) tree.classifyTuple(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Classifying");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
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
