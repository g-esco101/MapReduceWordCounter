using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq;

namespace MapReduceWordCounter
{
    public class NameNode
    {
        protected string[] allWords;
        protected int partitions;
        protected Thread[] threadArray;
        protected ConcurrentDictionary<string, int> reduceOutput;
        protected string mapWsdlUrl;
        protected string reduceWsdlUrl;
        protected string combineWsdlUrl;

        // Constructor - stores the arguments as instance variables
        public NameNode(string mapUrl, string reduceUrl, string combineUrl, string[] allWordsArray, int partitions)
        {
            this.allWords = allWordsArray;
            this.partitions = partitions;
            this.reduceOutput = new ConcurrentDictionary<string, int>();
            this.mapWsdlUrl = mapUrl;
            this.reduceWsdlUrl = reduceUrl;
            this.combineWsdlUrl = combineUrl;
        }

        // Partitions all the words in the file into a number of arrays that is equal to the 
        // thread count & distributes those arrays to a task tracker whose operations
        // are executed in parallel via multi-threading.
        public int Allocate()
        { 
            int lastSubStringLength = 0;
            int allWordsLength = allWords.Length;
            int subStringLength = allWordsLength / partitions;
            if (allWordsLength % partitions != 0 && partitions > 1)
            {
                lastSubStringLength = allWordsLength - subStringLength * (partitions - 1);
            }
            threadArray = new Thread[partitions];
            for (int i = 0; i < partitions; i++)
            {
                // Note: Captured variables could be modified after starting the thread, because they are shared. Making these variables
                // local to each iteration of the loop ensures that each thread captures a different memory location & they are not modified.
                string[] subArray = new string[subStringLength];
                int sLength = subStringLength;
                if (i == (partitions - 1) && lastSubStringLength != 0)
                {
                    sLength = lastSubStringLength;
                    subArray = new string[lastSubStringLength];
                }
                Array.Copy(allWords, i * subStringLength, subArray, 0, sLength);
                //TaskTracker tasker = new TaskTracker();
                threadArray[i] = new Thread(() => {
                    System.Diagnostics.Debug.WriteLine("Thread id - Allocate - " + Thread.CurrentThread.ManagedThreadId.ToString());
                    TaskTracker tasker = new TaskTracker();
                    IDictionary<string, int> mapped = tasker.Map(mapWsdlUrl, subArray);
                    IDictionary<string, int> reduced = tasker.Reduce(reduceWsdlUrl, mapped);
                    foreach (var item in reduced)
                    {
                        this.reduceOutput.TryAdd(Thread.CurrentThread.ManagedThreadId.ToString(), item.Value);
                    }
                });
                threadArray[i].Start();
            }
            return CombineFunction(combineWsdlUrl);
        }

        // Dynamically binds to CombineService service. 
        // Given the URL of the CombineService's wsdl, it returns the number of words in the uploaded file.
        public int CombineFunction(string combineWsdlUrl)
        {
            for (int i = 0; i < partitions; i++)
            {
                threadArray[i].Join();
            }
            ServiceInstantiation serviceIntantiator = new ServiceInstantiation();
            object serviceInstance = serviceIntantiator.instantiateService(combineWsdlUrl);
            string[] methodNames = serviceIntantiator.getMethodNames(serviceInstance);
            MethodInfo methodInfo = serviceInstance.GetType().GetMethod(methodNames[0]);
            Object[] parameters = new Object[1];
            IDictionary<string, int> newDictionary = this.reduceOutput.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            parameters[0] = newDictionary;
            int combineReturn = (int)methodInfo.Invoke(serviceInstance, parameters);
            return combineReturn;
        }
    }
}