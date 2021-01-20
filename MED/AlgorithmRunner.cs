using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class AlgorithmRunner
    {
        public static int MAX_THREADS = 4;
        private static int current_threads = 1;
        public static Object semaphore = new object();

        public static int getThreads()
        {
            int toReturn;
            System.Threading.Monitor.Enter(semaphore);
            toReturn = current_threads;
            System.Threading.Monitor.Exit(semaphore);
            return toReturn;
        }

        public static void increaseThreads()
        {
            System.Threading.Monitor.Enter(semaphore);
            current_threads++;
            System.Threading.Monitor.Exit(semaphore);
        }

        public static void decreaseThreads()
        {
            System.Threading.Monitor.Enter(semaphore);
            current_threads--;
            System.Threading.Monitor.Exit(semaphore);
        }

        public void KNNClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';', int k = 3)
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, true);
            Globals.stopWatch.Start();
            KNN knn = new KNN(trainingSet, k);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Training data set");
            Globals.stopWatch.Start();
            runKnnInThreads(knn, dataSet);
            //foreach (var v in dataSet.DataValues) knn.setClass(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Test data set");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
        }

        public void BayesClassification(string trainingPath, string trainingName, string dataPath, string dataName, char separator = ';')
        {
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, true);
            Globals.stopWatch.Start();
            NaiveBayes bayes = new NaiveBayes(trainingSet);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Training data set");
            Globals.stopWatch.Start();
            runBayesInThreads(bayes, dataSet);
            //foreach (var v in dataSet.DataValues) bayes.findMostLikelyClass(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Test data set");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
        }

        public void SprintClassification(string treePath, string treeName, string dataPath, string dataName, char separator = ';')
        {
            DecisionTree tree = new DecisionTree();
            Globals.stopWatch.Start();
            tree.loadFromTxt(treePath + "\\" + treeName + ".txt");
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Loading tree");
            DataSet dataSet = new DataSet(dataPath + "\\" + dataName + ".txt", separator, true);
            Globals.stopWatch.Start();
            runTreeInThreads(tree, dataSet);
            //foreach (var v in dataSet.DataValues) tree.classifyTuple(v);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Classifying");
            Globals.stopWatch.Start();
            dataSet.saveToFile(dataPath, dataName, separator);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving result to file");
        }

        public void createTree(string trainingPath, string trainingName, char separator)
        {
            Globals.stopWatch.Start();
            DataSet trainingSet = new DataSet(trainingPath + "\\" + trainingName + ".txt", separator, true);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Training data set");
            Globals.stopWatch.Start();
            Sprint sprint = new Sprint(trainingSet);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Sprint created");
            Globals.stopWatch.Start();
            DecisionTree tree = sprint.createTree();
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Tree created");
            Globals.stopWatch.Start();
            tree.saveToTxt(trainingPath, "Decision_Tree_" + trainingName);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Saving tree to file");
        }

        private void runBayesInThreads(NaiveBayes bayes, DataSet data)
        {
            int tuplesInThread = data.DataValues.Count / MAX_THREADS;
            int from = 0;

            List<System.Threading.Thread> tasks = new List<System.Threading.Thread>();
            for (int i = 0; i < MAX_THREADS - 1; i++)
            {
                System.Threading.Monitor.Enter(semaphore);
                Object fr = new object();
                fr = from;
                System.Threading.Thread thread = new System.Threading.Thread(delegate () {
                    runBayes(data, bayes, (int)fr, (int)fr + tuplesInThread);
                });
                tasks.Add(thread);
                thread.Start();
                from += tuplesInThread;
                System.Threading.Monitor.Exit(semaphore);
            }

            System.Threading.Thread lastThread = new System.Threading.Thread(delegate () {
                runBayes(data, bayes, from, data.DataValues.Count);
            });
            tasks.Add(lastThread);
            lastThread.Start();

            foreach (var temp in tasks)
            {
                temp.Join();
            }
        }

        private void runKnnInThreads(KNN knn, DataSet data)
        {
            int tuplesInThread = data.DataValues.Count / MAX_THREADS;
            int from = 0;
            List<int> froms = new List<int>();
            froms.Add(0);
            for (int i = 1; i < MAX_THREADS; i++)
            {
                froms.Add(froms[i - 1] + tuplesInThread);
            }
            foreach (var v in froms) Console.WriteLine(v);

            List<System.Threading.Thread> tasks = new List<System.Threading.Thread>();
            for (int i = 0; i < MAX_THREADS - 1; i++)
            {
                System.Threading.Monitor.Enter(semaphore);
                Object fr = new object();
                fr = from;
                System.Threading.Thread thread = new System.Threading.Thread(delegate () {
                    runKnn(data, knn, (int)fr, (int)fr + tuplesInThread);
                });
                tasks.Add(thread);
                thread.Start();
                from += tuplesInThread;
                System.Threading.Monitor.Exit(semaphore);
            }

            System.Threading.Thread lastThread = new System.Threading.Thread(delegate () {
                runKnn(data, knn, from, data.DataValues.Count);
            });
            tasks.Add(lastThread);
            lastThread.Start();

            foreach (var temp in tasks)
            {
                temp.Join();
            }
        }

        private void runTreeInThreads(DecisionTree tree, DataSet data)
        {
            int tuplesInThread = data.DataValues.Count / MAX_THREADS;
            int from = 0;
            List<int> froms = new List<int>();
            froms.Add(0);
            for (int i = 1; i < MAX_THREADS; i++)
            {
                froms.Add(froms[i - 1] + tuplesInThread);
            }
            foreach (var v in froms) Console.WriteLine(v);

            List<System.Threading.Thread> tasks = new List<System.Threading.Thread>();
            for (int i = 0; i < MAX_THREADS - 1; i++)
            {
                System.Threading.Monitor.Enter(semaphore);
                Object fr = new object();
                fr = from;
                System.Threading.Thread thread = new System.Threading.Thread(delegate () {
                    runTree(data, tree, (int)fr, (int)fr + tuplesInThread);
                });
                tasks.Add(thread);
                thread.Start();
                from += tuplesInThread;
                System.Threading.Monitor.Exit(semaphore);
            }

            System.Threading.Thread lastThread = new System.Threading.Thread(delegate () {
                runTree(data, tree, from, data.DataValues.Count);
            });
            tasks.Add(lastThread);
            lastThread.Start();

            foreach (var temp in tasks)
            {
                temp.Join();
            }
        }

        private void runBayes(DataSet data, NaiveBayes bayes, int from, int to)
        {
            data.clasifyBayesThread(bayes, from, to);
        }

        private void runKnn(DataSet data, KNN knn, int from, int to)
        {
            data.clasifyKnnThread(knn, from, to);
        }

        private void runTree(DataSet data, DecisionTree tree, int from, int to)
        {
            data.clasifyTreeThread(tree, from, to);
        }

    }
}
