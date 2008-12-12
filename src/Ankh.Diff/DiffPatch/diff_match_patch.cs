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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;

namespace Ankh.Diff.DiffPatch
{
    public class LinkedList<T> : System.Collections.Generic.LinkedList<T>
    {
        public LinkedList()
            : base()
        {
        }

        public LinkedList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public void Add(T item)
        {
            AddLast(item);
        }

        internal void AddRange(LinkedList<T> list)
        {
            foreach (T item in list)
                Add(item);
        }

        public ListIterator<T> ListIterator()
        {
            return new ListIterator<T>(this);
        }

        public T[] GetRange(int start, int length)
        {
            T[] items = new T[length];

            LinkedListNode<T> node = First;
            int i = 0;
            while (i++ < start && node != null)
                node = node.Next;

            i = 0;
            while (i++ < length && node != null)
            {
                items[i] = node.Value;
                node = node.Next;
            }

            return items;
        }
    }

    public class Set<T> : KeyedCollection<T, T>
    {
        protected override T GetKeyForItem(T item)
        {
            return item;
        }
    }

    public class ListIterator<T> : IEnumerator<T>
    {
        readonly LinkedList<T> _list;
        bool _started;
        LinkedListNode<T> _node;
        LinkedListNode<T> _next, _prev;
        public ListIterator(LinkedList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _list = list;
        }

        public bool MoveNext()
        {
            if (_next != null)
            {
                _node = _next;
                _next = _prev = null;
            }
            else if (_started && _node != null)
            {
                _node = _node.Next;
            }
            else if (!_started)
            {
                _started = true;
                _node = _list.First;
            }

            return (_node != null);
        }

        public T Current
        {
            get { return (_node != null) ? _node.Value : default(T); }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public void Reset()
        {
            _node = null;
            _started = false;
        }

        public bool Previous()
        {
            if (_prev != null)
                _node = _prev;
            else
                _node = _node.Previous;

            _prev = _next = null;

            return _node != null;
        }

        public void Remove()
        {
            if (_node == null)
                throw new InvalidOperationException();

            _prev = _node.Previous;
            _next = _node.Next;

            _list.Remove(_node);
            _node = null;
        }

        public void Add(T item)
        {
            _list.AddBefore(_node, item);
        }

        public bool HasPrevious
        {
            get { return _node != null ? ((_prev ?? _node.Previous) != null) : false; }
        }

        public bool HasNext
        {
            get { return _node != null ? ((_next ?? _node.Next) != null) : false; }
        }

        public void Set(T value)
        {
            LinkedListNode<T> old = _node;
            _node = _list.AddAfter(old, value);
            _list.Remove(old);
        }
    }


    /**-
         * The data structure representing a diff is a Linked list of Diff objects:
         * {Diff(Operation.DELETE, "Hello"), Diff(Operation.INSERT, "Goodbye"),
         *  Diff(Operation.EQUAL, " world.")}
         * which means: delete "Hello", add "Goodbye" and keep " world."
         */
    public enum Operation
    {
        None,
        DELETE, INSERT, EQUAL
    }

    /*
 * Functions for diff, match and patch.
 * Computes the difference between two texts to create a patch.
 * Applies the patch onto another text, allowing for errors.
 *
 * @author fraser@google.com (Neil Fraser)
 */

    /**
     * Class containing the diff, match and patch methods.
     * Also contains the behaviour settings.
     */
    public class diff_match_patch
    {

        // Defaults.
        // Set these on your diff_match_patch instance to override the defaults.

        // Number of seconds to map a diff before giving up.  (0 for infinity)
        public float Diff_Timeout = 1.0f;
        // Cost of an empty edit operation in terms of edit characters.
        public short Diff_EditCost = 4;
        // The size beyond which the double-ended diff activates.
        // Double-ending is twice as fast, but less accurate.
        public short Diff_DualThreshold = 32;
        // Tweak the relative importance (0.0 = accuracy, 1.0 = proximity)
        public float Match_Balance = 0.5f;
        // At what point is no match declared (0.0 = perfection, 1.0 = very loose)
        public float Match_Threshold = 0.5f;
        // The min and max cutoffs used when computing text lengths.
        public int Match_MinLength = 100;
        public int Match_MaxLength = 1000;
        // Chunk size for context length.
        public short Patch_Margin = 4;

        // The number of bits in an int.
        private int Match_MaxBits = 32;


        //  DIFF FUNCTIONS





        /**
         * Find the differences between two texts.
         * Run a faster slightly less optimal diff
         * This method allows the 'checklines' of diff_main() to be optional.
         * Most of the time checklines is wanted, so default to true.
         * @param text1 Old string to be diffed
         * @param text2 New string to be diffed
         * @return Linked List of Diff objects
         */
        public LinkedList<Diff> diff_main(String text1, String text2)
        {
            return diff_main(text1, text2, true);
        }

        /**
         * Find the differences between two texts.  Simplifies the problem by
         * stripping any common prefix or suffix off the texts before diffing.
         * @param text1 Old string to be diffed
         * @param text2 New string to be diffed
         * @param checklines Speedup flag.  If false, then don't run a
         *     line-level diff first to identify the changed areas.
         *     If true, then run a faster slightly less optimal diff
         * @return Linked List of Diff objects
         */
        public LinkedList<Diff> diff_main(String text1, String text2,
                                          bool checklines)
        {
            // Check for equality (speedup)
            LinkedList<Diff> diffs;
            if (text1.Equals(text2))
            {
                diffs = new LinkedList<Diff>();
                diffs.Add(new Diff(Operation.EQUAL, text1));
                return diffs;
            }

            // Trim off common prefix (speedup)
            int commonlength = diff_commonPrefix(text1, text2);
            String commonprefix = text1.Substring(0, commonlength);
            text1 = text1.Substring(commonlength);
            text2 = text2.Substring(commonlength);

            // Trim off common suffix (speedup)
            commonlength = diff_commonSuffix(text1, text2);
            String commonsuffix = text1.Substring(text1.Length - commonlength);
            text1 = text1.Substring(0, text1.Length - commonlength);
            text2 = text2.Substring(0, text2.Length - commonlength);

            // Compute the diff on the middle block
            diffs = diff_compute(text1, text2, checklines);

            // Restore the prefix and suffix
            if (commonprefix.Length != 0)
            {
                diffs.AddFirst(new Diff(Operation.EQUAL, commonprefix));
            }
            if (commonsuffix.Length != 0)
            {
                diffs.AddLast(new Diff(Operation.EQUAL, commonsuffix));
            }

            diff_cleanupMerge(diffs);
            return diffs;
        }


        /**
         * Find the differences between two texts.  Assumes that the texts do not
         * have any common prefix or suffix.
         * @param text1 Old string to be diffed
         * @param text2 New string to be diffed
         * @param checklines Speedup flag.  If false, then don't run a
         *     line-level diff first to identify the changed areas.
         *     If true, then run a faster slightly less optimal diff
         * @return Linked List of Diff objects
         */
        protected LinkedList<Diff> diff_compute(String text1, String text2,
                                                bool checklines)
        {
            LinkedList<Diff> diffs = new LinkedList<Diff>();

            if (text1.Length == 0)
            {
                // Just add some text (speedup)
                diffs.Add(new Diff(Operation.INSERT, text2));
                return diffs;
            }

            if (text2.Length == 0)
            {
                // Just delete some text (speedup)
                diffs.Add(new Diff(Operation.DELETE, text1));
                return diffs;
            }

            String longtext = text1.Length > text2.Length ? text1 : text2;
            String shorttext = text1.Length > text2.Length ? text2 : text1;
            int i = longtext.IndexOf(shorttext);
            if (i != -1)
            {
                // Shorter text is inside the longer text (speedup)
                Operation op = (text1.Length > text2.Length) ?
                               Operation.DELETE : Operation.INSERT;
                diffs.Add(new Diff(op, longtext.Substring(0, i)));
                diffs.Add(new Diff(Operation.EQUAL, shorttext));
                diffs.Add(new Diff(op, longtext.Substring(i + shorttext.Length)));
                return diffs;
            }
            longtext = shorttext = null;  // Garbage collect

            // Check to see if the problem can be split in two.
            String[] hm = diff_halfMatch(text1, text2);
            if (hm != null)
            {
                // A half-match was found, sort out the return data.
                String text1_a = hm[0];
                String text1_b = hm[1];
                String text2_a = hm[2];
                String text2_b = hm[3];
                String mid_common = hm[4];
                // Send both pairs off for separate processing.
                LinkedList<Diff> diffs_a = diff_main(text1_a, text2_a, checklines);
                LinkedList<Diff> diffs_b = diff_main(text1_b, text2_b, checklines);
                // Merge the results.
                diffs = diffs_a;
                diffs.Add(new Diff(Operation.EQUAL, mid_common));
                diffs.AddRange(diffs_b);
                return diffs;
            }

            // Perform a real diff.
            if (checklines && (text1.Length < 100 || text2.Length < 100))
            {
                checklines = false;  // Too trivial for the overhead.
            }
            List<String> linearray = null;
            if (checklines)
            {
                // Scan the text on a line-by-line basis first.
                DiffCharResult b = diff_linesToChars(text1, text2);
                text1 = b.Chars1;
                text2 = b.Chars2;
                linearray = b.Lines;
            }

            diffs = diff_map(text1, text2);
            if (diffs == null)
            {
                // No acceptable result.
                diffs = new LinkedList<Diff>();
                diffs.Add(new Diff(Operation.DELETE, text1));
                diffs.Add(new Diff(Operation.INSERT, text2));
            }

            if (checklines)
            {
                // Convert the diff back to original text.
                diff_charsToLines(diffs, linearray);
                // Eliminate freak matches (e.g. blank lines)
                diff_cleanupSemantic(diffs);

                // Rediff any replacement blocks, this time character-by-character.
                // Add a dummy entry at the end.
                diffs.Add(new Diff(Operation.EQUAL, ""));
                int count_delete = 0;
                int count_insert = 0;
                String text_delete = "";
                String text_insert = "";
                ListIterator<Diff> pointer = diffs.ListIterator();

                while (pointer.MoveNext())
                {
                    Diff thisDiff = pointer.Current;
                    switch (thisDiff.operation)
                    {
                        case Operation.INSERT:
                            count_insert++;
                            text_insert += thisDiff.text;
                            break;
                        case Operation.DELETE:
                            count_delete++;
                            text_delete += thisDiff.text;
                            break;
                        case Operation.EQUAL:
                            // Upon reaching an equality, check for prior redundancies.
                            if (count_delete >= 1 && count_insert >= 1)
                            {
                                // Delete the offending records and add the merged ones.
                                pointer.Previous();
                                for (int j = 0; j < count_delete + count_insert; j++)
                                {
                                    pointer.Previous();
                                    pointer.Remove();
                                }
                                foreach (Diff newDiff in diff_main(text_delete, text_insert, false))
                                {
                                    pointer.Add(newDiff);
                                }
                            }
                            count_insert = 0;
                            count_delete = 0;
                            text_delete = "";
                            text_insert = "";
                            break;
                    }
                }
                diffs.RemoveLast();  // Remove the dummy entry at the end.
            }
            return diffs;
        }

