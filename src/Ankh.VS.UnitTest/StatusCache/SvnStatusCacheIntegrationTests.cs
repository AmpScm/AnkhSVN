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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
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
        Mock<IFileStatusMonitor> _monitor;

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

            _monitor = new Mock<IFileStatusMonitor>();
            services.AddService(typeof(IFileStatusMonitor), _monitor.Object);
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

        [Test]
        public void ExternalPropertyChangeQueuesGlyphUpdateWithoutClearingCache()
        {
            SvnItem item = _cache[_trackedFile];
            Assert.That(item.IsModified, Is.False);
            _client.SetProperty(_trackedFile, "test:property", "changed");
            ShellChange(0x2000, _trackedFile, null); // SHCNE_UPDATEITEM
            Assert.That(_cache[_trackedFile], Is.SameAs(item));
            Assert.That(item.IsModified, Is.True);
            _monitor.Verify(m => m.ScheduleGlyphUpdate(It.Is<IEnumerable<string>>(
                paths => paths.Count() == 1 && paths.Contains(_trackedFile))), Times.Once);

            _client.Revert(_trackedFile);
            ShellChange(0x2000, _trackedFile, null);
            Assert.That(item.IsModified, Is.False);
        }

        [Test]
        public void DirectoryNotificationInvalidatesCachedDescendantsEvenWhenParentIsNotCached()
        {
            SvnItem item = _cache[_trackedFile];
            Assert.That(item.IsModified, Is.False);
            File.AppendAllText(_trackedFile, "external edit");
            ShellChange(0x1000, _workingCopyPath, null); // SHCNE_UPDATEDIR
            Assert.That(item.IsModified, Is.True);
            _monitor.Verify(m => m.ScheduleGlyphUpdate(It.Is<IEnumerable<string>>(
                paths => paths.Contains(_trackedFile))), Times.Once);
        }

        [Test]
        public void RenameRefreshesBothPathsAndUnrelatedNotificationsDoNothing()
        {
            string renamed = Path.Combine(_workingCopyPath, "renamed.txt");
            SvnItem oldItem = _cache[_trackedFile];
            SvnItem newItem = _cache[renamed];
            Assert.That(oldItem.Exists, Is.True);
            Assert.That(newItem.Exists, Is.False);
            File.Move(_trackedFile, renamed);
            ShellChange(0x0001, _trackedFile, renamed);
            Assert.That(oldItem.Exists, Is.False);
            Assert.That(newItem.Exists, Is.True);
            _monitor.Verify(m => m.ScheduleGlyphUpdate(It.Is<IEnumerable<string>>(
                paths => paths.Contains(_trackedFile) && paths.Contains(renamed))), Times.Once);
            _monitor.Invocations.Clear();
            ShellChange(0x1000, Path.Combine(_root, "unrelated"), null);
            ShellChange(0x2000, null, null);
            _monitor.VerifyNoOtherCalls();
        }

        void ShellChange(int flags, string first, string second)
        {
            typeof(SvnStatusCache).GetMethod("OnShellChange",
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(int), typeof(int), typeof(string), typeof(string) }, null)
                .Invoke(_cache, new object[] { flags, 0, first, second });
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
