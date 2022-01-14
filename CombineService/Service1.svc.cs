using System.Collections.Generic;
using System.ServiceModel;


namespace CombineService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class Service1 : IService1
    {
        // Adds the values of a dictionary where the key is the thread
        // id & the value is the total number of word occurrences. Returns the sum of all the values.
        public int CombineFunction(IDictionary<string, int> reduceOutput)
        {
            int sum = 0;
            foreach (var element in reduceOutput)
            {
                sum += element.Value;
            }
            return sum;
        }
    }
}