        protected sealed class DiffCharResult
        {
            readonly string _chars1;
            readonly string _chars2;
            readonly List<string> _lines;

            public DiffCharResult(string chars1, string chars2, List<string> lines)
            {
                _chars1 = chars1;
                _chars2 = chars2;
                _lines = lines;
            }

            public string Chars1 { get { return _chars1; } }
            public string Chars2 { get { return _chars2; } }
            public List<string> Lines { get { return _lines; } }
        }


        /**
         * Split two texts into a list of strings.  Reduce the texts to a string of
         * hashes where each Unicode character represents one line.
         * @param text1 First string
         * @param text2 Second string
         * @return Three element Object array, containing the encoded text1, the
         *     encoded text2 and the List of unique strings.  The zeroth element
         *     of the List of unique strings is intentionally blank.
         */
        protected DiffCharResult diff_linesToChars(String text1, String text2)
        {
            List<String> lineArray = new List<String>();
            Dictionary<String, int> lineHash = new Dictionary<String, int>();
            // e.g. linearray[4] == "Hello\n"
            // e.g. linehash.get("Hello\n") == 4

            // "\x00" is a valid character, but various debuggers don't like it.
            // So we'll insert a junk entry to avoid generating a null character.
            lineArray.Add("");

            String chars1 = diff_linesToCharsMunge(text1, lineArray, lineHash);
            String chars2 = diff_linesToCharsMunge(text2, lineArray, lineHash);
            return new DiffCharResult(chars1, chars2, lineArray);
        }


        /**
         * Split a text into a list of strings.  Reduce the texts to a string of
         * hashes where each Unicode character represents one line.
         * @param text String to encode
         * @param lineArray List of unique strings
         * @param lineHash Map of strings to indices
         * @return Encoded string
         */
        private String diff_linesToCharsMunge(String text, List<String> lineArray,
                                              Dictionary<String, int> lineHash)
        {
            int lineStart = 0;
            int lineEnd = -1;
            String line;
            StringBuilder chars = new StringBuilder();
            // Walk the text, pulling out a substring for each line.
            // text.split('\n') would would temporarily double our memory footprint.
            // Modifying text would create many large strings to garbage collect.
            while (lineEnd < text.Length - 1)
            {
                lineEnd = text.IndexOf('\n', lineStart);
                if (lineEnd == -1)
                {
                    lineEnd = text.Length - 1;
                }
                line = text.Substring(lineStart, lineEnd + 1);
                lineStart = lineEnd + 1;

                if (lineHash.ContainsKey(line))
                {
                    chars.AppendFormat(CultureInfo.InvariantCulture, "{0}", lineHash[line]);
                }
                else
                {
                    lineArray.Add(line);
                    lineHash[line] = lineArray.Count - 1;
                    chars.AppendFormat(CultureInfo.InvariantCulture, "{0}", lineArray.Count - 1);
                }
            }
            return chars.ToString();
        }


        /**
         * Rehydrate the text in a diff from a string of line hashes to real lines of
         * text.
         * @param diffs LinkedList of Diff objects
         * @param lineArray List of unique strings
         */
        protected void diff_charsToLines(LinkedList<Diff> diffs,
                                        List<String> lineArray)
        {
            StringBuilder text;
            foreach (Diff diff in diffs)
            {
                text = new StringBuilder();
                for (int y = 0; y < diff.text.Length; y++)
                {
                    text.Append(lineArray[diff.text[y]]);
                }
                diff.text = text.ToString();
            }
        }


        /**
         * Explore the intersection points between the two texts.
         * @param text1 Old string to be diffed
         * @param text2 New string to be diffed
         * @return LinkedList of Diff objects or null if no diff available
         */
        protected LinkedList<Diff> diff_map(String text1, String text2)
        {
            DateTime msEnd = DateTime.Now + TimeSpan.FromSeconds(Diff_Timeout);
            int max_d = text1.Length + text2.Length - 1;
            bool doubleEnd = Diff_DualThreshold * 2 < max_d;
            List<Set<long>> v_map1 = new List<Set<long>>();
            List<Set<long>> v_map2 = new List<Set<long>>();
            Dictionary<int, int> v1 = new Dictionary<int, int>();
            Dictionary<int, int> v2 = new Dictionary<int, int>();
            v1[1] = 0;
            v2[1] = 0;
            int x, y;
            long footstep = 0L;  // Used to track overlapping paths.
            Dictionary<long, int> footsteps = new Dictionary<long, int>();
            bool done = false;
            // If the total number of characters is odd, then the front path will
            // collide with the reverse path.
            bool front = ((text1.Length + text2.Length) % 2 == 1);
            for (int d = 0; d < max_d; d++)
            {
                // Bail out if timeout reached.
                if (Diff_Timeout > 0 && DateTime.Now > msEnd)
                {
                    return null;
                }

                // Walk the front path one step.
                v_map1.Add(new Set<long>());  // Adds at index 'd'.
                for (int k = -d; k <= d; k += 2)
                {
                    if (k == -d || k != d && v1[k - 1] < v1[k + 1])
                    {
                        x = v1[k + 1];
                    }
                    else
                    {
                        x = v1[k - 1] + 1;
                    }
                    y = x - k;
                    if (doubleEnd)
                    {
                        footstep = diff_footprint(x, y);
                        if (front && (footsteps.ContainsKey(footstep)))
                        {
                            done = true;
                        }
                        if (!front)
                        {
                            footsteps[footstep] = d;
                        }
                    }
                    while (!done && x < text1.Length && y < text2.Length
                           && text1[x] == text2[y])
                    {
                        x++;
                        y++;
                        if (doubleEnd)
                        {
                            footstep = diff_footprint(x, y);
                            if (front && (footsteps.ContainsKey(footstep)))
                            {
                                done = true;
                            }
                            if (!front)
                            {
                                footsteps[footstep] = d;
                            }
                        }
                    }
                    v1[k] = x;
                    v_map1[d].Add(diff_footprint(x, y));
                    if (x == text1.Length && y == text2.Length)
                    {
                        // Reached the end in single-path mode.
                        return diff_path1(v_map1, text1, text2);
                    }
                    else if (done)
                    {
                        // Front path ran over reverse path.
                        v_map2 = v_map2.GetRange(0, footsteps[footstep] + 1);
                        LinkedList<Diff> a = diff_path1(v_map1, text1.Substring(0, x),
                                                        text2.Substring(0, y));
                        a.AddRange(diff_path2(v_map2, text1.Substring(x), text2.Substring(y)));
                        return a;
                    }
                }

                if (doubleEnd)
                {
                    // Walk the reverse path one step.
                    v_map2.Add(new Set<long>());  // Adds at index 'd'.
                    for (int k = -d; k <= d; k += 2)
                    {
                        if (k == -d || k != d && v2[k - 1] < v2[k + 1])
                        {
                            x = v2[k + 1];
                        }
                        else
                        {
                            x = v2[k - 1] + 1;
                        }
                        y = x - k;
                        footstep = diff_footprint(text1.Length - x, text2.Length - y);
                        if (!front && (footsteps.ContainsKey(footstep)))
                        {
                            done = true;
                        }
                        if (front)
                        {
                            footsteps[footstep] = d;
                        }
                        while (!done && x < text1.Length && y < text2.Length
                               && text1[text1.Length - x - 1]
                               == text2[text2.Length - y - 1])
                        {
                            x++;
                            y++;
                            footstep = diff_footprint(text1.Length - x, text2.Length - y);
                            if (!front && (footsteps.ContainsKey(footstep)))
                            {
                                done = true;
                            }
                            if (front)
                            {
                                footsteps[footstep] = d;
                            }
                        }
                        v2[k] = x;
                        v_map2[d].Add(diff_footprint(x, y));
                        if (done)
                        {
                            // Reverse path ran over front path.
                            v_map1 = v_map1.GetRange(0, footsteps[footstep] + 1);
                            LinkedList<Diff> a
                                = diff_path1(v_map1, text1.Substring(0, text1.Length - x),
                                             text2.Substring(0, text2.Length - y));
                            a.AddRange(diff_path2(v_map2, text1.Substring(text1.Length - x),
                                                text2.Substring(text2.Length - y)));
                            return a;
                        }
                    }
                }
            }
            // Number of diffs equals number of characters, no commonality at all.
            return null;
        }


        /**
         * Work from the middle back to the start to determine the path.
         * @param v_map List of path sets.
         * @param text1 Old string fragment to be diffed
         * @param text2 New string fragment to be diffed
         * @return LinkedList of Diff objects
         */
        protected LinkedList<Diff> diff_path1(List<Set<long>> v_map,
                                              String text1, String text2)
        {
            LinkedList<Diff> path = new LinkedList<Diff>();
            int x = text1.Length;
            int y = text2.Length;
            Operation last_op = Operation.None;
            for (int d = v_map.Count - 2; d >= 0; d--)
            {
                while (true)
                {
                    if (v_map[d].Contains(diff_footprint(x - 1, y)))
                    {
                        x--;
                        if (last_op == Operation.DELETE)
                        {
                            path.First.Value.text = text1[x] + path.First.Value.text;
                        }
                        else
                        {
                            path.AddFirst(new Diff(Operation.DELETE,
                                                   text1.Substring(x, x + 1)));
                        }
                        last_op = Operation.DELETE;
                        break;
                    }
                    else if (v_map[d].Contains(diff_footprint(x, y - 1)))
                    {
                        y--;
                        if (last_op == Operation.INSERT)
                        {
                            path.First.Value.text = text2[y] + path.First.Value.text;
                        }
                        else
                        {
                            path.AddFirst(new Diff(Operation.INSERT,
                                                   text2.Substring(y, y + 1)));
                        }
                        last_op = Operation.INSERT;
                        break;
                    }
                    else
                    {
                        x--;
                        y--;
                        Debug.Assert(text1[x] == text2[y]
                               , "No diagonal.  Can't happen. (diff_path1)");
                        if (last_op == Operation.EQUAL)
                        {
                            path.First.Value.text = text1[x] + path.First.Value.text;
                        }
                        else
                        {
                            path.AddFirst(new Diff(Operation.EQUAL, text1.Substring(x, x + 1)));
                        }
                        last_op = Operation.EQUAL;
                    }
                }
            }
            return path;
        }


