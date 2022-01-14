using System.Collections.Generic;
using System.ServiceModel;

namespace MapService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class Service1 : IService1
    {
        // Maps an array of words to a dictionary where
        // each word is a key & the value is the number of times the word occurs in wordsArray. 
        public IDictionary<string, int> MapFunction(string[] wordsArray)
        {
            IDictionary<string, int> mapReturn = new Dictionary<string, int>();
            foreach (string word in wordsArray)
            {
                if (mapReturn.ContainsKey(word))
                {
                    mapReturn[word] = mapReturn[word] + 1;
                }
                else mapReturn.Add(word, 1);
            }
            return mapReturn;
        }
    }
}
