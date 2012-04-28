AdsClient
=========

This is the client implementation of the [Twincat](http://www.beckhoff.com/english.asp?twincat/default.htm) Ads protocol from [Beckhoff](http://http://www.beckhoff.com/).
(I'm not affiliated with Beckhoff)

The implementation is in C# and can be used in silverlight, metro, mono and windows phone projects.

There is a library with async and one without.

Async is a new feature and is not yet included in .Net 4.
That's why I added an async dll in the project.
For compiling this from VS2010 you need to install
http://msdn.microsoft.com/en-us/vstudio/gg316360

The NoAsync library doesn't require this dll and can be used in mono 2.10.
(It's the same source, but compiled with the NO_ASYNC directive)


Getting started
---------------

### Ads Route

First you have to give your device/machine the permission to communicate with the Twincat Ads server by adding a route.

There are different ways of doing this depending on the device.
You can use the Twincat Remote Manager for example.
On a CX9001 device you can connect with cerhost.exe and add a route with 
\Hard Disk\System\TcAmsRemoteMgr.exe
There is also an experimental function AdsClient.AddRoute() for doing this.

*If the library is not working, an incorrect/missing route may be the problem!.*

### Installation
You only need this library.
Twincat is _not_ needed. 

### Mono
The version without the async functions works in Mono 2.10

Mono for android:
Remember to set internet permissions in the manifest.
You must also configure a route for your android device.

### External documentation

[Specification for the ADS/AMS protocol](http://infosys.beckhoff.com/english.php?content=../content/1033/TcAdsAmsSpec/HTML/TcAdsAmsSpec_Intro.htm&id=)

[Index-Group/Offset specification](http://infosys.beckhoff.com/content/1033/tcadsdeviceplc/html/tcadsdeviceplc_intro.htm?id=11742)

### Examples