        /**
         * Work from the middle back to the end to determine the path.
         * @param v_map List of path sets.
         * @param text1 Old string fragment to be diffed
         * @param text2 New string fragment to be diffed
         * @return LinkedList of Diff objects
         */
        protected LinkedList<Diff> diff_path2(List<Set<long>> v_map,
                                              String text1, String text2)
        {
            LinkedList<Diff> path = new LinkedList<Diff>();
            int x = text1.Length;
            int y = text2.Length;
            Operation last_op = Operation.None;
            for (int d = v_map.Count - 2; d >= 0; d--)
            {
                while (true)
                {
                    if (v_map[d].Contains(diff_footprint(x - 1, y)))
                    {
                        x--;
                        if (last_op == Operation.DELETE)
                        {
                            path.Last.Value.text += text1[text1.Length - x - 1];
                        }
                        else
                        {
                            path.AddLast(new Diff(Operation.DELETE,
                                text1.Substring(text1.Length - x - 1, text1.Length - x)));
                        }
                        last_op = Operation.DELETE;
                        break;
                    }
                    else if (v_map[d].Contains(diff_footprint(x, y - 1)))
                    {
                        y--;
                        if (last_op == Operation.INSERT)
                        {
                            path.Last.Value.text += text2[text2.Length - y - 1];
                        }
                        else
                        {
                            path.AddLast(new Diff(Operation.INSERT,
                                text2.Substring(text2.Length - y - 1, text2.Length - y)));
                        }
                        last_op = Operation.INSERT;
                        break;
                    }
                    else
                    {
                        x--;
                        y--;
                        Debug.Assert(text1[text1.Length - x - 1]
                                == text2[text2.Length - y - 1]
                               , "No diagonal.  Can't happen. (diff_path2)");
                        if (last_op == Operation.EQUAL)
                        {
                            path.Last.Value.text += text1[text1.Length - x - 1];
                        }
                        else
                        {
                            path.AddLast(new Diff(Operation.EQUAL,
                                text1.Substring(text1.Length - x - 1, text1.Length - x)));
                        }
                        last_op = Operation.EQUAL;
                    }
                }
            }
            return path;
        }


        /**
         * Compute a good hash of two integers.
         * @param x First int
         * @param y Second int
         * @return A long made up of both ints.
         */
        protected long diff_footprint(int x, int y)
        {
            // The maximum size for a long is 9,223,372,036,854,775,807
            // The maximum size for an int is 2,147,483,647
            // Two ints fit nicely in one long.
            // The return value is usually destined as a key in a hash, so return an
            // object rather than a primitive, thus skipping an automatic boxing.
            long result = x;
            result = result << 32;
            result += y;
            return result;
        }


        /**
         * Determine the common prefix of two strings
         * @param text1 First string
         * @param text2 Second string
         * @return The number of characters common to the start of each string.
         */
        public int diff_commonPrefix(String text1, String text2)
        {
            // Performance analysis: http://neil.fraser.name/news/2007/10/09/
            int n = Math.Min(text1.Length, text2.Length);
            for (int i = 0; i < n; i++)
            {
                if (text1[i] != text2[i])
                {
                    return i;
                }
            }
            return n;
        }


        /**
         * Determine the common suffix of two strings
         * @param text1 First string
         * @param text2 Second string
         * @return The number of characters common to the end of each string.
         */
        public int diff_commonSuffix(String text1, String text2)
        {
            // Performance analysis: http://neil.fraser.name/news/2007/10/09/
            int n = Math.Min(text1.Length, text2.Length);
            for (int i = 0; i < n; i++)
            {
                if (text1[text1.Length - i - 1]
                    != text2[text2.Length - i - 1])
                {
                    return i;
                }
            }
            return n;
        }


        /**
         * Do the two texts share a substring which is at least half the length of
         * the longer text?
         * @param text1 First string
         * @param text2 Second string
         * @return Five element String array, containing the prefix of text1, the
         *     suffix of text1, the prefix of text2, the suffix of text2 and the
         *     common middle.  Or null if there was no match.
         */
        protected String[] diff_halfMatch(String text1, String text2)
        {
            String longtext = text1.Length > text2.Length ? text1 : text2;
            String shorttext = text1.Length > text2.Length ? text2 : text1;
            if (longtext.Length < 10 || shorttext.Length < 1)
            {
                return null;  // Pointless.
            }

            // First check if the second quarter is the seed for a half-match.
            String[] hm1 = diff_halfMatchI(longtext, shorttext,
                                           (longtext.Length + 3) / 4);
            // Check again based on the third quarter.
            String[] hm2 = diff_halfMatchI(longtext, shorttext,
                                           (longtext.Length + 1) / 2);
            String[] hm;
            if (hm1 == null && hm2 == null)
            {
                return null;
            }
            else if (hm2 == null)
            {
                hm = hm1;
            }
            else if (hm1 == null)
            {
                hm = hm2;
            }
            else
            {
                // Both matched.  Select the longest.
                hm = hm1[4].Length > hm2[4].Length ? hm1 : hm2;
            }

            // A half-match was found, sort out the return data.
            if (text1.Length > text2.Length)
            {
                return hm;
                //return new String[]{hm[0], hm[1], hm[2], hm[3], hm[4]};
            }
            else
            {
                return new String[] { hm[2], hm[3], hm[0], hm[1], hm[4] };
            }
        }


        /**
         * Does a substring of shorttext exist within longtext such that the
         * substring is at least half the length of longtext?
         * @param longtext Longer string
         * @param shorttext Shorter string
         * @param i Start index of quarter length substring within longtext
         * @return Five element String array, containing the prefix of longtext, the
         *     suffix of longtext, the prefix of shorttext, the suffix of shorttext
         *     and the common middle.  Or null if there was no match.
         */
        private String[] diff_halfMatchI(String longtext, String shorttext, int i)
        {
            // Start with a 1/4 length substring at position i as a seed.
            String seed = longtext.Substring(i,
                i + longtext.Length / 4);
            int j = -1;
            String best_common = "";
            String best_longtext_a = "", best_longtext_b = "";
            String best_shorttext_a = "", best_shorttext_b = "";
            while ((j = shorttext.IndexOf(seed, j + 1)) != -1)
            {
                int prefixLength = diff_commonPrefix(longtext.Substring(i),
                                               shorttext.Substring(j));
                int suffixLength = diff_commonSuffix(longtext.Substring(0, i),
                                           shorttext.Substring(0, j));
                if (best_common.Length < suffixLength + prefixLength)
                {
                    best_common = shorttext.Substring(j - suffixLength, j)
                        + shorttext.Substring(j, j + prefixLength);
                    best_longtext_a = longtext.Substring(0, i - suffixLength);
                    best_longtext_b = longtext.Substring(i + prefixLength);
                    best_shorttext_a = shorttext.Substring(0, j - suffixLength);
                    best_shorttext_b = shorttext.Substring(j + prefixLength);
                }
            }
            if (best_common.Length >= longtext.Length / 2)
            {
                return new String[]{best_longtext_a, best_longtext_b,
                          best_shorttext_a, best_shorttext_b, best_common};
            }
            else
            {
                return null;
            }
        }


        /**
         * Reduce the number of edits by eliminating semantically trivial equalities.
         * @param diffs LinkedList of Diff objects
         */
        public void diff_cleanupSemantic(LinkedList<Diff> diffs)
        {
            if (diffs.Count == 0)
            {
                return;
            }
            bool changes = false;
            Stack<Diff> equalities = new Stack<Diff>();  // Stack of qualities.
            String lastequality = null; // Always equal to equalities.lastElement().text
            ListIterator<Diff> pointer = diffs.ListIterator();
            // Number of characters that changed prior to the equality.
            int length_changes1 = 0;
            // Number of characters that changed after the equality.
            int length_changes2 = 0;

            while (pointer.MoveNext())
            {
                Diff thisDiff = pointer.Current;
                if (thisDiff.operation == Operation.EQUAL)
                {
                    // equality found
                    equalities.Push(thisDiff);
                    length_changes1 = length_changes2;
                    length_changes2 = 0;
                    lastequality = thisDiff.text;
                }
                else
                {
                    // an insertion or deletion
                    length_changes2 += thisDiff.text.Length;
                    if (lastequality != null && (lastequality.Length <= length_changes1)
                        && (lastequality.Length <= length_changes2))
                    {
                        //System.out.println("Splitting: '" + lastequality + "'");
                        // Walk back to offending equality.
                        while (thisDiff != equalities.Peek())
                        {
                            pointer.Previous();
                            thisDiff = pointer.Current;
                        }
                        pointer.MoveNext();

                        // Replace equality with a delete.
                        pointer.Set(new Diff(Operation.DELETE, lastequality));
                        // Insert a corresponding an insert.
                        pointer.Add(new Diff(Operation.INSERT, lastequality));

                        equalities.Pop();  // Throw away the equality we just deleted.
                        if (equalities.Count > 0)
                        {
                            // Throw away the previous equality (it needs to be reevaluated).
                            equalities.Pop();
                        }
                        if (equalities.Count == 0)
                        {
                            // There are no previous equalities, walk back to the start.
                            pointer.Reset();
                        }
                        else
                        {
                            // There is a safe equality we can fall back to.
                            thisDiff = equalities.Peek();
                            do
                            {
                                pointer.Previous();
                            }
                            while (pointer.Current != thisDiff);
                        }

                        length_changes1 = 0;  // Reset the counters.
                        length_changes2 = 0;
                        lastequality = null;
                        changes = true;
                    }
                }

            }

            if (changes)
            {
                diff_cleanupMerge(diffs);
            }
            diff_cleanupSemanticLossless(diffs);
        }


