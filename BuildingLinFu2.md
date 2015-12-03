# Getting the latest source code #

You can obtain the latest stable sources for [LinFu](http://code.google.com/p/linfu) by entering the following command in subversion:

`svn checkout http://linfu.googlecode.com/svn/branches/stable/2.0 linfu-read-only`

The stable branch represents the latest build that has been tested to work under a live production environment. Before you start using it, however, you're going to need to build the source files--and that's the next thing I'll show you how to do.

# Building LinFu 2.0 #

## Prerequisites ##

You'll need to have the following software installed in order to build LinFu:
  * Visual Studio 2008 (any version) OR the .NET Framework 2.0 SDK ([download link](http://www.microsoft.com/downloads/details.aspx?familyid=fe6f2099-b7b4-4f47-a244-c96d69c35dec))
  * .NET Framework 3.5 ([download link](http://www.microsoft.com/downloads/details.aspx?FamilyId=333325FD-AE52-4E35-B531-508D977D32A6))
  * ILMerge (optional). You can get it [here](http://www.microsoft.com/downloads/details.aspx?familyid=22914587-b4ad-4eae-87cf-b14ae6a939b0&displaylang=en) .

## Building the source tree ##

Each release of LinFu 2.0 has a batch file named _go.bat_ located in the root directory of the local LinFu source tree that was downloaded to your hard disk. To build the source tree, all you need to do is switch to the LinFu source directory (let's assume that it's "C:\LinFu" ) and type the following command from the VS2008 command line:

`c:\LinFu\go.bat`

The batch file will automatically run all unit tests associated with LinFu on every build. Aside from the tools from Microsoft, the source tree has all the files necessary in the _/tools_ directory to complete the build, so there's no need to download anything else in order to get the build up and running.

Once the build is successful, the output assemblies will be placed in the
_/build/Debug_ directory.

## Using ILMerge to merge the assemblies (optional) ##

Assuming that you have [ILMerge](http://www.microsoft.com/downloads/details.aspx?familyid=22914587-b4ad-4eae-87cf-b14ae6a939b0&displaylang=en) installed, you can opt to build LinFu into a single assembly using the following command:

`c:\LinFu\go.bat merge`

The batch file will automatically perform the build and merge the output files into a single assembly named _LinFu.Core.dll_ and place it into the _/build_ directory. _LinFu.Core.dll_ will contain all the projects from LinFu so you won't have to reference more than one LinFu assembly in all your projects.