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
using System.Text;
using Microsoft.VisualStudio.Package;
using System.Runtime.InteropServices;

namespace Ankh.VS.LanguageServices
{
    [Guid(AnkhId.UnifiedDiffLanguageServiceId), ComVisible(true), CLSCompliant(false)]
    [GlobalService(typeof(UnifiedDiffLanguageService), PublicService = true)]
    public class UnifiedDiffLanguageService : AnkhLanguageService
    {
        public const string ServiceName = AnkhId.UnifiedDiffServiceName;

        public UnifiedDiffLanguageService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        LanguagePreferences _preferences;
        public override Microsoft.VisualStudio.Package.LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(this.Site, typeof(UnifiedDiffLanguageService).GUID, ServiceName);
                _preferences.Init();
            }

            return _preferences;
        }

        UnifiedDiffScanner _scanner;
        public override Microsoft.VisualStudio.Package.IScanner GetScanner(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
        {
            if (_scanner == null)
                _scanner = new UnifiedDiffScanner(Context);
            return _scanner;
        }

        public override string Name
        {
            get { return ServiceName; }
        }

        public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(Microsoft.VisualStudio.Package.ParseRequest req)
        {
            return null;
        }

        class UnifiedDiffScanner : AnkhService, IScanner
        {
            string _line;
            int _offset;

            public UnifiedDiffScanner(IAnkhServiceProvider context)
                : base(context)
            {
            }

            public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
            {
                tokenInfo.StartIndex = _offset;
                tokenInfo.EndIndex = _line.Length;

                char c = '\0';
                if (_line.Length > 0)
                {
                    c = _line[0];
                }
                    switch (c)
                    {
                        case '+':
                            tokenInfo.Color = TokenColor.String;
                            tokenInfo.Type = TokenType.String;
                            break;
                        case '-':
                            tokenInfo.Color = TokenColor.Keyword;
                            tokenInfo.Type = TokenType.Keyword;
                            break;
                        case '@':
                            tokenInfo.Color = TokenColor.Comment;
                            tokenInfo.Type = TokenType.Comment;
                            break;
                        default:
                            tokenInfo.Color = TokenColor.Text;
                            tokenInfo.Type = TokenType.Text;
                            break;
                    }
                

                if (_offset < _line.Length)
                {
                    _offset = _line.Length;
                    return true;
                }
                return false;
            }

            public void SetSource(string source, int offset)
            {
                _line = source;
                _offset = offset;
            }
        }

        public override TypeAndMemberDropdownBars CreateDropDownHelper(Microsoft.VisualStudio.TextManager.Interop.IVsTextView forView)
        {
            return new UnifiedDiffDropDownBar(this);
        }
    }
}
