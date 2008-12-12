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

namespace Ankh.Diff.DiffUtils.Controls
{
    public struct DiffViewPosition
    {
        public DiffViewPosition(int iLine, int iColumn)
        {
            Line = iLine;
            Column = iColumn;
        }

        public int Line;
        public int Column;

        public int CompareTo(DiffViewPosition Pos)
        {
            int iResult = Line - Pos.Line;

            if (iResult == 0)
            {
                iResult = Column - Pos.Column;
            }

            return iResult;
        }

        public override bool Equals(object oPos)
        {
            return CompareTo((DiffViewPosition)oPos) == 0;
        }

        public override int GetHashCode()
        {
            return Line << 16 + Column;
        }

        public static bool operator ==(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return Pos1.Equals(Pos2);
        }

        public static bool operator !=(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return !Pos1.Equals(Pos2);
        }

        public static bool operator <(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return Pos1.CompareTo(Pos2) < 0;
        }

        public static bool operator >(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return Pos1.CompareTo(Pos2) > 0;
        }

        public static bool operator <=(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return Pos1.CompareTo(Pos2) <= 0;
        }

        public static bool operator >=(DiffViewPosition Pos1, DiffViewPosition Pos2)
        {
            return Pos1.CompareTo(Pos2) >= 0;
        }

        public static readonly DiffViewPosition Empty = new DiffViewPosition(-100000, -100000);
    }
}
