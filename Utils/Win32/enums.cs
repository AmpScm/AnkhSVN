// $Id$
using System;
using System.Runtime.InteropServices;

namespace Utils.Win32
{
    // Styles used in the BROWSEINFO.ulFlags field.
    [Flags]    
    public enum BffStyles 
    {
        RestrictToFilesystem = 0x0001, // BIF_RETURNONLYFSDIRS
        RestrictToDomain =     0x0002, // BIF_DONTGOBELOWDOMAIN
        RestrictToSubfolders = 0x0008, // BIF_RETURNFSANCESTORS
        ShowTextBox =          0x0010, // BIF_EDITBOX
        ValidateSelection =    0x0020, // BIF_VALIDATE
        NewDialogStyle =       0x0040, // BIF_NEWDIALOGSTYLE
        BrowseForComputer =    0x1000, // BIF_BROWSEFORCOMPUTER
        BrowseForPrinter =     0x2000, // BIF_BROWSEFORPRINTER
        BrowseForEverything =  0x4000, // BIF_BROWSEINCLUDEFILES
    }

    [Flags]
    public enum Shacf : uint
    {
        /// <summary>
        /// Currently (SHACF_FILESYSTEM | SHACF_URLALL)
        /// </summary>
        Default              =    0x00000000,  
        /// <summary>
        /// This includes the File System as well as the rest of the shell (Desktop\My Computer\Control Panel\)
        /// </summary>
        Filesystem           =    0x00000001,  

        UrlAll               =    (UrlHistory | UrlMru),
        /// <summary>
        /// URLs in the User's History
        /// </summary>
        UrlHistory           =    0x00000002,   

        /// <summary>
        /// URLs in the User's Recently Used list.
        /// </summary>
        UrlMru               =    0x00000004,   

        /// <summary>
        /// Use the tab to move thru the autocomplete possibilities instead of to the next dialog/window control.
        /// </summary>
        UseTab               =    0x00000008,   

        /// <summary>
        /// This includes the File System
        /// </summary>
        FileSysOnly          =    0x00000010,   

        /// <summary>
        /// Same as SHACF_FILESYS_ONLY except it only includes directories, UNC servers, and UNC server shares.
        /// </summary>
        FileSysDirs          =    0x00000020,  
 
        /// <summary>
        /// Ignore the registry default and force the feature on.
        /// </summary>
        AutoSuggestForceOn   =    0x10000000,   

        /// <summary>
        /// Ignore the registry default and force the feature off.
        /// </summary>
        AutoSuggestForceOff  =    0x20000000,   

        /// <summary>
        /// Ignore the registry default and force the feature on. (Also know as AutoComplete)
        /// </summary>
        AutoAppendForceOn    =    0x40000000,  

        /// <summary>
        /// Ignore the registry default and force the feature off. (Also know as AutoComplete)
        /// </summary>
        AutoAppendForceOff   =    0x80000000,   

    }


}
