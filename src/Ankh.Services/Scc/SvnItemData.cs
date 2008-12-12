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
using System.ComponentModel;
using Ankh.Selection;
using System.Diagnostics;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Status={Status}")]
    public class SvnItemData : AnkhPropertyGridItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public SvnItemData(IAnkhServiceProvider context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnItem SvnItem
        {
            get { return _item; }
        }

        [DisplayName("Full Path"), Category("Subversion")]
        public string FullPath
        {
            get { return _item.FullPath; }
        }

        [DisplayName("File Name"), Category("Subversion")]
        public string Name
        {
            get { return _item.Name; }
        }

        [DisplayName("Change List"), Category("Subversion")]
        public string ChangeList
        {
            get { return _item.Status.ChangeList; }
        }

        [DisplayName("Project"), Category("Visual Studio")]
        public string Project
        {
            get 
            {
                IProjectFileMapper mapper = _context.GetService<IProjectFileMapper>();

                if (mapper != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (SvnProject p in mapper.GetAllProjectsContaining(FullPath))
                    {
                        ISvnProjectInfo info = mapper.GetProjectInfo(p);

                        if (info == null)
                        {
                            if (string.Equals(FullPath, mapper.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                                return "<Solution>";
                        }
                        else
                        {
                            if (sb.Length > 0)
                                sb.Append(';');

                            sb.Append(info.UniqueProjectName);
                        }
                    }

                    return sb.ToString();
                }
                return ""; 
            }
        }

        [DisplayName("Status Content"), Category("Subversion")]
        public string Status
        {
            get 
            {
                return _item.Status.LocalContentStatus.ToString(); 
            }
        }

        [DisplayName("Status Properties"), Category("Subversion")]
        public string PropertyStatus
        {
            get
            {
                return _item.Status.LocalPropertyStatus.ToString();
            }
        }

        protected override string ComponentName
        {
            get { return Name; }
        }

        protected override string ClassName
        {
            get { return "Path Status"; }
        }
    }
}
