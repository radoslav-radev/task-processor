# Task Processor

A clustered engine for execution and monitoring a queue of tasks.

## Features

* Task Agnostic - you can plug-in your own tasks and your code to process them.
* Clustered - multiple instances can be deployed on different machines as windows services to process a shared queue. 
* Load Balanced - tasks are distributed equally among the existing processor instances.
* Scalable - a new instance can be deployed and start immediately process the tasks in the queue; an instance can be safely shutdown - you request a shutdown so no more tasks will be assigned to it and when it completes execution of current tasks it will die.
* Extremly configurable - you can replace every module in the system with your own, for example the default algorithm for tasks distribution. This is achieved with custom Service Locator configurable in App.config file.
* Tasks are run and monitored as separate processes by the processor instance.
* StyleCop and Code Analysis compliance.
* Fully documented code + Sandcastle documentation.
* Test Driven Development + a lot of unit and integration tests.
* Used in real-world case for monitoring of employees real-time adherence.

## Technologies

* .NET Framework 4.5
* Visual Studio 2013
* Redis (Service Stack)
* Test Driven Development
