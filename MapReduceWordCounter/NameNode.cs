using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace MapReduceWordCounter
{
    public class NameNode
    {
        protected string[] allWords;
        protected int threadNumber;
        protected Thread[] threadArray;
        protected IDictionary<string, int> reduceOutput;
        protected string mapWsdlUrl;
        protected string reduceWsdlUrl;
        protected string combineWsdlUrl;

        // Constructor - stores the arguments as instance variables
        public NameNode(string mapUrl, string reduceUrl, string combineUrl, string[] allWordsArray, int threadcount)
        {
            this.allWords = allWordsArray;
            this.threadNumber = threadcount;
            reduceOutput = new Dictionary<string, int>();
            this.mapWsdlUrl = mapUrl;
            this.reduceWsdlUrl = reduceUrl;
            this.combineWsdlUrl = combineUrl;
        }

        // Partitions all the words in the file into a number of arrays that is equal to the 
        // thread count & distributes those arrays to a task tracker whose operations
        // are executed in parallel via multi-threading.
        public int Allocate()
        {
            int subStringLength = 0;
            int lastSubStringLength = 0;
            int allWordsLength = 0;
            try
            {
                allWordsLength = allWords.Length;
                subStringLength = allWordsLength / threadNumber;
                if (allWordsLength % threadNumber != 0 && threadNumber > 1)
                {
                    lastSubStringLength = allWordsLength - subStringLength * (threadNumber - 1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NameNode.Allocate 1st Try ex.Message is " + ex.Message);
            }
            threadArray = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                // Note: Captured variables could be modified after starting the thread, because they are shared. Making these variables
                // local to each iteration of the loop ensures that each thread captures a different memory location & they are not modified.
                string[] subStr = new string[subStringLength];
                int sLength = subStringLength;
                try
                {
                    if (i == (threadNumber - 1) && lastSubStringLength != 0)
                    {
                        sLength = lastSubStringLength;
                        subStr = new string[lastSubStringLength];
                    }
                    Array.Copy(allWords, i * subStringLength, subStr, 0, sLength);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NameNode.Allocate 2nd Try ex.Message is " + ex.Message);
                }
                TaskTracker tasker = new TaskTracker();
                threadArray[i] = new Thread(() => {
                    IDictionary<string, int> mapReduceReturn = tasker.MapReduce(mapWsdlUrl, reduceWsdlUrl, subStr);
                    foreach (var item in mapReduceReturn)
                    {
                        reduceOutput.Add(Thread.CurrentThread.ManagedThreadId.ToString(), item.Value);
                    }
                });
                threadArray[i].Start();
            }
            return CombineFunction(combineWsdlUrl);
        }

        // Dynamically binds to CombineService service. 
        // Given the URL of the CombineService's wsdl, it returns the number of words in the uploaded file.
        public int CombineFunction(string wsdlUri)
        {
            for (int i = 0; i < threadNumber; i++)
            {
                threadArray[i].Join();
            }
            ServiceInstantiation serviceIntantiator = new ServiceInstantiation();
            int combineReturn = 0;
            object serviceInstance = serviceIntantiator.instantiateService(wsdlUri);
            MethodInfo methodInformation = null;
            try
            {
                string[] methodNames = serviceIntantiator.getMethodNames(serviceInstance);
                methodInformation = serviceInstance.GetType().GetMethod(methodNames[0]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NameNode.CombineFunction 1st Try ex.Message is " + ex.Message);
            }
            Object[] parames = new Object[1];
            parames[0] = this.reduceOutput;

            try
            {
                combineReturn = (int)methodInformation.Invoke(serviceInstance, parames);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NameNode.CombineFunction 2nd Try ex.Message is " + ex.Message);
            }
            return combineReturn;
        }
    }
}