        /**
         * Look for single edits surrounded on both sides by equalities
         * which can be shifted sideways to align the edit to a word boundary.
         * e.g: The c<ins>at c</ins>ame. -> The <ins>cat </ins>came.
         * @param diffs LinkedList of Diff objects
         */
        public void diff_cleanupSemanticLossless(LinkedList<Diff> diffs)
        {
            String equality1, edit, equality2;
            String commonString;
            int commonOffset;
            int score, bestScore;
            String bestEquality1, bestEdit, bestEquality2;
            // Create a new iterator at the start.
            ListIterator<Diff> pointer = diffs.ListIterator();
            Diff prevDiff = pointer.MoveNext() ? pointer.Current : null;
            Diff thisDiff = pointer.MoveNext() ? pointer.Current : null;
            Diff nextDiff = pointer.MoveNext() ? pointer.Current : null;
            // Intentionally ignore the first and last element (don't need checking).
            while (nextDiff != null)
            {
                if (prevDiff.operation == Operation.EQUAL &&
                    nextDiff.operation == Operation.EQUAL)
                {
                    // This is a single edit surrounded by equalities.
                    equality1 = prevDiff.text;
                    edit = thisDiff.text;
                    equality2 = nextDiff.text;

                    // First, shift the edit as far left as possible.
                    commonOffset = diff_commonSuffix(equality1, edit);
                    if (commonOffset != 0)
                    {
                        commonString = edit.Substring(edit.Length - commonOffset);
                        equality1 = equality1.Substring(0, equality1.Length - commonOffset);
                        edit = commonString + edit.Substring(0, edit.Length - commonOffset);
                        equality2 = commonString + equality2;
                    }

                    // Second, step character by character right, looking for the best fit.
                    bestEquality1 = equality1;
                    bestEdit = edit;
                    bestEquality2 = equality2;
                    bestScore = diff_cleanupSemanticScore(equality1, edit)
                        + diff_cleanupSemanticScore(edit, equality2);
                    while (edit.Length != 0 && equality2.Length != 0
                        && edit[0] == equality2[0])
                    {
                        equality1 += edit[0];
                        edit = edit.Substring(1) + equality2[0];
                        equality2 = equality2.Substring(1);
                        score = diff_cleanupSemanticScore(equality1, edit)
                            + diff_cleanupSemanticScore(edit, equality2);
                        // The >= encourages trailing rather than leading whitespace on edits.
                        if (score >= bestScore)
                        {
                            bestScore = score;
                            bestEquality1 = equality1;
                            bestEdit = edit;
                            bestEquality2 = equality2;
                        }
                    }

                    if (!prevDiff.text.Equals(bestEquality1))
                    {
                        // We have an improvement, save it back to the diff.
                        if (bestEquality1.Length != 0)
                        {
                            prevDiff.text = bestEquality1;
                        }
                        else
                        {
                            pointer.Previous(); // Walk past nextDiff.
                            pointer.Previous(); // Walk past thisDiff.
                            pointer.Previous(); // Walk past prevDiff.
                            pointer.Remove(); // Delete prevDiff.
                            pointer.MoveNext(); // Walk past thisDiff.
                            pointer.MoveNext(); // Walk past nextDiff.
                        }
                        thisDiff.text = bestEdit;
                        if (bestEquality2.Length != 0)
                        {
                            nextDiff.text = bestEquality2;
                        }
                        else
                        {
                            pointer.Remove(); // Delete nextDiff.
                            nextDiff = thisDiff;
                            thisDiff = prevDiff;
                        }
                    }
                }
                prevDiff = thisDiff;
                thisDiff = nextDiff;
                nextDiff = pointer.MoveNext() ? pointer.Current : null;
            }
        }


        /**
         * Given two strings, compute a score representing whether the internal
         * boundary falls on logical boundaries.
         * Scores range from 5 (best) to 0 (worst).
         * @param one First string
         * @param two Second string
         * @return The score.
         */
        private int diff_cleanupSemanticScore(String one, String two)
        {
            if (one.Length == 0 || two.Length == 0)
            {
                // Edges are the best.
                return 5;
            }

            // Each port of this function behaves slightly differently due to
            // subtle differences in each language's definition of things like
            // 'whitespace'.  Since this function's purpose is largely cosmetic,
            // the choice has been made to use each language's native features
            // rather than force total conformity.
            int score = 0;
            // One point for non-alphanumeric.
            if (!char.IsLetterOrDigit(one, one.Length - 1)
                || !char.IsLetterOrDigit(two, 0))
            {
                score++;
                // Two points for whitespace.
                if (char.IsWhiteSpace(one, one.Length - 1)
                    || char.IsWhiteSpace(two, 0))
                {
                    score++;
                    // Three points for line breaks.
                    if (char.IsControl(one, one.Length - 1)
                        || char.IsControl(two, 0))
                    {
                        score++;
                        // Four points for blank lines.
                        if (BLANKLINEEND.IsMatch(one)
                            || BLANKLINESTART.IsMatch(two))
                        {
                            score++;
                        }
                    }
                }
            }
            return score;
        }


