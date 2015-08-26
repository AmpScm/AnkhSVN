// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Diagnostics;
using Ankh.Configuration;
using Ankh.VS.LanguageServices.Core;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.VS.LanguageServices.LogMessages
{
    class LogMessageColorizer : AnkhColorizer
    {
        public LogMessageColorizer(LogMessageLanguage language, IVsTextLines lines)
            : base(language, lines)
        {
        }

        IAnkhIssueService _issueService;
        IAnkhIssueService IssueService
        {
            [DebuggerStepThrough]
            get { return _issueService ?? (_issueService = GetService<IAnkhIssueService>()); }
        }

        IAnkhConfigurationService _svc;
        IAnkhConfigurationService Configuration
        {
            get { return _svc ?? (_svc = GetService<IAnkhConfigurationService>()); }
        }

        protected override void ColorizeLine(string line, int lineNr, int startState, uint[] attrs, out int endState)
        {
            if (!Configuration.Instance.DisableDashInLogComment)
            {
                bool isComment = false;
                for (int i = 0; i < line.Length; i++)
                {
                    if (!char.IsWhiteSpace(line, i))
                    {
                        if (line[i] == '#')
                            isComment = true;
                        break;
                    }
                }

                if (isComment)
                {
                    for (int i = 0; i < attrs.Length; i++)
                        attrs[i] = (uint)TokenColor.Comment | (uint)COLORIZER_ATTRIBUTE.HUMAN_TEXT_ATTR;

                    endState = 1; 
                    return;
                }
            }

            string combined = null;
            int start = 0, end;
            endState = 1;

            if (startState == 0 && lineNr > 0)
            {
                combined = GetLine(lineNr - 1) + "\n";
                start = combined.Length;
            }

            combined += line;

            end = combined.Length;

            string followed = GetLine(lineNr + 1);
            if (!string.IsNullOrEmpty(followed))
                combined += "\n" + followed;

            if (!string.IsNullOrEmpty(line))
                endState = 0;

            if (IssueService != null)
            {
                IEnumerable<TextMarker> markers;
                if (IssueService.TryGetRevisions(combined, out markers))
                    foreach (TextMarker im in markers)
                    {
                        int from = Math.Max(im.Index, start);
                        int to = Math.Min(end, im.Index + im.Length);

                        for (int i = from; i < to; i++)
                            attrs[i - start] = (uint)TokenColor.String | (uint)COLORIZER_ATTRIBUTE.HUMAN_TEXT_ATTR;
                    }

                if (IssueService.TryGetIssues(combined, out markers))
                    foreach (TextMarker im in markers)
                    {
                        int from = Math.Max(im.Index, start);
                        int to = Math.Min(end, im.Index + im.Length);

                        for (int i = from; i < to; i++)
                            attrs[i - start] = (uint)TokenColor.Keyword | (uint)COLORIZER_ATTRIBUTE.HUMAN_TEXT_ATTR;
                    }
            }
        }
    }
}
