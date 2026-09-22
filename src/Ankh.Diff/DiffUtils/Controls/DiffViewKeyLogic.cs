// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Windows.Forms;

namespace Ankh.Diff.DiffUtils.Controls
{
    internal enum DiffViewKeyOperation
    {
        None,
        CopySelection,
        OffsetPosition,
        ExtendSelection,
        SetPosition
    }

    internal sealed class DiffViewKeyAction
    {
        public DiffViewKeyAction(
            DiffViewKeyOperation operation,
            int lineDelta,
            int columnDelta,
            int pageFactor,
            bool scrollVertically,
            int targetLine,
            int targetColumn)
        {
            Operation = operation;
            LineDelta = lineDelta;
            ColumnDelta = columnDelta;
            PageFactor = pageFactor;
            ScrollVertically = scrollVertically;
            TargetLine = targetLine;
            TargetColumn = targetColumn;
        }

        public DiffViewKeyOperation Operation { get; private set; }
        public int LineDelta { get; private set; }
        public int ColumnDelta { get; private set; }
        public int PageFactor { get; private set; }
        public bool ScrollVertically { get; private set; }
        public int TargetLine { get; private set; }
        public int TargetColumn { get; private set; }
    }

    internal static class DiffViewKeyLogic
    {
        static readonly DiffViewKeyAction None =
            new DiffViewKeyAction(DiffViewKeyOperation.None, 0, 0, 0, false, 0, 0);

        public static DiffViewKeyAction GetAction(
            Keys keyCode,
            Keys modifiers,
            bool hasSelection,
            int currentLine,
            int currentColumn,
            int lineCount,
            int currentLineLength,
            int documentEndLineLength)
        {
            bool ctrl = modifiers == Keys.Control;
            bool shift = modifiers == Keys.Shift;
            bool normal = modifiers == Keys.None;

            switch (keyCode)
            {
                case Keys.C:
                    return ctrl && hasSelection
                        ? Action(DiffViewKeyOperation.CopySelection)
                        : None;

                case Keys.Up:
                    if (ctrl)
                        return Move(-1, 0, true);
                    if (shift)
                        return Extend(-1, 0);
                    if (normal)
                        return Move(-1, 0, false);
                    return None;

                case Keys.Down:
                    if (ctrl)
                        return Move(1, 0, true);
                    if (shift)
                        return Extend(1, 0);
                    if (normal)
                        return Move(1, 0, false);
                    return None;

                case Keys.Left:
                    if (shift)
                        return Extend(0, -1);
                    if (normal)
                        return Move(0, -1, false);
                    return None;

                case Keys.Right:
                    if (shift)
                        return Extend(0, 1);
                    if (normal)
                        return Move(0, 1, false);
                    return None;

                case Keys.PageUp:
                    if (shift)
                        return Page(DiffViewKeyOperation.ExtendSelection, -1, false);
                    if (normal)
                        return Page(DiffViewKeyOperation.OffsetPosition, -1, true);
                    return None;

                case Keys.PageDown:
                    if (shift)
                        return Page(DiffViewKeyOperation.ExtendSelection, 1, false);
                    if (normal)
                        return Page(DiffViewKeyOperation.OffsetPosition, 1, true);
                    return None;

                case Keys.Home:
                    if (ctrl)
                        return Set(0, 0);
                    if (shift)
                        return Extend(0, -currentColumn);
                    if (normal)
                        return Set(currentLine, 0);
                    return None;

                case Keys.End:
                    if (ctrl)
                        return Set(lineCount, documentEndLineLength);
                    if (shift)
                        return Extend(0, currentLineLength - currentColumn);
                    if (normal)
                        return Set(currentLine, currentLineLength);
                    return None;

                default:
                    return None;
            }
        }

        static DiffViewKeyAction Action(DiffViewKeyOperation operation)
        {
            return new DiffViewKeyAction(operation, 0, 0, 0, false, 0, 0);
        }

        static DiffViewKeyAction Move(int lineDelta, int columnDelta, bool scrollVertically)
        {
            return new DiffViewKeyAction(
                DiffViewKeyOperation.OffsetPosition,
                lineDelta,
                columnDelta,
                0,
                scrollVertically,
                0,
                0);
        }

        static DiffViewKeyAction Extend(int lineDelta, int columnDelta)
        {
            return new DiffViewKeyAction(
                DiffViewKeyOperation.ExtendSelection,
                lineDelta,
                columnDelta,
                0,
                false,
                0,
                0);
        }

        static DiffViewKeyAction Page(
            DiffViewKeyOperation operation,
            int pageFactor,
            bool scrollVertically)
        {
            return new DiffViewKeyAction(
                operation,
                0,
                0,
                pageFactor,
                scrollVertically,
                0,
                0);
        }

        static DiffViewKeyAction Set(int line, int column)
        {
            return new DiffViewKeyAction(
                DiffViewKeyOperation.SetPosition,
                0,
                0,
                0,
                false,
                line,
                column);
        }
    }
}
