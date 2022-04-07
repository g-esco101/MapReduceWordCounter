# Map Reduce Word Counter

A service-oriented web application & the SOAP services that collectively count the number of words in a text file via MapReduce. The user uploads a text file (the file is not saved) & has the option to input the URLs of other services to perform the tasks or to use the default services. It binds to the services dynamically. To ensure that the tasks are performed in simultaneously, multithreading is implemented: each thread receives a partition of the words in the text file & invokes the web services. The web services are configured such that a new instance is created per call. It mimics the Hadoop process by distributing the data & the processing over a network.

It uses ASP.NET - C#.


## To run...

Download or clone the MapReduceWordCounterAWS repository

Open it using Visual Studio by double clicking the MapReduceWordCounter.sln file

Under the CombineService project, right click the Service1.svc file & select 'View in Browser'

Under the MapService project, right click the Service1.svc file & select 'View in Browser'

Under the ReduceService project, right click the Service1.svc file & select 'View in Browser'

Right click the MapReduceWordCounter project & select 'Set as StartUp Project'

Select the 'Debug' tab & then select 'Start Without Debugging'
