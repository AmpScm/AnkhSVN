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
using System.IO;
using Ankh;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Scc.Engine;
using Ankh.Scc.StatusCache;
using AnkhSvn_UnitTestProject.Helpers;
using Moq;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.StatusCache
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class SvnStatusCacheIntegrationTests
    {
        string _root;
        string _repositoryPath;
        string _workingCopyPath;
        string _trackedFile;
        SvnClient _client;
        SvnStatusCache _cache;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(
                Path.GetTempPath(),
                "AnkhSvn-StatusCache-" + Guid.NewGuid().ToString("N"));
            _repositoryPath = Path.Combine(_root, "repo");
            _workingCopyPath = Path.Combine(_root, "wc");

            Directory.CreateDirectory(_root);

            using (SvnRepositoryClient repositoryClient = new SvnRepositoryClient())
            {
                Assert.That(
                    repositoryClient.CreateRepository(_repositoryPath),
                    Is.True);
            }

            Uri repositoryUri = SvnTools.LocalPathToUri(_repositoryPath, true);

            _client = new SvnClient();
            Assert.That(_client.CheckOut(repositoryUri, _workingCopyPath), Is.True);

            _trackedFile = Path.Combine(_workingCopyPath, "tracked.txt");
            File.WriteAllText(_trackedFile, "initial");
            Assert.That(_client.Add(_trackedFile), Is.True);

            SvnCommitArgs commitArgs = new SvnCommitArgs();
            commitArgs.LogMessage = "Create status-cache test file";
            Assert.That(_client.Commit(_trackedFile, commitArgs), Is.True);

            AnkhServiceProvider services = new AnkhServiceProvider();
            services.AddService(
                typeof(IAnkhCommandService),
                new Mock<IAnkhCommandService>().Object);

            _cache = new SvnStatusCache(services);
        }

        [TearDown]
        public void TearDown()
        {
            if (_cache != null)
                ((IDisposable)_cache).Dispose();
            if (_client != null)
                _client.Dispose();

            if (!String.IsNullOrEmpty(_root) && Directory.Exists(_root))
            {
                NormalizeAttributes(_root);
                Directory.Delete(_root, true);
            }
        }

        [Test]
        public void RefreshPath_TracksCleanAndModifiedVersionedFile()
        {
            SvnItem item = _cache[_trackedFile];

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.IsFile, Is.True);
            Assert.That(item.IsModified, Is.False);

            File.AppendAllText(_trackedFile, "-changed");
            ((ISccStatusCache)_cache).MarkDirty(_trackedFile);

            Assert.That(item.IsModified, Is.True);
        }

        [Test]
        public void RefreshPath_HandlesDirectoryUnversionedAndMissingWorkingCopyPaths()
        {
            SvnItem directory = _cache[_workingCopyPath];

            Assert.That(directory.IsVersioned, Is.True);
            Assert.That(directory.IsDirectory, Is.True);

            string unversionedPath = Path.Combine(_workingCopyPath, "unversioned.txt");
            File.WriteAllText(unversionedPath, "not added");

            SvnItem unversioned = _cache[unversionedPath];
            Assert.That(unversioned.IsVersioned, Is.False);
            Assert.That(unversioned.Exists, Is.True);

            string missingPath = Path.Combine(_workingCopyPath, "missing.txt");
            SvnItem missing = _cache[missingPath];

            Assert.That(missing.IsVersioned, Is.False);
            Assert.That(missing.Exists, Is.False);
        }

        [Test]
        public void RefreshPath_RecognizesPathOutsideAnyWorkingCopy()
        {
            string outsidePath = Path.Combine(_root, "outside.txt");
            File.WriteAllText(outsidePath, "outside");

            SvnItem outside = _cache[outsidePath];

            Assert.That(outside.IsVersioned, Is.False);
            Assert.That(outside.IsVersionable, Is.False);
            Assert.That(outside.Exists, Is.True);
        }

        static void NormalizeAttributes(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                catch (IOException)
                {
                }
            }

            foreach (string directory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.SetAttributes(directory, FileAttributes.Normal);
                }
                catch (IOException)
                {
                }
            }

            File.SetAttributes(path, FileAttributes.Normal);
        }
    }
}
