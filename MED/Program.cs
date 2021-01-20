using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class Globals
    {
        public static Stopwatch stopWatch = new Stopwatch();
        public static TimeSpan ts = new TimeSpan();
        public static string elapsedTime;
        public static void printElapsedTimeResetWatch(TimeSpan ts, string activity)
        {
            Globals.elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Console.WriteLine(activity + " - finished in time: " + Globals.elapsedTime + " (hours:minutes:seconds)");
            Globals.stopWatch.Reset();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AlgorithmRunner runner = new AlgorithmRunner();
            string pulpit = "C://Users//user//Desktop//Nowe_dane//";
            char separator = ',';
            //runner.createTree(pulpit, "NSLKDD_train2", separator);
            //runner.SprintClassification(pulpit, "Decision_Tree_NSLKDD_train2", pulpit, "NSLKDD_test_sprint2", separator);
            runner.KNNClassification(pulpit, "NSLKDD_train", pulpit, "NSLKDD_test_k", separator, 3);
            //runner.BayesClassification(pulpit, "NSLKDD_train", pulpit, "NSLKDD_test_b3", separator);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}
