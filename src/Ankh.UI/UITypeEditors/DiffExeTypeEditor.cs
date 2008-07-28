using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.UITypeEditors
{
    /// <summary>
    /// UIType editor. Namespace referenced from Ankh.Configuration.Config, Ankh.Configuration
    /// </summary>
    public class DiffExeTypeEditor : StringTypeEditorWithTemplates
    {
        public DiffExeTypeEditor()
        {
            this.SetTitle("DiffExe");
        }

        protected override StringEditorTemplate[] GetTemplates()
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            return new StringEditorTemplate[]{
                        new StringEditorTemplate("%base", "Base version", "Base version (%base)"),
                        new StringEditorTemplate("%mine", "My version", "My version (%mine)"),
                        new StringEditorTemplate("%basename", "Base Name", "Base Name (%basename)"),
                        new StringEditorTemplate("%minename", "Mine Name", "Mine Name (%minename)"),

                        // TortoiseMerge
                        // BH: I use %PROGRAMFILES% here, as that is /not/ mapped at X64!
                        new StringEditorTemplate((@"'%PROGRAMFILES%\TortoiseSVN\bin\TortoiseMerge.exe' " +
                                                 "/base:'%base' /mine:'%mine'").Replace('\'', '"'),
                                                 "TortoiseMerge", "TortoiseMerge"),

                        // KDiff3
                        new StringEditorTemplate(("'" + programFiles + @"\KDiff3\KDiff3.exe' " + 
                                                 "'%base' --fname '%basename' '%mine' --fname '%minename'").Replace('\'','"'), 
                                                 "KDiff3", "KDiff3"),

                        // SourceGear DiffMerge
                        new StringEditorTemplate(("'" + programFiles + @"\SourceGear\DiffMerge\DiffMerge.exe' " +
                                                 "'%base' '%mine' " + "/t1='%basename' /t2='%minename'").Replace('\'', '"'), 
                                                 "DiffMerge", "DiffMerge"),

                        // WinMerge
                        new StringEditorTemplate(("'" + programFiles + @"\WinMerge\WinMergeU.exe' -e -x -ub " +
                                                 "-dl '%basename' -dr 'minename' " +
                                                 "'%base' '%mine'").Replace('\'', '"'),
                                                 "WinMerge", "WinMerge"),

                        new StringEditorTemplate(("'" + programFiles + @"\Araxis\Araxis Merge\Compare.exe' " +
                                                "/wait /2 /title1:'%basename' /title2:'%minename' " +
                                                "'%base' '%mine'").Replace('\'', '"'),
                                                "Araxis", "Araxis"),
                        };
        }
    }
}