        private Regex BLANKLINEEND
            = new Regex("\\n\\r?\\n\\Z", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex BLANKLINESTART
            = new Regex("\\A\\r?\\n\\r?\\n", RegexOptions.Singleline | RegexOptions.Compiled);


        /**
         * Reduce the number of edits by eliminating operationally trivial equalities.
         * @param diffs LinkedList of Diff objects
         */
        public void diff_cleanupEfficiency(LinkedList<Diff> diffs)
        {
            if (diffs.Count == 0)
            {
                return;
            }
            bool changes = false;
            Stack<Diff> equalities = new Stack<Diff>();  // Stack of equalities.
            String lastequality = null; // Always equal to equalities.lastElement().text
            ListIterator<Diff> pointer = diffs.ListIterator();
            // Is there an insertion operation before the last equality.
            bool pre_ins = false;
            // Is there a deletion operation before the last equality.
            bool pre_del = false;
            // Is there an insertion operation after the last equality.
            bool post_ins = false;
            // Is there a deletion operation after the last equality.
            bool post_del = false;
            while (pointer.MoveNext())
            {
                Diff thisDiff = pointer.Current;
                Diff safeDiff = thisDiff;  // The last Diff that is known to be unsplitable.
                if (thisDiff.operation == Operation.EQUAL)
                {
                    // equality found
                    if (thisDiff.text.Length < Diff_EditCost && (post_ins || post_del))
                    {
                        // Candidate found.
                        equalities.Push(thisDiff);
                        pre_ins = post_ins;
                        pre_del = post_del;
                        lastequality = thisDiff.text;
                    }
                    else
                    {
                        // Not a candidate, and can never become one.
                        equalities.Clear();
                        lastequality = null;
                        safeDiff = thisDiff;
                    }
                    post_ins = post_del = false;
                }
                else
                {
                    // an insertion or deletion
                    if (thisDiff.operation == Operation.DELETE)
                    {
                        post_del = true;
                    }
                    else
                    {
                        post_ins = true;
                    }
                    /*
                     * Five types to be split:
                     * <ins>A</ins><del>B</del>XY<ins>C</ins><del>D</del>
                     * <ins>A</ins>X<ins>C</ins><del>D</del>
                     * <ins>A</ins><del>B</del>X<ins>C</ins>
                     * <ins>A</del>X<ins>C</ins><del>D</del>
                     * <ins>A</ins><del>B</del>X<del>C</del>
                     */
                    if (lastequality != null
                        && ((pre_ins && pre_del && post_ins && post_del)
                            || ((lastequality.Length < Diff_EditCost / 2)
                                && ((pre_ins ? 1 : 0) + (pre_del ? 1 : 0)
                                    + (post_ins ? 1 : 0) + (post_del ? 1 : 0)) == 3)))
                    {
                        //System.out.println("Splitting: '" + lastequality + "'");
                        // Walk back to offending equality.
                        while (thisDiff != equalities.Peek())
                        {
                            pointer.Previous();
                            thisDiff = pointer.Current;
                        }
                        pointer.MoveNext();

                        // Replace equality with a delete.
                        pointer.Set(new Diff(Operation.DELETE, lastequality));
                        // Insert a corresponding an insert.
                        pointer.Add(thisDiff = new Diff(Operation.INSERT, lastequality));

                        equalities.Pop();  // Throw away the equality we just deleted.
                        lastequality = null;
                        if (pre_ins && pre_del)
                        {
                            // No changes made which could affect previous entry, keep going.
                            post_ins = post_del = true;
                            equalities.Clear();
                            safeDiff = thisDiff;
                        }
                        else
                        {
                            if (equalities.Count != 0)
                            {
                                // Throw away the previous equality (it needs to be reevaluated).
                                equalities.Pop();
                            }
                            if (equalities.Count == 0)
                            {
                                // There are no previous questionable equalities,
                                // walk back to the last known safe diff.
                                thisDiff = safeDiff;
                            }
                            else
                            {
                                // There is an equality we can fall back to.
                                thisDiff = equalities.Peek();
                            }
                            while (thisDiff != pointer.Current && pointer.Previous())
                            {
                                // Intentionally empty loop.
                            }
                            post_ins = post_del = false;
                        }

                        changes = true;
                    }
                }
            }

            if (changes)
            {
                diff_cleanupMerge(diffs);
            }
        }


        /**
         * Reorder and merge like edit sections.  Merge equalities.
         * Any edit section can move as long as it doesn't cross an equality.
         * @param diffs LinkedList of Diff objects
         */
        public void diff_cleanupMerge(LinkedList<Diff> diffs)
        {
            diffs.Add(new Diff(Operation.EQUAL, ""));  // Add a dummy entry at the end.
            ListIterator<Diff> pointer = diffs.ListIterator();
            int count_delete = 0;
            int count_insert = 0;
            String text_delete = "";
            String text_insert = "";

            Diff prevEqual = null;
            Diff thisDiff;
            int commonlength;

            while (pointer.MoveNext())
            {
                thisDiff = pointer.Current;
                switch (thisDiff.operation)
                {
                    case Operation.INSERT:
                        count_insert++;
                        text_insert += thisDiff.text;
                        prevEqual = null;
                        break;
                    case Operation.DELETE:
                        count_delete++;
                        text_delete += thisDiff.text;
                        prevEqual = null;
                        break;
                    case Operation.EQUAL:
                        if (count_delete != 0 || count_insert != 0)
                        {
                            // Delete the offending records.
                            pointer.Previous();  // Reverse direction.
                            while (count_delete-- > 0)
                            {
                                pointer.Previous();
                                pointer.Remove();
                            }
                            while (count_insert-- > 0)
                            {
                                pointer.Previous();
                                pointer.Remove();
                            }
                            if (count_delete != 0 && count_insert != 0)
                            {
                                // Factor out any common prefixies.
                                commonlength = diff_commonPrefix(text_insert, text_delete);
                                if (commonlength != 0)
                                {
                                    if (pointer.HasPrevious)
                                    {
                                        pointer.Previous();
                                        thisDiff = pointer.Current;
                                        Debug.Assert(thisDiff.operation == Operation.EQUAL
                                               , "Previous diff should have been an equality.");
                                        thisDiff.text += text_insert.Substring(0, commonlength);
                                        pointer.MoveNext();
                                    }
                                    else
                                    {
                                        pointer.Add(new Diff(Operation.EQUAL,
                                            text_insert.Substring(0, commonlength)));
                                    }
                                    text_insert = text_insert.Substring(commonlength);
                                    text_delete = text_delete.Substring(commonlength);
                                }
                                // Factor out any common suffixies.
                                commonlength = diff_commonSuffix(text_insert, text_delete);
                                if (commonlength != 0)
                                {
                                    pointer.MoveNext();
                                    thisDiff = pointer.Current;
                                    thisDiff.text = text_insert.Substring(text_insert.Length
                                        - commonlength) + thisDiff.text;
                                    text_insert = text_insert.Substring(0, text_insert.Length
                                        - commonlength);
                                    text_delete = text_delete.Substring(0, text_delete.Length
                                        - commonlength);
                                    pointer.Previous();
                                }
                            }
                            // Insert the merged records.
                            if (text_delete.Length != 0)
                            {
                                pointer.Add(new Diff(Operation.DELETE, text_delete));
                            }
                            if (text_insert.Length != 0)
                            {
                                pointer.Add(new Diff(Operation.INSERT, text_insert));
                            }
                            // Step forward to the equality.
                            thisDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
                        }
                        else if (prevEqual != null)
                        {
                            // Merge this equality with the previous one.
                            prevEqual.text += thisDiff.text;
                            pointer.Remove();
                            pointer.Previous();
                            thisDiff = pointer.Current;
                            pointer.MoveNext();  // Forward direction
                        }
                        count_insert = 0;
                        count_delete = 0;
                        text_delete = "";
                        text_insert = "";
                        prevEqual = thisDiff;
                        break;
                }
                thisDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
            }
            // System.out.println(diff);
            if (diffs.Last.Value.text.Length == 0)
            {
                diffs.RemoveLast();  // Remove the dummy entry at the end.
            }

            /*
             * Second pass: look for single edits surrounded on both sides by equalities
             * which can be shifted sideways to eliminate an equality.
             * e.g: A<ins>BA</ins>C -> <ins>AB</ins>AC
             */
            bool changes = false;
            // Create a new iterator at the start.
            // (As opposed to walking the current one back.)
            pointer = diffs.ListIterator();
            Diff prevDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
            thisDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
            Diff nextDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
            // Intentionally ignore the first and last element (don't need checking).
            while (nextDiff != null)
            {
                if (prevDiff.operation == Operation.EQUAL &&
                    nextDiff.operation == Operation.EQUAL)
                {
                    // This is a single edit surrounded by equalities.
                    if (thisDiff.text.EndsWith(prevDiff.text))
                    {
                        // Shift the edit over the previous equality.
                        thisDiff.text = prevDiff.text
                            + thisDiff.text.Substring(0, thisDiff.text.Length
                                                         - prevDiff.text.Length);
                        nextDiff.text = prevDiff.text + nextDiff.text;
                        pointer.Previous(); // Walk past nextDiff.
                        pointer.Previous(); // Walk past thisDiff.
                        pointer.Previous(); // Walk past prevDiff.
                        pointer.Remove(); // Delete prevDiff.
                        pointer.MoveNext(); // Walk past thisDiff.
                        thisDiff = pointer.MoveNext() ? pointer.Current : null; // Walk past nextDiff.
                        nextDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
                        changes = true;
                    }
                    else if (thisDiff.text.StartsWith(nextDiff.text))
                    {
                        // Shift the edit over the next equality.
                        prevDiff.text += nextDiff.text;
                        thisDiff.text = thisDiff.text.Substring(nextDiff.text.Length)
                            + nextDiff.text;
                        pointer.Remove(); // Delete nextDiff.
                        nextDiff = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
                        changes = true;
                    }
                }
                prevDiff = thisDiff;
                thisDiff = nextDiff;
                nextDiff = pointer.MoveNext() ? pointer.Current : null;
            }
            // If shifts were made, the diff needs reordering and another shift sweep.
            if (changes)
            {
                diff_cleanupMerge(diffs);
            }
        }


        /**
         * loc is a location in text1, compute and return the equivalent location in
         * text2.
         * e.g. "The cat" vs "The big cat", 1->1, 5->8
         * @param diffs LinkedList of Diff objects
         * @param loc Location within text1
         * @return Location within text2
         */
        public int diff_xIndex(LinkedList<Diff> diffs, int loc)
        {
            int chars1 = 0;
            int chars2 = 0;
            int last_chars1 = 0;
            int last_chars2 = 0;
            Diff lastDiff = null;
            foreach (Diff aDiff in diffs)
            {
                if (aDiff.operation != Operation.INSERT)
                {
                    // Equality or deletion.
                    chars1 += aDiff.text.Length;
                }
                if (aDiff.operation != Operation.DELETE)
                {
                    // Equality or insertion.
                    chars2 += aDiff.text.Length;
                }
                if (chars1 > loc)
                {
                    // Overshot the location.
                    lastDiff = aDiff;
                    break;
                }
                last_chars1 = chars1;
                last_chars2 = chars2;
            }
            if (lastDiff != null && lastDiff.operation == Operation.DELETE)
            {
                // The location was deleted.
                return last_chars2;
            }
            // Add the remaining character length.
            return last_chars2 + (loc - last_chars1);
        }


        /**
         * Convert a Diff list into a pretty HTML report.
         * @param diffs LinkedList of Diff objects
         * @return HTML representation
         */
        public String diff_prettyHtml(LinkedList<Diff> diffs)
        {
            StringBuilder html = new StringBuilder();
            int i = 0;
            foreach (Diff aDiff in diffs)
            {
                String text = aDiff.text.Replace("&", "&amp;").Replace("<", "&lt;")
                    .Replace(">", "&gt;").Replace("\n", "&para;<BR>");
                switch (aDiff.operation)
                {
                    case Operation.INSERT:
                        html.Append("<INS STYLE=\"background:#E6FFE6;\" TITLE=\"i=" + i + "\">"
                            + text + "</INS>");
                        break;
                    case Operation.DELETE:
                        html.Append("<DEL STYLE=\"background:#FFE6E6;\" TITLE=\"i=" + i + "\">"
                            + text + "</DEL>");
                        break;
                    case Operation.EQUAL:
                        html.Append("<SPAN TITLE=\"i=" + i + "\">" + text + "</SPAN>");
                        break;
                }
                if (aDiff.operation != Operation.DELETE)
                {
                    i += aDiff.text.Length;
                }
            }
            return html.ToString();
        }

        /**
         * Compute and return the source text (all equalities and deletions).
         * @param diffs LinkedList of Diff objects
         * @return Source text
         */
        public String diff_text1(LinkedList<Diff> diffs)
        {
            StringBuilder txt = new StringBuilder();
            foreach (Diff aDiff in diffs)
            {
                if (aDiff.operation != Operation.INSERT)
                {
                    txt.Append(aDiff.text);
                }
            }
            return txt.ToString();
        }

        /**
         * Compute and return the destination text (all equalities and insertions).
         * @param diffs LinkedList of Diff objects
         * @return Destination text
         */
        public String diff_text2(LinkedList<Diff> diffs)
        {
            StringBuilder txt = new StringBuilder();
            foreach (Diff aDiff in diffs)
            {
                if (aDiff.operation != Operation.DELETE)
                {
                    txt.Append(aDiff.text);
                }
            }
            return txt.ToString();
        }


        /**
         * Crush the diff into an encoded string which describes the operations
         * required to transform text1 into text2.
         * E.g. =3\t-2\t+ing  -> Keep 3 chars, delete 2 chars, insert 'ing'.
         * Operations are tab-separated.  Inserted text is escaped using %xx notation.
         * @param diffs Array of diff tuples
         * @return Delta text
         */
        public String diff_toDelta(LinkedList<Diff> diffs)
        {
            StringBuilder txt = new StringBuilder();
            foreach (Diff aDiff in diffs)
            {
                switch (aDiff.operation)
                {
                    case Operation.INSERT:
                        txt.Append("+" + Uri.EscapeDataString(aDiff.text).Replace('+', ' ') + "\t");
                        break;
                    case Operation.DELETE:
                        txt.Append("-" + aDiff.text.Length + "\t");
                        break;
                    case Operation.EQUAL:
                        txt.Append("=" + aDiff.text.Length + "\t");
                        break;
                }
            }
            String delta = txt.ToString();
            if (delta.Length != 0)
            {
                // Strip off trailing tab character.
                delta = delta.Substring(0, delta.Length - 1);
                // Unescape selected chars for compatability with JavaScript's encodeURI.
                // In speed critical applications this could be dropped since the
                // receiving application will probably decode these fine.
                delta = delta.Replace("%21", "!").Replace("%7E", "~")
                    .Replace("%27", "'").Replace("%28", "(").Replace("%29", ")")
                    .Replace("%3B", ";").Replace("%2F", "/").Replace("%3F", "?")
                    .Replace("%3A", ":").Replace("%40", "@").Replace("%26", "&")
                    .Replace("%3D", "=").Replace("%2B", "+").Replace("%24", "$")
                    .Replace("%2C", ",").Replace("%23", "#");
            }
            return delta;
        }


        /**
         * Given the original text1, and an encoded string which describes the
         * operations required to transform text1 into text2, compute the full diff.
         * @param text1 Source string for the diff
         * @param delta Delta text
         * @return Array of diff tuples or null if invalid
         * @throw IllegalArgumentException If invalid input
         */
        public LinkedList<Diff> diff_fromDelta(String text1, String delta)
        {
            LinkedList<Diff> diffs = new LinkedList<Diff>();
            int pointer = 0;  // Cursor in text1
            String[] tokens = delta.Split('\t');
            foreach (String token in tokens)
            {
                if (token.Length == 0)
                {
                    // Blank tokens are ok (from a trailing \t).
                    continue;
                }
                // Each token begins with a one character parameter which specifies the
                // operation of this token (delete, insert, equality).
                String param = token.Substring(1);
                switch (token[0])
                {
                    case '+':
                        // decode would change all "+" to " "
                        param = param.Replace("+", "%2B");

                        param = Uri.UnescapeDataString(param);
                        diffs.Add(new Diff(Operation.INSERT, param));
                        break;
                    case '-':
                    // Fall through.
                    case '=':
                        int n;
                        try
                        {
                            n = int.Parse(param);
                        }
                        catch (FormatException e)
                        {
                            throw new ArgumentException(
                                "Invalid number in diff_fromDelta: " + param, e);
                        }
                        if (n < 0)
                        {
                            throw new ArgumentException(
                                "Negative number in diff_fromDelta: " + param);
                        }
                        String text;
                        try
                        {
                            text = text1.Substring(pointer, pointer += n);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            throw new ArgumentException("Delta length (" + pointer
                                + ") larger than source text length (" + text1.Length
                                + ").", e);
                        }
                        if (token[0] == '=')
                        {
                            diffs.Add(new Diff(Operation.EQUAL, text));
                        }
                        else
                        {
                            diffs.Add(new Diff(Operation.DELETE, text));
                        }
                        break;
                    default:
                        // Anything else is an error.
                        throw new ArgumentException(
                            "Invalid diff operation in diff_fromDelta: " + token[0]);
                }
            }
            if (pointer != text1.Length)
            {
                throw new ArgumentException("Delta length (" + pointer
                    + ") smaller than source text length (" + text1.Length + ").");
            }
            return diffs;
        }


        //  MATCH FUNCTIONS


        /**
         * Locate the best instance of 'pattern' in 'text' near 'loc'.
         * Returns -1 if no match found.
         * @param text The text to search
         * @param pattern The pattern to search for
         * @param loc The location to search around
         * @return Best match index or -1
         */
        public int match_main(String text, String pattern, int loc)
        {
            loc = Math.Max(0, Math.Min(loc, text.Length - pattern.Length));
            if (text.Equals(pattern))
            {
                // Shortcut (potentially not guaranteed by the algorithm)
                return 0;
            }
            else if (text.Length == 0)
            {
                // Nothing to match.
                return -1;
            }
            else if (text.Substring(loc, loc + pattern.Length).Equals(pattern))
            {
                // Perfect match at the perfect spot!  (Includes case of null pattern)
                return loc;
            }
            else
            {
                // Do a fuzzy compare.
                return match_bitap(text, pattern, loc);
            }
        }


        /**
         * Locate the best instance of 'pattern' in 'text' near 'loc' using the
         * Bitap algorithm.  Returns -1 if no match found.
         * @param text The text to search
         * @param pattern The pattern to search for
         * @param loc The location to search around
         * @return Best match index or -1
         */
        protected int match_bitap(String text, String pattern, int loc)
        {
            Debug.Assert(Match_MaxBits == 0 || pattern.Length <= Match_MaxBits,
                "Pattern too long for this application.");

            // Initialise the alphabet.
            Dictionary<char, int> s = match_alphabet(pattern);

            int score_text_length = text.Length;
            // Coerce the text length between reasonable maximums and minimums.
            score_text_length = Math.Max(score_text_length, Match_MinLength);
            score_text_length = Math.Min(score_text_length, Match_MaxLength);

            // Highest score beyond which we give up.
            double score_threshold = Match_Threshold;
            // Is there a nearby exact match? (speedup)
            int best_loc = text.IndexOf(pattern, loc);
            if (best_loc != -1)
            {
                score_threshold = Math.Min(match_bitapScore(0, best_loc, loc,
                    score_text_length, pattern), score_threshold);
            }
            // What about in the other direction? (speedup)
            best_loc = text.LastIndexOf(pattern, loc + pattern.Length);
            if (best_loc != -1)
            {
                score_threshold = Math.Min(match_bitapScore(0, best_loc, loc,
                    score_text_length, pattern), score_threshold);
            }

            // Initialise the bit arrays.
            int matchmask = 1 << (pattern.Length - 1);
            best_loc = -1;

            int bin_min, bin_mid;
            int bin_max = Math.Max(loc + loc, text.Length);
            // Empty initialization added to appease Java compiler.
            int[] last_rd = new int[0];
            for (int d = 0; d < pattern.Length; d++)
            {
                // Scan for the best match; each iteration allows for one more error.
                int[] rd = new int[text.Length];

                // Run a binary search to determine how far from 'loc' we can stray at
                // this error level.
                bin_min = loc;
                bin_mid = bin_max;
                while (bin_min < bin_mid)
                {
                    if (match_bitapScore(d, bin_mid, loc, score_text_length, pattern)
                        < score_threshold)
                    {
                        bin_min = bin_mid;
                    }
                    else
                    {
                        bin_max = bin_mid;
                    }
                    bin_mid = (bin_max - bin_min) / 2 + bin_min;
                }
                // Use the result from this iteration as the maximum for the next.
                bin_max = bin_mid;
                int start = Math.Max(0, loc - (bin_mid - loc) - 1);
                int finish = Math.Min(text.Length - 1, pattern.Length + bin_mid);

                if (text[finish] == pattern[pattern.Length - 1])
                {
                    rd[finish] = (1 << (d + 1)) - 1;
                }
                else
                {
                    rd[finish] = (1 << d) - 1;
                }
                for (int j = finish - 1; j >= start; j--)
                {
                    if (d == 0)
                    {
                        // First pass: exact match.
                        rd[j] = ((rd[j + 1] << 1) | 1) & (s.ContainsKey(text[j])
                            ? s[text[j]]
                            : 0);
                    }
                    else
                    {
                        // Subsequent passes: fuzzy match.
                        rd[j] = ((rd[j + 1] << 1) | 1) & (s.ContainsKey(text[j])
                            ? s[text[j]] : 0) | ((last_rd[j + 1] << 1) | 1)
                            | ((last_rd[j] << 1) | 1) | last_rd[j + 1];
                    }
                    if ((rd[j] & matchmask) != 0)
                    {
                        double score = match_bitapScore(d, j, loc, score_text_length,
                                                        pattern);
                        // This match will almost certainly be better than any existing
                        // match.  But check anyway.
                        if (score <= score_threshold)
                        {
                            // Told you so.
                            score_threshold = score;
                            best_loc = j;
                            if (j > loc)
                            {
                                // When passing loc, don't exceed our current distance from loc.
                                start = Math.Max(0, loc - (j - loc));
                            }
                            else
                            {
                                // Already passed loc, downhill from here on in.
                                break;
                            }
                        }
                    }
                }
                if (match_bitapScore(d + 1, loc, loc, score_text_length, pattern)
                    > score_threshold)
                {
                    // No hope for a (better) match at greater error levels.
                    break;
                }
                last_rd = rd;
            }
            return best_loc;
        }


        /**
         * Compute and return the score for a match with e errors and x location.
         * @param e Number of errors in match
         * @param x Location of match
         * @param loc Expected location of match
         * @param score_text_length Coerced version of text's length
         * @param pattern Pattern being sought
         * @return Overall score for match
         */
        private double match_bitapScore(int e, int x, int loc,
                                        int score_text_length, String pattern)
        {
            int d = Math.Abs(loc - x);
            return (e / (float)pattern.Length / Match_Balance)
                + (d / (float)score_text_length / (1.0 - Match_Balance));
        }


        /**
         * Initialise the alphabet for the Bitap algorithm.
         * @param pattern The text to encode
         * @return Hash of character locations
         */
        protected Dictionary<char, int> match_alphabet(String pattern)
        {
            Dictionary<char, int> s = new Dictionary<char, int>();
            char[] char_pattern = pattern.ToCharArray();
            foreach (char c in char_pattern)
            {
                s[c] = 0;
            }
            int i = 0;
            foreach (char c in char_pattern)
            {
                s[c] = s[c] | (1 << (pattern.Length - i - 1));
                i++;
            }
            return s;
        }


        //  PATCH FUNCTIONS


        /**
         * Increase the context until it is unique,
         * but don't let the pattern expand beyond Match_MaxBits.
         * @param patch The patch to grow
         * @param text Source text
         */
        protected void patch_addContext(Patch patch, String text)
        {
            String pattern = text.Substring(patch.start2, patch.start2 + patch.length1);
            int padding = 0;
            // Increase the context until we're unique (but don't let the pattern
            // expand beyond Match_MaxBits).
            while (text.IndexOf(pattern) != text.LastIndexOf(pattern)
                && pattern.Length < Match_MaxBits - Patch_Margin - Patch_Margin)
            {
                padding += Patch_Margin;
                pattern = text.Substring(Math.Max(0, patch.start2 - padding),
                    Math.Min(text.Length, patch.start2 + patch.length1 + padding));
            }
            // Add one chunk for good luck.
            padding += Patch_Margin;
            // Add the prefix.
            String prefix = text.Substring(Math.Max(0, patch.start2 - padding),
                patch.start2);
            if (prefix.Length != 0)
            {
                patch.diffs.AddFirst(new Diff(Operation.EQUAL, prefix));
            }
            // Add the suffix.
            String suffix = text.Substring(patch.start2 + patch.length1,
                Math.Min(text.Length, patch.start2 + patch.length1 + padding));
            if (suffix.Length != 0)
            {
                patch.diffs.AddLast(new Diff(Operation.EQUAL, suffix));
            }

            // Roll back the start points.
            patch.start1 -= prefix.Length;
            patch.start2 -= prefix.Length;
            // Extend the lengths.
            patch.length1 += prefix.Length + suffix.Length;
            patch.length2 += prefix.Length + suffix.Length;
        }


        /**
         * Compute a list of patches to turn text1 into text2.
         * A set of diffs will be computed.
         * @param text1 Old text
         * @param text2 New text
         * @return LinkedList of Patch objects.
         */
        public LinkedList<Patch> patch_make(String text1, String text2)
        {
            // No diffs provided, compute our own.
            LinkedList<Diff> diffs = diff_main(text1, text2, true);
            if (diffs.Count > 2)
            {
                diff_cleanupSemantic(diffs);
                diff_cleanupEfficiency(diffs);
            }
            return patch_make(text1, text2, diffs);
        }


        /**
         * Compute a list of patches to turn text1 into text2.
         * text1 and text2 will be derived from the provided diff.
         * @param diffs Array of diff tuples for text1 to text2.
         * @return LinkedList of Patch objects.
         */
        public LinkedList<Patch> patch_make(LinkedList<Diff> diffs)
        {
            // No orgin strings provided, compute our own.
            String text1 = diff_text1(diffs);
            String text2 = "";  // text2 isn't actually used.
            return patch_make(text1, text2, diffs);
        }


        /**
         * Compute a list of patches to turn text1 into text2.
         * Use the diffs provided.
         * @param text1 Old text
         * @param text2 New text
         * @param diffs Optional array of diff tuples for text1 to text2.
         * @return LinkedList of Patch objects.
         */
        public LinkedList<Patch> patch_make(String text1, String text2,
                                            LinkedList<Diff> diffs)
        {
            LinkedList<Patch> patches = new LinkedList<Patch>();
            if (diffs.Count == 0)
            {
                return patches;  // Get rid of the null case.
            }
            Patch patch = new Patch();
            int char_count1 = 0;  // Number of characters into the text1 string.
            int char_count2 = 0;  // Number of characters into the text2 string.
            // Recreate the patches to determine context info.
            String prepatch_text = text1;
            String postpatch_text = text1;
            foreach (Diff aDiff in diffs)
            {
                if (patch.diffs.Count == 0 && aDiff.operation != Operation.EQUAL)
                {
                    // A new patch starts here.
                    patch.start1 = char_count1;
                    patch.start2 = char_count2;
                }

                switch (aDiff.operation)
                {
                    case Operation.INSERT:
                        patch.diffs.Add(aDiff);
                        patch.length2 += aDiff.text.Length;
                        postpatch_text = postpatch_text.Substring(0, char_count2)
                            + aDiff.text + postpatch_text.Substring(char_count2);
                        break;
                    case Operation.DELETE:
                        patch.length1 += aDiff.text.Length;
                        patch.diffs.Add(aDiff);
                        postpatch_text = postpatch_text.Substring(0, char_count2)
                            + postpatch_text.Substring(char_count2 + aDiff.text.Length);
                        break;
                    case Operation.EQUAL:
                        if (aDiff.text.Length <= 2 * Patch_Margin
                            && patch.diffs.Count == 0 && aDiff != diffs.Last.Value)
                        {
                            // Small equality inside a patch.
                            patch.diffs.Add(aDiff);
                            patch.length1 += aDiff.text.Length;
                            patch.length2 += aDiff.text.Length;
                        }

                        if (aDiff.text.Length >= 2 * Patch_Margin)
                        {
                            // Time for a new patch.
                            if (patch.diffs.Count > 0)
                            {
                                patch_addContext(patch, prepatch_text);
                                patches.Add(patch);
                                patch = new Patch();
                                prepatch_text = postpatch_text;
                            }
                        }
                        break;
                }

                // Update the current character count.
                if (aDiff.operation != Operation.INSERT)
                {
                    char_count1 += aDiff.text.Length;
                }
                if (aDiff.operation != Operation.DELETE)
                {
                    char_count2 += aDiff.text.Length;
                }
            }
            // Pick up the leftover patch if not empty.
            if (patch.diffs.Count > 0)
            {
                patch_addContext(patch, prepatch_text);
                patches.Add(patch);
            }

            return patches;
        }

        public class ApplyResult
        {
            readonly string _text;
            readonly bool[] _results;

            public ApplyResult(string text, bool[] results)
            {
                _text = text;
                _results = results;
            }

            public string Text
            {
                get { return _text; }
            }

            public bool[] Results
            {
                get { return _results; }
            }
        }

        /**
         * Merge a set of patches onto the text.  Return a patched text, as well
         * as an array of true/false values indicating which patches were applied.
         * @param patches Array of patch objects
         * @param text Old text
         * @return Two element Object array, containing the new text and an array of
         *      bool values
         */
        public ApplyResult patch_apply(LinkedList<Patch> patches, String text)
        {
            if (patches.Count == 0)
            {
                return new ApplyResult(text, new bool[0]);
            }

            // Deep copy the patches so that no changes are made to originals.
            LinkedList<Patch> patchesCopy = new LinkedList<Patch>();
            foreach (Patch aPatch in patches)
            {
                Patch patchCopy = new Patch();
                foreach (Diff aDiff in aPatch.diffs)
                {
                    Diff diffCopy = new Diff(aDiff.operation, aDiff.text);
                    patchCopy.diffs.Add(diffCopy);
                }
                patchCopy.start1 = aPatch.start1;
                patchCopy.start2 = aPatch.start2;
                patchCopy.length1 = aPatch.length1;
                patchCopy.length2 = aPatch.length2;
                patchesCopy.Add(patchCopy);
            }
            patches = patchesCopy;

            String nullPadding = this.patch_addPadding(patches);
            text = nullPadding + text + nullPadding;
            patch_splitMax(patches);

            int x = 0;
            // delta keeps track of the offset between the expected and actual location
            // of the previous patch.  If there are patches expected at positions 10 and
            // 20, but the first patch was found at 12, delta is 2 and the second patch
            // has an effective expected position of 22.
            int delta = 0;
            bool[] results = new bool[patches.Count];
            int expected_loc, start_loc;
            String text1, text2;
            int index1, index2;
            foreach (Patch aPatch in patches)
            {
                expected_loc = aPatch.start2 + delta;
                text1 = diff_text1(aPatch.diffs);
                start_loc = match_main(text, text1, expected_loc);
                if (start_loc == -1)
                {
                    // No match found.  :(
                    results[x] = false;
                }
                else
                {
                    // Found a match.  :)
                    results[x] = true;
                    delta = start_loc - expected_loc;
                    text2 = text.Substring(start_loc,
                        Math.Min(start_loc + text1.Length, text.Length));
                    if (text1.Equals(text2))
                    {
                        // Perfect match, just shove the replacement text in.
                        text = text.Substring(0, start_loc) + diff_text2(aPatch.diffs)
                            + text.Substring(start_loc + text1.Length);
                    }
                    else
                    {
                        // Imperfect match.  Run a diff to get a framework of equivalent
                        // indicies.
                        LinkedList<Diff> diffs = diff_main(text1, text2, false);
                        diff_cleanupSemanticLossless(diffs);
                        index1 = 0;
                        foreach (Diff aDiff in aPatch.diffs)
                        {
                            if (aDiff.operation != Operation.EQUAL)
                            {
                                index2 = diff_xIndex(diffs, index1);
                                if (aDiff.operation == Operation.INSERT)
                                {
                                    // Insertion
                                    text = text.Substring(0, start_loc + index2) + aDiff.text
                                        + text.Substring(start_loc + index2);
                                }
                                else if (aDiff.operation == Operation.DELETE)
                                {
                                    // Deletion
                                    text = text.Substring(0, start_loc + index2)
                                        + text.Substring(start_loc + diff_xIndex(diffs,
                                        index1 + aDiff.text.Length));
                                }
                            }
                            if (aDiff.operation != Operation.DELETE)
                            {
                                index1 += aDiff.text.Length;
                            }
                        }
                    }
                }
                x++;
            }
            // Strip the padding off.
            text = text.Substring(nullPadding.Length, text.Length
                - nullPadding.Length);
            return new ApplyResult(text, results);
        }

        /**
         * Add some padding on text start and end so that edges can match something.
         * @param patches Array of patch objects
         * @return The padding string added to each side.
         */
        protected String patch_addPadding(LinkedList<Patch> patches)
        {
            LinkedList<Diff> diffs;
            String nullPadding = "";
            for (int x = 0; x < this.Patch_Margin; x++)
            {
                nullPadding += x.ToString(CultureInfo.InvariantCulture);
            }

            // Bump all the patches forward.
            foreach (Patch aPatch in patches)
            {
                aPatch.start1 += nullPadding.Length;
                aPatch.start2 += nullPadding.Length;
            }

            // Add some padding on start of first diff.
            Patch patch = patches.First.Value;
            diffs = patch.diffs;
            if (diffs.Count == 0 || diffs.First.Value.operation != Operation.EQUAL)
            {
                // Add nullPadding equality.
                diffs.AddFirst(new Diff(Operation.EQUAL, nullPadding));
                patch.start1 -= nullPadding.Length;  // Should be 0.
                patch.start2 -= nullPadding.Length;  // Should be 0.
                patch.length1 += nullPadding.Length;
                patch.length2 += nullPadding.Length;
            }
            else if (nullPadding.Length > diffs.First.Value.text.Length)
            {
                // Grow first equality.
                Diff firstDiff = diffs.First.Value;
                int extraLength = nullPadding.Length - firstDiff.text.Length;
                firstDiff.text = nullPadding.Substring(firstDiff.text.Length)
                    + firstDiff.text;
                patch.start1 -= extraLength;
                patch.start2 -= extraLength;
                patch.length1 += extraLength;
                patch.length2 += extraLength;
            }

            // Add some padding on end of last diff.
            patch = patches.Last.Value;
            diffs = patch.diffs;
            if (diffs.Count == 0 || diffs.Last.Value.operation != Operation.EQUAL)
            {
                // Add nullPadding equality.
                diffs.AddLast(new Diff(Operation.EQUAL, nullPadding));
                patch.length1 += nullPadding.Length;
                patch.length2 += nullPadding.Length;
            }
            else if (nullPadding.Length > diffs.Last.Value.text.Length)
            {
                // Grow last equality.
                Diff lastDiff = diffs.Last.Value;
                int extraLength = nullPadding.Length - lastDiff.text.Length;
                lastDiff.text += nullPadding.Substring(0, extraLength);
                patch.length1 += extraLength;
                patch.length2 += extraLength;
            }

            return nullPadding;
        }

        /**
         * Look through the patches and break up any which are longer than the
         * maximum limit of the match algorithm.
         * @param patches LinkedList of Patch objects.
         */
        public void patch_splitMax(LinkedList<Patch> patches)
        {
            int patch_size;
            String precontext, postcontext;
            Patch patch;
            int start1, start2;
            bool empty;
            Operation diff_type;
            String diff_text;
            ListIterator<Patch> pointer = patches.ListIterator();
            Patch bigpatch = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
            while (bigpatch != null)
            {
                if (bigpatch.length1 <= Match_MaxBits)
                {
                    bigpatch = (pointer.HasNext && pointer.MoveNext()) ? pointer.Current : null;
                    continue;
                }
                // Remove the big old patch.
                pointer.Remove();
                patch_size = Match_MaxBits;
                start1 = bigpatch.start1;
                start2 = bigpatch.start2;
                precontext = "";
                while (bigpatch.diffs.Count > 0)
                {
                    // Create one of several smaller patches.
                    patch = new Patch();
                    empty = true;
                    patch.start1 = start1 - precontext.Length;
                    patch.start2 = start2 - precontext.Length;
                    if (precontext.Length != 0)
                    {
                        patch.length1 = patch.length2 = precontext.Length;
                        patch.diffs.Add(new Diff(Operation.EQUAL, precontext));
                    }
                    while (bigpatch.diffs.Count > 0
                        && patch.length1 < patch_size - Patch_Margin)
                    {
                        diff_type = bigpatch.diffs.First.Value.operation;
                        diff_text = bigpatch.diffs.First.Value.text;
                        if (diff_type == Operation.INSERT)
                        {
                            // Insertions are harmless.
                            patch.length2 += diff_text.Length;
                            start2 += diff_text.Length;
                            patch.diffs.AddLast(bigpatch.diffs.First.Value);
                            bigpatch.diffs.RemoveFirst();
                            empty = false;
                        }
                        else
                        {
                            // Deletion or equality.  Only take as much as we can stomach.
                            diff_text = diff_text.Substring(0, Math.Min(diff_text.Length,
                                patch_size - patch.length1 - Patch_Margin));
                            patch.length1 += diff_text.Length;
                            start1 += diff_text.Length;
                            if (diff_type == Operation.EQUAL)
                            {
                                patch.length2 += diff_text.Length;
                                start2 += diff_text.Length;
                            }
                            else
                            {
                                empty = false;
                            }
                            patch.diffs.Add(new Diff(diff_type, diff_text));
                            if (diff_text.Equals(bigpatch.diffs.First.Value.text))
                            {
                                bigpatch.diffs.RemoveFirst();
                            }
                            else
                            {
                                bigpatch.diffs.First.Value.text = bigpatch.diffs.First.Value.text
                                    .Substring(diff_text.Length);
                            }
                        }
                    }
                    // Compute the head context for the next patch.
                    precontext = diff_text2(patch.diffs);
                    precontext = precontext.Substring(Math.Max(0, precontext.Length
                        - Patch_Margin));
                    // Append the end context for this patch.
                    if (diff_text1(bigpatch.diffs).Length > Patch_Margin)
                    {
                        postcontext = diff_text1(bigpatch.diffs).Substring(0, Patch_Margin);
                    }
                    else
                    {
                        postcontext = diff_text1(bigpatch.diffs);
                    }
                    if (postcontext.Length != 0)
                    {
                        patch.length1 += postcontext.Length;
                        patch.length2 += postcontext.Length;
                        if (patch.diffs.Count > 0
                            && patch.diffs.Last.Value.operation == Operation.EQUAL)
                        {
                            patch.diffs.Last.Value.text += postcontext;
                        }
                        else
                        {
                            patch.diffs.Add(new Diff(Operation.EQUAL, postcontext));
                        }
                    }
                    if (!empty)
                    {
                        pointer.Add(patch);
                    }
                }
                bigpatch = pointer.MoveNext() ? pointer.Current : null;
            }
        }


        /**
         * Take a list of patches and return a textual representation.
         * @param patches List of Patch objects.
         * @return Text representation of patches.
         */
        public String patch_toText(List<Patch> patches)
        {
            StringBuilder text = new StringBuilder();
            foreach (Patch aPatch in patches)
            {
                text.Append(aPatch);
            }
            return text.ToString();
        }


        /**
         * Parse a textual representation of patches and return a List of Patch
         * objects.
         * @param textline Text representation of patches
         * @return List of Patch objects.
         * @throws IllegalArgumentException If invalid input
         */
        public List<Patch> patch_fromText(String textline)
        {
            List<Patch> patches = new List<Patch>();
            if (textline.Length == 0)
            {
                return patches;
            }
            List<String> textList = new List<string>(textline.Split('\n'));
            LinkedList<String> text = new LinkedList<String>(textList);
            Patch patch;
            Regex patchHeader
                = new Regex("^@@ -(\\d+),?(\\d*) \\+(\\d+),?(\\d*) @@$");
            Match m;
            char sign;
            String line;
            while (text.Count > 0)
            {
                m = patchHeader.Match(text.First.Value);
                if (!m.Success)
                {
                    throw new ArgumentException(
                        "Invalid patch string: " + text.First.Value);
                }
                patch = new Patch();
                patches.Add(patch);
                patch.start1 = int.Parse(m.Groups[1].Value);
                if (m.Groups[2].Length == 0)
                {
                    patch.start1--;
                    patch.length1 = 1;
                }
                else if (m.Groups[2].Value.Equals("0"))
                {
                    patch.length1 = 0;
                }
                else
                {
                    patch.start1--;
                    patch.length1 = int.Parse(m.Groups[2].Value);
                }

                patch.start2 = int.Parse(m.Groups[3].Value);
                if (m.Groups[4].Length == 0)
                {
                    patch.start2--;
                    patch.length2 = 1;
                }
                else if (m.Groups[4].Equals("0"))
                {
                    patch.length2 = 0;
                }
                else
                {
                    patch.start2--;
                    patch.length2 = int.Parse(m.Groups[4].Value);
                }
                text.RemoveFirst();

                while (text.Count > 0)
                {
                    try
                    {
                        sign = text.First.Value[0];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Blank line?  Whatever.
                        text.RemoveFirst();
                        continue;
                    }
                    line = text.First.Value.Substring(1);
                    line = line.Replace("+", "%2B");  // decode would change all "+" to " "

                    line = Uri.UnescapeDataString(line);

                    if (sign == '-')
                    {
                        // Deletion.
                        patch.diffs.Add(new Diff(Operation.DELETE, line));
                    }
                    else if (sign == '+')
                    {
                        // Insertion.
                        patch.diffs.Add(new Diff(Operation.INSERT, line));
                    }
                    else if (sign == ' ')
                    {
                        // Minor equality.
                        patch.diffs.Add(new Diff(Operation.EQUAL, line));
                    }
                    else if (sign == '@')
                    {
                        // Start of next patch.
                        break;
                    }
                    else
                    {
                        // WTF?
                        throw new ArgumentException(
                            "Invalid patch mode '" + sign + "' in: " + line);
                    }
                    text.RemoveFirst();
                }
            }
            return patches;
        }
    }

