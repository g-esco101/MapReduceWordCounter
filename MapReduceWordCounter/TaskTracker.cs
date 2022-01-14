using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MapReduceWordCounter
{
    public class TaskTracker
    {
        // Dynamically binds to map service. 
        // Given the URL of the map service's wsdl & an array of strings, it returns IDictionary<string, int> where string is a word in wordArray
        // & int is the number of times that word appears in the array. 
        public IDictionary<string, int> Map(string wsdlUri, string[] wordArray)
        {
            System.Diagnostics.Debug.WriteLine("Thread id - Map - " + Thread.CurrentThread.ManagedThreadId.ToString());
            ServiceInstantiation serviceIntantiator = new ServiceInstantiation();
            IDictionary<string, int> mapReturn = new Dictionary<string, int>();
            MethodInfo methodInformation = null;
            object serviceInstance = null;
            string[] methodNames = null;
            Object[] parameters = new Object[1];
            parameters[0] = wordArray;
            try
            {
                serviceInstance = serviceIntantiator.instantiateService(wsdlUri);
                methodNames = serviceIntantiator.getMethodNames(serviceInstance);
                methodInformation = serviceInstance.GetType().GetMethod(methodNames[0]);
                mapReturn = (IDictionary<string, int>)methodInformation.Invoke(serviceInstance, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TaskTracker - Map - exception message: " + ex.Message);

                // If the service that the user inputs fails, use default service.
//                wsdlUri = "http://localhost:64890/Service1.svc";
//                serviceInstance = serviceIntantiator.instantiateService(wsdlUri);
//                methodNames = serviceIntantiator.getMethodNames(serviceInstance);
//                methodInformation = serviceInstance.GetType().GetMethod(methodNames[0]);
//                mapReturn = (IDictionary<string, int>)methodInformation.Invoke(serviceInstance, parameters);

            }
            return mapReturn;
        }

        // Dynamically binds to reduce service.
        // Given the URL of the reduce service's wsdl & an array of strings, it returns IDictionary<string, int> where string is a word in wordArray
        // & int is the number of times that word appears in the array. 
        public IDictionary<string, int> Reduce(string wsdlUri, IDictionary<string, int> wordCountDictionary)
        {
            System.Diagnostics.Debug.WriteLine("Thread id - Reduce - " + Thread.CurrentThread.ManagedThreadId.ToString());
            ServiceInstantiation serviceIntantiator = new ServiceInstantiation();
            IDictionary<string, int> reduceReturn = new Dictionary<string, int>();
            MethodInfo methodInformation = null;
            object serviceInstance = null;
            Object[] parameters = new Object[1];
            parameters[0] = wordCountDictionary;
            try
            {
                serviceInstance = serviceIntantiator.instantiateService(wsdlUri);
                string[] methodNames = serviceIntantiator.getMethodNames(serviceInstance);
                methodInformation = serviceInstance.GetType().GetMethod(methodNames[0]);
                reduceReturn = (IDictionary<string, int>)methodInformation.Invoke(serviceInstance, parameters);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TaskTracker - Reduce - exception message: " + ex.Message);
            }
            return reduceReturn;
        }
    }
}