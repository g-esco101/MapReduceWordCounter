using System;
using System.Reflection;
using System.ServiceModel.Description;
using DynamicProxyLibrary;
using System.Runtime.Serialization;
using System.Xml;

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
                // Set binding limits
                var binding = ep.Binding as System.ServiceModel.BasicHttpBinding;
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.MaxBufferPoolSize = 2147483647;
                XmlDictionaryReaderQuotas myReaderQuotas = new XmlDictionaryReaderQuotas();
                myReaderQuotas.MaxStringContentLength = 2147483647;
                myReaderQuotas.MaxNameTableCharCount = 2147483647;
                myReaderQuotas.MaxArrayLength = 2147483647;
                myReaderQuotas.MaxBytesPerRead = 2147483647;
                myReaderQuotas.MaxDepth = 64;
                binding.GetType().GetProperty("ReaderQuotas").SetValue(binding, myReaderQuotas, null);

                DynamicProxy proxy = proxyFactory.CreateProxy(ep);
                serviceInstance = proxy.ObjectInstance;
            }
            return serviceInstance;
        }
    }
}