    /**
     * Class representing one diff operation.
     */
    public sealed class Diff
    {
        public Operation operation;
        // One of: INSERT, DELETE or EQUAL.
        public String text;
        // The text associated with this diff operation.

        /**
         * Constructor.  Initializes the diff with the provided values.
         * @param operation One of INSERT, DELETE or EQUAL
         * @param text The text being applied
         */
        public Diff(Operation operation, String text)
        {
            // Construct a diff with the specified operation and text.
            this.operation = operation;
            this.text = text;
        }

        /**
         * Display a human-readable version of this Diff.
         * @return text version
         */
        public override String ToString()
        {
            String prettyText = this.text.Replace('\n', '\u00b6');
            return "Diff(" + this.operation + ",\"" + prettyText + "\")";
        }

        /**
         * Is this Diff equivalent to another Diff?
         * @param d Another Diff to compare against
         * @return true or false
         */
        public override bool Equals(Object d)
        {
            return Equals(d as Diff);
        }

        public bool Equals(Diff d)
        {
            if (d == null)
                return false;

            return d.operation == this.operation && d.text == this.text;
        }

        public override int GetHashCode()
        {
            return operation.GetHashCode() ^ text.GetHashCode();
        }
    }


    /**
     * Class representing one patch operation.
     */
    public sealed class Patch
    {
        public LinkedList<Diff> diffs;
        public int start1;
        public int start2;
        public int length1;
        public int length2;

