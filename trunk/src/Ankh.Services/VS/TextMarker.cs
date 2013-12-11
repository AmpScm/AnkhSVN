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

namespace Ankh.VS
{
    public sealed class TextMarker
    {
        readonly int _index;
        readonly int _length;
        readonly string _value;

        public TextMarker(int index, int length, string value)
        {
            _index = index;
            _length = length;
            _value = value;
        }

        public int Index
        {
            get { return _index; }
        }

        public int Length
        {
            get { return _length; }
        }

        public string Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _value ?? "";
        }
    }
}
