$Id: README.rtf 1073 2004-01-26 23:20:27Z Arild $

*AnkhSVN*

This is the 0.4.1 release of AnkhSVN, a Visual Studio .NET addin for the Subversion version control system.

AnkhSVN supports both VS.NET 2002 and VS.NET 2003. The installer will set up the correct registry entries for both environments. AnkhSVN 0.4 is built against Subversion 0.37, openssl 0.9.7 and Berkeley DB 4.2.52.

The 0.4 release now supports https:// connections, in addition to the file:// and http:// connections supported by prior versions.


*What is currently supported?*

·	The solution explorer shows the status of all files in your working copy(svn status).
·   	You can view selected files in the Repository Explorer(svn cat).
·	You can checkout directories from the Repository Explorer.
·	You can commit(svn commit), update(svn update), add(svn add), revert(svn revert) and run diffs(svn diff) against the text base from within VS.NET. This works on the solution, individual projects, folders and files.
·	Ankh calls svn delete on files deleted from the IDE.
·	You can rename files from within the IDE(svn rename).
·	You can clean up a locked working copy(svn cleanup).
·   You can resolve conflicts(svn resolve).
·   The Properties window displays svn info-style information about a file.
·   You can invoke svn.exe directly from the VS.NET Command Window(type svn /?), as well as navigate the filesystem.

*Support*

The AnkhSVN developers usually hang out in the #ankhsvn channel on the Freenode(irc.freenode.net) IRC network. Feel free to drop in at any time if you have any questions or comments.

Furthermore, there is a mailing list set up at http://ankhsvn.tigris.org. To subscribe, send a mail to users-subscribe@ankhsvn.tigris.org - if you don't want to subscribe, you can mail users@ankhsvn.tigris.org and we will accommodate any requests to CC you directly.

There is also an issue tracker set up at http://ankhsvn.tigris.org/servlets/ProjectIssues - to be able to report issues, you need to have a tigris.org user account and observer status on the AnkhSVN project. We will honor all requests for such status, but we will appreciate it if you search the archives before submitting any new issues.




*Configuration File*

Ankh maintains a configuration file in the %APPDATA%\AnkhSVN directory, which controls various aspects of Ankh's behavior. This file will be created if it does not already exist. The default file is heavily commented.



*Known issues*

·	If you delete a file, the status on the project node will not change. To be able to commit a delete, you need to change something else, then run commit on the project or solution.
·	Adding a (web)form will not automatically add the corresponding .resx(or code behind) file. To add these files, click the Show All Files button in the solution explorer, and then add them the usual way.
·	No support for import - you will need to create your repository and import your source to it using the command line tools. For details on how to use these, see http://svnbook.red-bean.com
·	No support for editing SVN properties - this still needs to be done from the command line.

*Error reporting*

If an unhandled exception is thrown, Ankh will pop up a dialog asking you to submit an error report. If you click yes a web browser will open and take you to a form with the stack trace already filled in. We would appreciate it if you also filled out a description of what you were doing when the exception occurred. Your email address is optional, however, we have no way of getting back to you if you don't fill it in.

If Ankh for some reason stops loading automatically, you need to go into the Tools->Addin Manager... dialog to reenable it.


*Setting up a solution for use with Ankh*

You will get best results from Ankh if you organize your solution so that all the projects are in subdirectories under a common solution folder, like this:

ProjectName/
   ProjectName.sln
   Subproject1/
      Subproject1.csproj
      Foo.cs		
   Subproject2/
      Subproject2.vcproj
      Bar.cpp
      Bar.h
   Subproject3
      Subproject3.vbproj
      Fubar.aspx.vb
      Fubar.aspx

Unfortunately, VS.NET will default to putting the solution file in the same folder as the first project you create. This will work fine if you have only a single project, but might cause problems with solution-level Ankh operations if you have multiple projects. To get around this, click the 'More' button in the New Project dialog, and then check "Create directory for solution". VS.NET will then create a directory structure that looks like the above. Alternatively, you can start out with a blank solution and add the subprojects to that.

When it comes to web projects, the situation is even worse. VS.NET will by default put the solution file somewhere under My Documents\Visual Studio Projects, and put the project itself in a directory under \Inetpub\wwwroot. To have more control over the layout of your solution, take the following steps:

·	Create a blank solution
·	Create a subdirectory under the solution directory to put the web project in
·	Open the Internet Services Manager(Under Administrative Tools)
·	Right click on the web server and choose New->Virtual directory
·	Choose an alias for the virtual directory(foo) and point it to your subdirectory
·	Now right click on the solution and choose Add New Project
·	Choose ASP.NET Web Application
·	For the URL, type in http://localhost/foo.  VS.NET will now create the web project in the subdirectory you created. 

Thank you and enjoy.

Sincerely, the AnkhSVN team
