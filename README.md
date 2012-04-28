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