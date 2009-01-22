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
using Ankh.Ids;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.Commands
{
    public enum TextQueryType
    {
        None,
        Name,
        Status
    }

    public class CommandUpdateEventArgs : BaseCommandEventArgs
    {
        readonly TextQueryType _queryType;
        bool _disabled;
        bool _invisible;
        bool _latched;
        bool _ninched;
        bool _dynamicMenuEnd;
        string _text;

        public CommandUpdateEventArgs(AnkhCommand command, AnkhContext context, TextQueryType textQuery)
            : this(command, context)
        {
            _queryType = textQuery;
        }

        public CommandUpdateEventArgs(AnkhCommand command, AnkhContext context)
            : base(command, context)
        {
        }

        /// <summary>
        /// The command is enabled
        /// </summary>
        public bool Enabled
        {
            get { return !_disabled; }
            set { _disabled = !value; }
        }

        /// <summary>
        /// The command is visible (Requires dynamicVisibility)
        /// </summary>
        public bool Visible
        {
            get { return !_invisible; }
            set { _invisible = !value; }
        }

        /// <summary>
        /// The command is an on-off toggle and is currently on. (VS term: Latched)
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            get { return _latched; }
            set { _latched = value; }
        }

        /// <summary>
        /// Visual Studio SDK: Reserved for future usage
        /// </summary>
        public bool Ninched
        {
            get { return _ninched; }
            set { _ninched = value; }
        }

        /// <summary>
        /// Allows updating the text (Use <see cref="TextQueryType" /> to determine what text to update
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets the type of the text query.
        /// </summary>
        /// <value>The type of the text query.</value>
        public TextQueryType TextQueryType
        {
            get { return _queryType; }
        }

        /// <summary>
        /// Marks the end of a dynamic menu range if set
        /// </summary>
        public bool DynamicMenuEnd
        {
            get { return _dynamicMenuEnd; }
            set { _dynamicMenuEnd = value; }
        }

        /// <summary>
        /// Updates the ole flags from the command status
        /// </summary>
        /// <param name="cmdf">The CMDF.</param>
        /// <remarks>Used by the commandmappers</remarks>
        [CLSCompliant(false)]
        public void UpdateFlags(ref OLECMDF cmdf)
        {
            if (Enabled)
                cmdf |= OLECMDF.OLECMDF_ENABLED;

            if (Checked)
                cmdf |= OLECMDF.OLECMDF_LATCHED;

            if (Ninched)
                cmdf |= OLECMDF.OLECMDF_NINCHED;

            if (!Visible)
                cmdf |= OLECMDF.OLECMDF_INVISIBLE;
        }
    }
}
