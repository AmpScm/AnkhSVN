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

                        // TortoiseMerge
                        new StringEditorTemplate("\"" + programFiles + @"\TortoiseSVN\bin\TortoiseMerge.exe"" " +
                                                 "/base:\"%base\" /mine:\"%mine\"",
                                                 "TortoiseMerge", "TortoiseMerge"),

                        // KDiff3
                        new StringEditorTemplate("\"" + programFiles + @"\KDiff3\KDiff3.exe"" " + 
                                                 "\"%base\" --fname \"Base version\" \"%mine\" --fname \"My version\"", 
                                                 "KDiff3", "KDiff3"),

                        // SourceGear DiffMerge
                        new StringEditorTemplate("\"" + programFiles + @"\SourceGear\DiffMerge\DiffMerge.exe"" " +
                                                 "\"%base\" \"%mine\" " +
                                                 "/t1=\"Base version\" /t2=\"My version\"", "DiffMerge", "DiffMerge"),

                        // WinMerge
                        new StringEditorTemplate("\"" + programFiles + @"\WinMerge\WinMergeU.exe"" -e -x -ub " +
                                                 "-dl \"Base version\" -dr \"My version\" " +
                                                 "\"%base\" \"%mine\"", "WinMerge", "WinMerge"),

                        new StringEditorTemplate("\"" + programFiles + @"\Araxis\Araxis Merge\Compare.exe"" " +
                                                "/wait /2 \"%base\" \"%mine\"", "Araxis", "Araxis"),
                        };
        }
    }
}
