# RealSenseMaker
Utility to update Visual Studio solutions and projects (C#) with all that they need target 32 bit and 64 bit RealSense libraries.

# How To Use It
When you run the application, you need to pick two folders. The first folder is the root folder of the RealSense SDK (RSSDK) from when you installed the SDK. As an example, I always install the folder to the following location - C:\Intel\RSSDK. The application is looking for the Bin folders directly under here so it's vital that you choose the root folder.

The second folder location must be somewhere up the folder tree from where your Visual Studio Solution file is (.sln). As the application looks for all solution files, you don't have to pick the exact level; the application will work it out by itself.

When it is processing a solution, it copies the relevant library files into a common Libs folder and then sets up the solution with 64 bit and 32 build jobs, as well as setting up the projects with the appropriate libs references for the 64 bit and 32 bit builds. It's that easy - open your solution, choose which configuration you want to build and you should be good to go.

Pete
