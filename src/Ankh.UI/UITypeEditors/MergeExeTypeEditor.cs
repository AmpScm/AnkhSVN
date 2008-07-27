using System;
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
                        new StringEditorTemplate("%mine", "My version", "My version (%mine)"),
                        new StringEditorTemplate("%base", "Base version", "Base version (%base)"),
                        new StringEditorTemplate("%theirs", "Their version", "Their version (%theirs)"),
                        new StringEditorTemplate("%merged", "Merged output", "Merged ouput (%merged)"),

                        // TortoiseMerge
                        new StringEditorTemplate("\"" + programFiles + @"\TortoiseSVN\bin\TortoiseMerge.exe " +
                                                 "/base:\"%base\" /theirs:\"%theirs\" /mine:\"%mine\" /merged:\"%merged\"",
                                                 "TortoiseMerge", "TortoiseMerge"),

                        // KDiff3
                        new StringEditorTemplate("\"" + programFiles + @"\KDiff3\KDiff3.exe " + 
                                                 "\"%base\" --fname \"Base version\" \"%theirs\" --fname \"Their version\" " + 
                                                 "\"%mine\" --fname \"My version\" -o \"%merged\"", "KDiff3", "KDiff3"),

                        // DiffMerge
                        new StringEditorTemplate(("'" + programFiles + @"\SourceGear\DiffMerge\DiffMerge.exe' " +
                                                 "/m '/r=%merged' '%base' '%mine' '%theirs' " +
                                                 "'/t1=%bname' '/t2=%yname' '/t3=%tname' '/c=%mname'").Replace('\'', '\"'),
                                                 "DiffMerge", "DiffMerge"),
                        // WinMerge
                        new StringEditorTemplate("\"" + programFiles + @"\WinMerge\WinMergeU.exe -e -x -ub " +
                                                 "-dl \"Base version\" -dr \"My version\" " +
                                                 "\"%base\" \"%mine\" \"%theirs\"", "WinMerge", "WinMerge"),

                       

                        // Araxis merge
                        new StringEditorTemplate( "\"" + programFiles + @"\Araxis\Araxis Merge\Compare.exe " +
                            "/wait /swap /a3 /3 /title1:Base /title2:Theirs /title3:Merged " +
                            "\"%base\" \"%theirs\" \"%mine\" \"%merged\"", "Araxis", "Araxis" ),

                 };


        }
    }
}
