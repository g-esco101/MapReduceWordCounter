using System;
using System.Reflection;
using System.ServiceModel.Description;
using DynamicProxyLibrary;

namespace MapReduceWordCounter
{
    public class ServiceInstantiation
    {
        // Returns the methods of a service instance. 
        public string[] getMethodNames(object serviceInstance)
        {
            string names = " ";
            MethodInfo[] methodInformation = serviceInstance.GetType().GetMethods();
            foreach (MethodInfo info in methodInformation)
            {
                if (info.Name == "get_ChannelFactory")
                {
                    break;
                }
                names += info.Name + ' ';
            }
            names = names.Trim();
            return names.Split(' ');
        }

        // Given the URL of the WSDL, creates & returns an instance of the service. Note each endpoint is a method & these services only contain one method.
        public object instantiateService(string wsdlUrl)
        {
            object serviceInstance = null;
            DynamicProxyFactory proxyFactory = new DynamicProxyFactory(wsdlUrl);
            foreach (ServiceEndpoint ep in proxyFactory.Endpoints)
            {
                DynamicProxy proxy = proxyFactory.CreateProxy(ep);
                serviceInstance = proxy.ObjectInstance;
            }
            return serviceInstance;
        }
    }
}