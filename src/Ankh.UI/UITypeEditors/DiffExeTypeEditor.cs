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
            return new StringEditorTemplate[]{
                        new StringEditorTemplate("%base", "Base version", "Base version (%base)"),
                        new StringEditorTemplate("%mine", "My version", "My version (%mine)"),

                        // TortoiseMerge
                        new StringEditorTemplate(@"C:\Program Files\TortoiseSVN\bin\TortoiseMerge.exe " +
                                                 "/base:\"%base\" /mine:\"%mine\"",
                                                 "TortoiseMerge", "TortoiseMerge"),

                        // KDiff3
                        new StringEditorTemplate(@"C:\Program Files\KDiff3\KDiff3.exe " + 
                                                 "\"%base\" --fname \"Base version\" \"%mine\" --fname \"My version\"", 
                                                 "KDiff3", "KDiff3"),

                        // SourceGear DiffMerge
                        new StringEditorTemplate(@"C:\Program Files\SourceGear\DiffMerge\DiffMerge.exe " +
                                                 "\"%base\" \"%mine\" " +
                                                 "/t1=\"Base version\" /t2=\"My version\"", "DiffMerge", "DiffMerge"),

                        // WinMerge
                        new StringEditorTemplate(@"C:\Program Files\WinMerge\WinMergeU.exe -e -x -ub " +
                                                 "-dl \"Base version\" -dr \"My version\" " +
                                                 "\"%base\" \"%mine\"", "WinMerge", "WinMerge"),

                        new StringEditorTemplate(@"C:\Program Files\Araxis\Araxis Merge\Compare.exe " +
                                                "/wait /2 \"%base\" \"%mine\"", "Araxis", "Araxis"),
                        };
        }
    }
}
