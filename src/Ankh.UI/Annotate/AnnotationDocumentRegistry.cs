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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.Annotate
{
    public static class AnnotationDocumentRegistry
    {
        static readonly object _syncRoot = new object();
        static readonly Dictionary<string, AnnotationDocument> _documents =
            new Dictionary<string, AnnotationDocument>(StringComparer.OrdinalIgnoreCase);

        public static void Register(
            string fileName,
            IAnkhServiceProvider context,
            SvnOrigin origin,
            Collection<SvnBlameEventArgs> blameResult)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (origin == null)
                throw new ArgumentNullException(nameof(origin));
            if (blameResult == null)
                throw new ArgumentNullException(nameof(blameResult));

            string key = NormalizePath(fileName);
            lock (_syncRoot)
            {
                _documents[key] = new AnnotationDocument(context, origin, blameResult);
            }
        }

        internal static bool TryGet(string fileName, out AnnotationDocument document)
        {
            document = null;
            if (string.IsNullOrEmpty(fileName))
                return false;

            string key = NormalizePath(fileName);
            lock (_syncRoot)
            {
                return _documents.TryGetValue(key, out document);
            }
        }

        internal static void Unregister(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            string key = NormalizePath(fileName);
            lock (_syncRoot)
            {
                _documents.Remove(key);
            }
        }

        static string NormalizePath(string fileName)
        {
            return Path.GetFullPath(fileName).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }

    internal sealed class AnnotationDocument
    {
        public AnnotationDocument(
            IAnkhServiceProvider context,
            SvnOrigin origin,
            Collection<SvnBlameEventArgs> blameResult)
        {
            Context = context;
            Origin = origin;
            BlameResult = blameResult;
        }

        public IAnkhServiceProvider Context { get; }
        public SvnOrigin Origin { get; }
        public Collection<SvnBlameEventArgs> BlameResult { get; }
    }
}
