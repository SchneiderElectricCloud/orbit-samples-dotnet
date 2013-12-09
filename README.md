# orbit-samples-dotnet

This repository contains the Orbit Developer Samples for .NET (C# source code).

## Instructions
1. Optionally fork the repository.
2. Clone the repository or download the .zip file.
3. Open the DeveloperSamples.sln file in Visual Studio 2012 or later.
4. In the Solution Explorer pane, right-click the solution node (Solution 'DeveloperSamples') and select "Manage NuGet Packages for Solution...".
	- You may receive a message at the top of the Manage NuGet Packages dialog asking you to restore missing packages. If so, click the Restore button to have Visual Studio download any missing packages from NuGet.
	- Alternatively, you can build the solution twice. The first build will trigger NuGet package restore. The second attempt will build the solution.

## Requirements
1. Visual Studio 2012 or 2013 (Express or paid versions).
2. .NET Framework 4.5 or later.
3. A <a href="https://infrastructurecommunity.schneider-electric.com/community/products/orbit">Schneider Electric Orbit account</a>.

## Samples

### Sample Orbit Event Listener Service
Provides an example of listening for and reacting to Orbit task events. See SampleOrbitEventListenerService/README.md for additional details.