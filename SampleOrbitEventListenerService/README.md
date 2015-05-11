# Sample Orbit Event Listener Service #

## Overview ##

This sample is intended to demonstrate how to develop a Windows Service that listens and reacts to ArcFM Mobile task events. This project is a sample only in the sense that the code is meant to be modified to meet the integration needs of each particular use-case; however, it is written and follows practices with the intention of being production ready.

This service can be deployed on-premises allowing integration with on-premises resources and services.

## How To Customize ##

1. Retrieve the developer sample from GitHub
	a. If you have Git installed, you can clone the repository
	b. Otherwise, [download a .zip](https://github.com/SchneiderElectricCloud/orbit-samples-dotnet/archive/master.zip "Click to download the latest source") containing the latest code and extract the contents
2. Open the Visual Studio Solution file (DeveloperSamples.sln)
3. Update the App.config appSettings
	```xml
	<appSettings>
	  <add key="Microsoft.ServiceBus.ConnectionString" value="[SB-CONN-STRING]" />
	  <add key="Orbit.Events.TopicName" value="taskservices-[TENANT-ID]" />
	  <add key="Orbit.Events.SubscriptionName" value="default" />
	
	  <add key="ApiKey" value="[ORBIT_APIKEY]" />
	</appSettings>
	```

	- ArcFM Mobile uses Azure ServiceBus topics & subscriptions (publish/subscribe) feature to publish events regarding task events. A ServiceBus connection string is required to listen to these events. These settings are tenant-specific and you must contact the ArcFM Mobile team for these settings:
		- Microsoft.ServiceBus.ConnectionString
		- Orbit.Events.TopicName
		- Orbit.Events.SubscriptionName
	- The easiest way to authenticate with ArcFM Mobile from an unattended application (like a Windows Service) is using API Keys. Assuming you have an ArcFM Mobile account, you can create an ApiKey in the [Administration](https://orbit.schneider-electric.com/Admin "Click to open...") page of the ArcFM Mobile web portal.
		- ApiKey

4. The MessageHandlers folder contains various sample message handlers. You can create new message handlers or modify one of the samples to match your needs. You can delete the message handlers you don't need (or leave them, if you'd prefer).
	- The CreatePoleInspectionHandler demonstrates how to react to the "TaskCompleted" event and create a related record via ArcGIS Server.

## How It Works ##

todo


