using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapReduceWordCounter
{
    public class TaskTracker
    {
        // Dynamically binds to map service. 
        // Given the URL of the map service's wsdl & an array of strings, it returns IDictionary<string, int> where string is a word in wordArray
        // & int is the number of times that word appears in the array. 
        public IDictionary<string, int> Map(string wsdlUri, string[] wordArray)
        {
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
            catch(Exception ex)
            {
                mapReturn.Add("MAP ERROR", -1);
            }
            return mapReturn;
        }

        // Dynamically binds to reduce service.
        // Given the URL of the reduce service's wsdl & an array of strings, it returns IDictionary<string, int> where string is a word in wordArray
        // & int is the number of times that word appears in the array. 
        public IDictionary<string, int> Reduce(string wsdlUri, IDictionary<string, int> wordCountDictionary)
        {
            IDictionary<string, int> reduceReturn = new Dictionary<string, int>();
            if (wordCountDictionary.ContainsKey("MAP ERROR"))
            {
                reduceReturn.Add("MAP ERROR", -1);
                return reduceReturn;
            }
            ServiceInstantiation serviceIntantiator = new ServiceInstantiation();
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
            catch
            {
                reduceReturn.Add("REDUCE ERROR", -2);
            }
            return reduceReturn;
        }
    }
}