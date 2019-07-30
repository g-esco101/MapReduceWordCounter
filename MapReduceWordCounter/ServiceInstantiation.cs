﻿using System;
using System.Reflection;
using System.ServiceModel.Description;
using DynamicProxyLibrary;

namespace MapReduceWordCounter
{
    public class ServiceInstantiation
    {
        // Returns the methods of a service instance. Similar to pages 167 & 168 in Service-Oriented Computing and System Integration 6th ed by Yinong Chen.
        public string[] getMethodNames(object serviceInstance)
        {
            string names = " ";
            try
            {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ServiceInstantiation.getMethodNames ex.Message is " + ex.Message);
            }
            return names.Split(' ');
        }

        // Given the URL of the WSDL, creates & returns an instance of the service. Note each endpoint is a method & these services only contain one method.
        // Similar to page 168 in Service-Oriented Computing and System Integration 6th ed by Yinong Chen.
        public object instantiateService(string wsdlUrl)
        {
            object serviceInstance = null;
            try
            {
                DynamicProxyFactory proxyFactory = new DynamicProxyFactory(wsdlUrl);
                foreach (ServiceEndpoint ep in proxyFactory.Endpoints)
                {
                    DynamicProxy proxy = proxyFactory.CreateProxy(ep);
                    serviceInstance = proxy.ObjectInstance;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ServiceInstantiation.instantiateService ex.Message is " + ex.Message);
            }
            return serviceInstance;
        }
    }
}