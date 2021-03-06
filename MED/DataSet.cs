﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class DataSet
    {
        private List<string> headers;
        private List<AnalyzedData> dataValues;

        public List<string> Headers { get { return headers; } set { headers = value; } }
        public List<AnalyzedData> DataValues { get { return dataValues; } set { dataValues = value; } }

        public DataSet(string path, char splitter, bool hasLabel)
        {
            Headers = new List<string>();
            DataValues = new List<AnalyzedData>();
            int rid = 1;
            try
            {
                using (var sr = new StreamReader(path))
                {
                    string headersLine = sr.ReadLine();
                    var headersNames = headersLine.Split(' ');
                    for (int i = 0; i < headersNames.Length - 1; i++) Headers.Add(headersNames[i]);
                    string newLine;
                    while (true)
                    {
                        newLine = sr.ReadLine();
                        if (newLine == null) break;
                        var attributes = newLine.Split(splitter);
                        AnalyzedData toAdd = new AnalyzedData(rid, attributes, Headers, hasLabel);
                        DataValues.Add(toAdd);
                        rid++;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public DataSet(List<string> h, List<AnalyzedData> d)
        {
            headers = h;
            dataValues = d;
        }

        private Tuple<DataSet, DataSet> splitTableByStringAttribute(int number, string att)
        {
            int a = 0;
            int b = 0;
            for (int i = 0; i < DataValues.Count; i++)
            {
                if (DataValues[i].Attributes[number].getValueAsString().Equals(att))
                {
                    a = i;
                    break;
                }
            }

            for (int i = a; i < DataValues.Count; i++)
            {
                if (DataValues[i].Attributes[number].getValueAsString().Equals(att)) b++;
                else break;
            }

            List<AnalyzedData> leftList = new List<AnalyzedData>();
            List<AnalyzedData> rightList = new List<AnalyzedData>();

            rightList.AddRange(DataValues.GetRange(0, a));
            leftList.AddRange(DataValues.GetRange(a, b));
            rightList.AddRange(DataValues.GetRange(a + b, DataValues.Count - a - b));

            DataSet leftSet = new DataSet(headers, leftList);
            DataSet rightSet = new DataSet(headers, rightList);

            Tuple<DataSet, DataSet> toReturn = new Tuple<DataSet, DataSet>(leftSet, rightSet);
            return toReturn;
        }

        private Tuple<DataSet, DataSet> splitTableByNumericalAttribute(int number, double att)
        {
            int a = 0;
            for (int i = 0; i < DataValues.Count; i++)
            {
                if (DataValues[i].Attributes[number].getValueAsDouble() <= att) a++;
                else break;
            }

            List<AnalyzedData> leftList = new List<AnalyzedData>();
            List<AnalyzedData> rightList = new List<AnalyzedData>();

            leftList.AddRange(DataValues.GetRange(0, a));
            rightList.AddRange(DataValues.GetRange(a, DataValues.Count - a));

            DataSet leftSet = new DataSet(headers, leftList);
            DataSet rightSet = new DataSet(headers, rightList);

            Tuple<DataSet, DataSet> toReturn = new Tuple<DataSet, DataSet>(leftSet, rightSet);
            return toReturn;
        }

        public Tuple<DataSet, DataSet> splitTable(int number, string att)
        {
            if (dataValues[0].Attributes[number].isNumerical())
            {
                dataValues = dataValues.OrderBy(x => x.Attributes[number].getValueAsDouble()).ToList();
                return splitTableByNumericalAttribute(number, Double.Parse(att));
            }

            else
            {
                dataValues = dataValues.OrderBy(x => x.Attributes[number].getValueAsString()).ToList();
                return splitTableByStringAttribute(number, att);
            }
        }

        private List<AttributeTable> createTables()
        {
            List<AttributeTable> attributes = new List<AttributeTable>();
            List<List<DataAttribute>> att = new List<List<DataAttribute>>();
            foreach (var v in Headers) att.Add(new List<DataAttribute>());
            foreach (var v in DataValues)
            {
                for (int i = 0; i < v.Attributes.Count; i++) att[i].Add(v.Attributes[i]);
            }

            for (int i = 0; i < att.Count; i++) attributes.Add(new AttributeTable(att[i], DataValues[0].Attributes[i].isNumerical()));
            return attributes;
        }

        public Tuple<int, string> findBestSplit()
        {
            List<AttributeTable> attributes = createTables();
            List<Tuple<string, double>> ginies = new List<Tuple<string, double>>();

            List<List<Tuple<string, double>>> subGinies = new List<List<Tuple<string, double>>>();
            for (int i = 0; i < AlgorithmRunner.MAX_THREADS; i++) subGinies.Add(new List<Tuple<string, double>>());

            int from = 0;
            int attributesInThread = attributes.Count / AlgorithmRunner.MAX_THREADS;

            List<System.Threading.Thread> tasks = new List<System.Threading.Thread>();
            for (int i = 0; i < AlgorithmRunner.MAX_THREADS - 1; i++)
            {
                System.Threading.Monitor.Enter(AlgorithmRunner.semaphore);
                Object fr = new object();
                Object iMon = new object();
                iMon = i;
                fr = from;
                System.Threading.Thread thread = new System.Threading.Thread(delegate () {
                    findBestSplitThread(subGinies[(int)iMon], attributes, (int)fr, (int)fr + attributesInThread);
                });
                tasks.Add(thread);
                thread.Start();
                from += attributesInThread;
                System.Threading.Monitor.Exit(AlgorithmRunner.semaphore);
            }
            System.Threading.Thread lastThread = new System.Threading.Thread(delegate () {
                findBestSplitThread(subGinies.Last(), attributes, from, attributes.Count);
            });
            tasks.Add(lastThread);
            lastThread.Start();

            foreach (var temp in tasks)
            {
                temp.Join();
            }


            foreach (var v in subGinies) ginies.AddRange(v);
            //foreach (var v in attributes) ginies.Add(v.findBestSplit());

            int best = 0;

            for (int i = 0; i < ginies.Count; i++)
            {
                if (ginies[i].Item2 < ginies[best].Item2) best = i;
            }

            return new Tuple<int, string>(best, ginies[best].Item1);
        }

        public bool checkIfPlain()
        {
            for (int i = 0; i < dataValues.Count - 1; i++)
            {
                if (!dataValues[i].DataClass.Equals(dataValues[i + 1].DataClass)) return false;
            }
            return true;
        }

        public void printData()
        {
            Console.Write("Rid\t");
            foreach (var s in Headers) Console.Write(s + "\t");
            Console.WriteLine("Class");
            foreach (var s in DataValues) s.printData();
        }

        public void saveToFile(string path, string name, char separator)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, name + ".txt")))
            {
                foreach (var v in headers) outputFile.Write(v + " ");
                outputFile.WriteLine("label");
                foreach (var v in dataValues)
                {
                    foreach (var w in v.Attributes) outputFile.Write(w.getValueAsString().Replace(',', '.') + separator);
                    outputFile.WriteLine(v.DataClass);
                }
            }
        }

        public void clasifyBayesThread(NaiveBayes bayes, int from, int to)
        {
            if (to > dataValues.Count) to = dataValues.Count;
            for(int i = from; i < to; i++) bayes.findMostLikelyClass(dataValues[i]);
        }

        public void clasifyKnnThread(KNN knn, int from, int to)
        {
            if (to > dataValues.Count) to = dataValues.Count;
            for (int i = from; i < to; i++) knn.setClass(dataValues[i]);
        }

        public void clasifyTreeThread(DecisionTree tree, int from, int to)
        {
            if (to > dataValues.Count) to = dataValues.Count;
            for (int i = from; i < to; i++) tree.classifyTuple(dataValues[i]);
        }

        public void findBestSplitThread(List<Tuple<string, double>> ginies,List<AttributeTable> attributes, int from, int to)
        {
            if (to > attributes.Count) to = attributes.Count;
            for (int i = from; i < to; i++)
            {
                ginies.Add(attributes[i].findBestSplit());
            }
        }

    }
}
