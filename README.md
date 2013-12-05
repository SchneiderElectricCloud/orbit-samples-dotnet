# orbit-samples-dotnet

This repository contains the Orbit Developer Samples for .NET (C# source code).

## Instructions
1. Optionally fork the repository
2. Clone the repository or download the .zip file
3. Open the DeveloperSamples.sln in Visual Studio 2012 (or greater)
4. In the Solution Explorer, right click on Solution 'DeveloperSamples' and select "Manage NuGet Packages for Solution...".
	- If you see a message at the top of "Manage NuGet Packages" dialog that says: "Some NuGet packages are missing from this solution. Click to restore from your online package sources", click the "Restore" button. This will download any missing packages from NuGet.
	- Alternatively, you can just build twice: the first build will trigger NuGet package restore, and again to build the solution.

## Requirements
1. Visual Studio 2012 or 2013 (Express version should work fine)
2. Requires .NET Framework 4.5 (or greater)
3. Requires a Schneider Electric Orbit account

## Samples

### Sample Orbit Event Listener Service
Provides an example of listening for and reacting to Orbit task events. See SampleOrbitEventListenerService/README.md for additional details.