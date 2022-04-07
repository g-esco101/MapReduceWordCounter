# Map Reduce Word Counter

A service-oriented web application & the SOAP services that collectively count the number of words in a text file via MapReduce. It mimics the Hadoop process by distributing the data & the processing over a network. The user uploads a text file (the file is not saved) & has the option to input the URLs of other web services to perform the tasks or to use the default web services. It binds to the web services dynamically. 

To ensure that the tasks are performed simultaneously, multithreading is implemented: each thread receives a partition of the words in the text file, invokes the map service to count the frequency of each word, and then invokes the reduce service to count all the words from the resulting map service. A ConcurrentDictionary is used to track the 
results of each thread. Finally, the combine service is invoked to sum up the results of each counted partition by a thread. The web services are configured such that a new instance is created per call. 

## Technoloy

- ASP.NET - C#
- Visual Studio

## To run...

Download or clone the MapReduceWordCounter repository

```
git clone https://github.com/g-esco101/MapReduceWordCounter.git
```

Open it using Visual Studio by double clicking the MapReduceWordCounter.sln file

Under the CombineService project, right click the Service1.svc file & select 'View in Browser'

Under the MapService project, right click the Service1.svc file & select 'View in Browser'

Under the ReduceService project, right click the Service1.svc file & select 'View in Browser'

Right click the MapReduceWordCounter project & select 'Set as StartUp Project'

Select the 'Debug' tab & then select 'Start Without Debugging'
