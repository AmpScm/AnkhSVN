// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
