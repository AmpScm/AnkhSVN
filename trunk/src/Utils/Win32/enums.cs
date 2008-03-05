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
    public enum FileAttribute
    {
        Readonly =            0x00000001,  
        Hidden =              0x00000002, 
        System =              0x00000004,  
        Directory =           0x00000010,  
        Archive =             0x00000020,  
        Device =              0x00000040,  
        Normal =              0x00000080,  
        Temporary =           0x00000100,  
        SparseFile =          0x00000200,  
        ReparsePoint =        0x00000400, 
        Compressed =          0x00000800,  
        Offline =             0x00001000,  
        NotContentIndexed =   0x00002000  
    }


}
