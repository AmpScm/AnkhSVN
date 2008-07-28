﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.UITypeEditors
{
    /// <summary>
    /// UIType editor. Namespace referenced from Ankh.Configuration.Config, Ankh.Configuration
    /// </summary>
    public class MergeExeTypeEditor : StringTypeEditorWithTemplates
    {
        public MergeExeTypeEditor()
        {
            this.SetTitle("MergeExe");
        }

        protected override StringEditorTemplate[] GetTemplates()
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            return new StringEditorTemplate[]{
                        new StringEditorTemplate("%mine", "My version", "My Version (%mine)"),
                        new StringEditorTemplate("%base", "Base version", "Base Version (%base)"),
                        new StringEditorTemplate("%theirs", "Their version", "Their Version (%theirs)"),
                        new StringEditorTemplate("%merged", "Merged output", "Merged Ouput (%merged)"),

                        new StringEditorTemplate("%minename", "My Name", "My Name (%minename)"),
                        new StringEditorTemplate("%basename", "Base Name", "Base Name (%basename)"),
                        new StringEditorTemplate("%theirsname", "Their Name", "Their Name (%theirsname)"),
                        new StringEditorTemplate("%mergedname", "Merged Name", "Merged Name (%mergedname)"),

                        // TortoiseMerge
                        // BH: I use  %PROGRAMFILES% here, as that is /not/ mapped at X64!
                        new StringEditorTemplate((@"'%PROGRAMFILES%\TortoiseSVN\bin\TortoiseMerge.exe' " +
                                                 "/base:'%base' /theirs:'%theirs' /mine:'%mine' /merged:'%merged'").Replace('\'', '"'),
                                                 "TortoiseMerge", "TortoiseMerge"),

                        // KDiff3
                        new StringEditorTemplate(("'" + programFiles + @"\KDiff3\KDiff3.exe' " + 
                                                 "'%base' --fname '%basename' '%theirs' --fname '%theirsname' " + 
                                                 "'%mine' --fname '%minename' -o '%merged'").Replace('\'', '"'), 
                                                 "KDiff3", "KDiff3"),

                        // DiffMerge
                        new StringEditorTemplate(("'" + programFiles + @"\SourceGear\DiffMerge\DiffMerge.exe' " +
                                                 "/m '/r=%merged' '%base' '%mine' '%theirs' " +
                                                 "'/t1=%basename' '/t2=%minename' '/t3=%theirsname' '/c=%mergedname'").Replace('\'', '\"'),
                                                 "DiffMerge", "DiffMerge"),
                        // WinMerge
                        // BH: This one misses the %merged reference
                        new StringEditorTemplate(("'" + programFiles + @"\WinMerge\WinMergeU.exe' -e -x -ub " +
                                                 "-dl '%basename' -dr '%minename' " +
                                                 "'%base' '%mine' '%theirs'").Replace('\'', '"'),
                                                 "WinMerge", "WinMerge"),
                       
                        // Araxis merge
                        new StringEditorTemplate(("'" + programFiles + @"\Araxis\Araxis Merge\Compare.exe' " +
                                                "/wait /swap /a3 /3 /title1:'%basename' /title2:'%theirsname' " +
                                                "/title3:'%minename' '%base' '%theirs' '%mine' '%merged'").Replace('\'', '"'), 
                                                "Araxis", "Araxis" ),
                 };


        }
    }
}