        /**
         * Constructor.  Initializes with an empty list of diffs.
         */
        public Patch()
        {
            this.diffs = new LinkedList<Diff>();
        }

        /**
         * Emmulate GNU diff's format.
         * Header: @@ -382,8 +481,9 @@
         * Indicies are printed as 1-based, not 0-based.
         * @return The GNU diff string
         */
        public override String ToString()
        {
            String coords1, coords2;
            if (this.length1 == 0)
            {
                coords1 = this.start1 + ",0";
            }
            else if (this.length1 == 1)
            {
                coords1 = (this.start1 + 1).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                coords1 = (this.start1 + 1) + "," + this.length1;
            }
            if (this.length2 == 0)
            {
                coords2 = this.start2 + ",0";
            }
            else if (this.length2 == 1)
            {
                coords2 = (this.start2 + 1).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                coords2 = (this.start2 + 1) + "," + this.length2;
            }
            StringBuilder txt = new StringBuilder();
            txt.Append("@@ -" + coords1 + " +" + coords2 + " @@\n");
            // Escape the body of the patch with %xx notation.
            foreach (Diff aDiff in this.diffs)
            {
                switch (aDiff.operation)
                {
                    case Operation.INSERT:
                        txt.Append('+');
                        break;
                    case Operation.DELETE:
                        txt.Append('-');
                        break;
                    case Operation.EQUAL:
                        txt.Append(' ');
                        break;
                }

                txt.Append(Uri.EscapeDataString(aDiff.text).Replace('+', ' ')
                    + "\n");
            }
            // Unescape selected chars for compatability with JavaScript's encodeURI.
            // In speed critical applications this could be dropped since the
            // receiving application will probably decode these fine.
            txt.Replace("%21", "!").Replace("%7E", "~")
                .Replace("%27", "'").Replace("%28", "(").Replace("%29", ")")
                .Replace("%3B", ";").Replace("%2F", "/").Replace("%3F", "?")
                .Replace("%3A", ":").Replace("%40", "@").Replace("%26", "&")
                .Replace("%3D", "=").Replace("%2B", "+").Replace("%24", "$")
                .Replace("%2C", ",").Replace("%23", "#");
            return txt.ToString();
        }
    }
}

