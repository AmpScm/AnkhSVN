using System;
using System.Collections.Generic;
using Ankh;
using Ankh.Scc;
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class SccDocumentDataLifecycleTests
    {
        const _VSRDTFLAGS PendingInitialization = (_VSRDTFLAGS)0x00040000;
        const _VSRDTFLAGS DontPollForState = (_VSRDTFLAGS)0x00020000;

        [Test]
        public void VirtualDocumentCoversUninitializedAndDisposedFallbackPaths()
        {
            using (var services = new AnkhServiceContainer())
            {
                var data = new SccDocumentData(services, "virtual:document");

                Assert.Multiple(() =>
                {
                    Assert.That(data.Name, Is.EqualTo("virtual:document"));
                    Assert.That(data.FullPath, Is.Null);
                    Assert.That(data.RawDocument, Is.Null);
                    Assert.That(data.IsDocumentInitialized, Is.True);
                    Assert.That(data.NeedsNoDirtyCheck, Is.False);
                    Assert.That(data.IsReloadable(), Is.False);
                    Assert.That(data.IsReadOnly(), Is.False);
                    Assert.That(data.SetReadOnly(true), Is.False);
                    Assert.That(data.IsProjectPropertyPageHost, Is.False);
                    Assert.That(data.IgnoreFileChanges(true), Is.False);
                    Assert.That(data.IgnoreFileChanges(false), Is.False);
                });

                data.Cookie = 10;
                Assert.That(data.Cookie, Is.EqualTo(10));
                Assert.Throws<InvalidOperationException>(() => data.Cookie = 11);
                data.Cookie = 0;
                data.Cookie = 12;
                Assert.That(data.Cookie, Is.EqualTo(12));

                data.SetFlags(PendingInitialization);
                Assert.Multiple(() =>
                {
                    Assert.That(data.IsDocumentInitialized, Is.False);
                    Assert.That(data.NeedsNoDirtyCheck, Is.True);
                    Assert.That(data.Reload(false), Is.True);
                    Assert.That(data.IgnoreFileChanges(true), Is.False);
                    Assert.That(data.SaveDocument(null), Is.True);
                });

                data.Dispose();
                Assert.That(data.Reload(false), Is.False);
            }
        }

        [Test]
        public void FileDocumentHooksChangesTracksDirtyStateAndSchedulesStatus()
        {
            Mock<IProjectFileMapper> mapper;
            Mock<IVsFileChangeEx> fileChanges;
            Mock<IFileStatusMonitor> monitor;

            using (AnkhServiceContainer services = CreateFileServices(
                out mapper, out fileChanges, out monitor))
            {
                var data = new SccDocumentData(services, @"C:\wc\file.txt");

                Assert.That(data.FullPath, Is.Not.Null);

                bool polled = false;
                data.OnCookieLoad(delegate(SccDocumentData d, out bool dirty)
                {
                    polled = true;
                    dirty = true;
                    return true;
                });

                Assert.Multiple(() =>
                {
                    Assert.That(polled, Is.True);
                    Assert.That(data.IsDirty, Is.True);
                });

                data.SetDirty(true);
                data.OnSaved();
                Assert.That(data.IsDirty, Is.False);
                monitor.Verify(m => m.ScheduleSvnStatus(It.IsAny<string>()), Times.Once);

                data.CheckDirty(delegate(SccDocumentData d, out bool dirty)
                {
                    dirty = true;
                    return true;
                });
                Assert.That(data.IsDirty, Is.True);

                data.CheckDirty(delegate(SccDocumentData d, out bool dirty)
                {
                    Assert.Fail("A dirty document should not poll again.");
                    dirty = false;
                    return true;
                });

                data.SetDirty(false);
                Assert.That(data.IsDirty, Is.False);

                data.OnAttributeChange(__VSRDTATTRIB.RDTA_DocDataIsDirty, AlwaysClean);
                Assert.That(data.IsDirty, Is.True);
                data.OnAttributeChange(__VSRDTATTRIB.RDTA_DocDataIsNotDirty, AlwaysClean);
                Assert.That(data.IsDirty, Is.False);

                Assert.That(data.DirectoryChanged(@"C:\wc"), Is.EqualTo(VSConstants.S_OK));
                Assert.That(data.FilesChanged(0, null, null), Is.EqualTo(VSConstants.S_OK));
                Assert.That(data.FilesChanged(1,
                    new[] { @"C:\wc\file.txt" },
                    new[] { (uint)_VSFILECHANGEFLAGS.VSFILECHG_Time }), Is.EqualTo(VSConstants.S_OK));

                data.OnClosed();
                fileChanges.Verify(f => f.UnadviseFileChange(It.IsAny<uint>()), Times.AtLeastOnce);
            }
        }

        [Test]
        public void VirtualizingAFileDocumentUnhooksAndClearsItsFullPath()
        {
            Mock<IProjectFileMapper> mapper;
            Mock<IVsFileChangeEx> fileChanges;
            Mock<IFileStatusMonitor> monitor;

            using (AnkhServiceContainer services = CreateFileServices(
                out mapper, out fileChanges, out monitor))
            {
                var data = new SccDocumentData(services, @"C:\wc\file.txt");
                Assert.That(data.FullPath, Is.Not.Null);

                data.SetFlags(_VSRDTFLAGS.RDT_VirtualDocument);

                Assert.That(data.FullPath, Is.Null);
                fileChanges.Verify(f => f.UnadviseFileChange(It.IsAny<uint>()), Times.Once);
                data.Dispose();
            }
        }

        [Test]
        public void RawDocumentSupportsReloadReadOnlyAndNestedIgnoreCounts()
        {
            using (var services = new AnkhServiceContainer())
            {
                var data = new SccDocumentData(services, "virtual:document");

                var persist = new Mock<IVsPersistDocData2>();
                var changeControl = persist.As<IVsDocDataFileChangeControl>();

                int reloadable = 1;
                persist.As<IVsPersistDocData>().Setup(p => p.IsDocDataReloadable(out reloadable))
                    .Returns(VSConstants.S_OK);

                int readOnly = 1;
                persist.Setup(p => p.IsDocDataReadOnly(out readOnly))
                    .Returns(VSConstants.S_OK);
                persist.Setup(p => p.SetDocDataReadOnly(It.IsAny<int>()))
                    .Returns(VSConstants.S_OK);

                changeControl.Setup(p => p.IgnoreFileChanges(It.IsAny<int>()))
                    .Returns(VSConstants.S_OK);

                data.RawDocument = persist.Object;

                Assert.Multiple(() =>
                {
                    Assert.That(data.RawDocument, Is.SameAs(persist.Object));
                    Assert.That(data.IsReloadable(), Is.True);
                    Assert.That(data.IsReadOnly(), Is.True);
                    Assert.That(data.SetReadOnly(true), Is.True);
                    Assert.That(data.SetReadOnly(false), Is.True);
                });

                Assert.That(data.IgnoreFileChanges(true), Is.True);
                Assert.That(data.IgnoreFileChanges(true), Is.True);
                Assert.That(data.IgnoreFileChanges(false), Is.True);
                Assert.That(data.IgnoreFileChanges(false), Is.True);
                Assert.That(data.IgnoreFileChanges(false), Is.False);

                persist.Verify(p => p.SetDocDataReadOnly(1), Times.Once);
                persist.Verify(p => p.SetDocDataReadOnly(0), Times.Once);
                changeControl.Verify(p => p.IgnoreFileChanges(1), Times.AtLeastOnce);
                changeControl.Verify(p => p.IgnoreFileChanges(0), Times.AtLeastOnce);
            }
        }

        [Test]
        public void ReloadRecognizesReloadNotificationAndRemoveUndoFlag()
        {
            using (var services = new AnkhServiceContainer())
            {
                var data = new SccDocumentData(services, "virtual:document");
                var persist = new Mock<IVsPersistDocData>();

                persist.Setup(p => p.ReloadDocData(It.IsAny<uint>()))
                    .Callback<uint>(flags =>
                    {
                        data.OnAttributeChange(
                            __VSRDTATTRIB.RDTA_DocDataReloaded,
                            AlwaysClean);
                    })
                    .Returns(VSConstants.S_OK);

                data.RawDocument = persist.Object;

                Assert.That(data.Reload(true), Is.True);
                persist.Verify(
                    p => p.ReloadDocData((uint)_VSRELOADDOCDATA.RDD_RemoveUndoStack),
                    Times.Once);
            }
        }

        [Test]
        public void SaveDocumentHandlesSuccessFailureAndDontSave()
        {
            using (var services = new AnkhServiceContainer())
            {
                var data = new SccDocumentData(services, "virtual:document");
                var rdt = new Mock<IVsRunningDocumentTable>();

                rdt.Setup(r => r.SaveDocuments(
                        It.IsAny<uint>(),
                        It.IsAny<IVsHierarchy>(),
                        It.IsAny<uint>(),
                        It.IsAny<uint>()))
                    .Returns(VSConstants.S_OK);

                data.SetDirty(true);
                Assert.That(data.SaveDocument(rdt.Object), Is.True);
                Assert.That(data.IsDirty, Is.False);

                rdt.Setup(r => r.SaveDocuments(
                        It.IsAny<uint>(),
                        It.IsAny<IVsHierarchy>(),
                        It.IsAny<uint>(),
                        It.IsAny<uint>()))
                    .Returns(VSConstants.E_FAIL);

                Assert.That(data.SaveDocument(rdt.Object), Is.False);

                data.SetFlags(_VSRDTFLAGS.RDT_DontSave);
                Assert.That(data.SaveDocument(null), Is.True);
            }
        }

        [Test]
        public void CopyStateCopiesCachedDocumentIdentityAndRejectsNull()
        {
            using (var services = new AnkhServiceContainer())
            {
                var source = new SccDocumentData(services, "virtual:source");
                var target = new SccDocumentData(services, "virtual:target");
                var hierarchy = new Mock<IVsHierarchy>().Object;

                source.ItemId = 42;
                source.Hierarchy = hierarchy;
                source.SetDirty(true);
                source.OnCookieLoad(AlwaysDirty);

                target.CopyState(source);

                Assert.Multiple(() =>
                {
                    Assert.That(target.ItemId, Is.EqualTo(42));
                    Assert.That(target.Hierarchy, Is.SameAs(hierarchy));
                    Assert.That(target.IsDirty, Is.EqualTo(source.IsDirty));
                    Assert.Throws<ArgumentNullException>(() => target.CopyState(null));
                });
            }
        }

        [Test]
        public void DontPollFlagUsesCachedDirtyState()
        {
            Mock<IProjectFileMapper> mapper;
            Mock<IVsFileChangeEx> fileChanges;
            Mock<IFileStatusMonitor> monitor;

            using (AnkhServiceContainer services = CreateFileServices(
                out mapper, out fileChanges, out monitor))
            {
                var data = new SccDocumentData(services, @"C:\wc\file.txt");
                data.SetDirty(true);
                data.SetFlags(DontPollForState);

                bool invoked = false;
                data.OnCookieLoad(delegate(SccDocumentData d, out bool dirty)
                {
                    invoked = true;
                    dirty = false;
                    return true;
                });

                Assert.Multiple(() =>
                {
                    Assert.That(invoked, Is.False);
                    Assert.That(data.IsDirty, Is.True);
                    Assert.That(data.NeedsNoDirtyCheck, Is.True);
                });

                data.Dispose();
            }
        }

        static AnkhServiceContainer CreateFileServices(
            out Mock<IProjectFileMapper> mapper,
            out Mock<IVsFileChangeEx> fileChanges,
            out Mock<IFileStatusMonitor> monitor)
        {
            var services = new AnkhServiceContainer();

            mapper = new Mock<IProjectFileMapper>();
            mapper.Setup(m => m.GetAllDocumentFiles(It.IsAny<string>()))
                .Returns((string path) => new[] { path });
            services.AddService(typeof(IProjectFileMapper), mapper.Object);

            fileChanges = new Mock<IVsFileChangeEx>();
            fileChanges.As<SVsFileChangeEx>();
            uint cookie = 17;
            fileChanges.Setup(f => f.AdviseFileChange(
                    It.IsAny<string>(),
                    It.IsAny<uint>(),
                    It.IsAny<IVsFileChangeEvents>(),
                    out cookie))
                .Returns(VSConstants.S_OK);
            fileChanges.Setup(f => f.UnadviseFileChange(It.IsAny<uint>()))
                .Returns(VSConstants.S_OK);
            services.AddService(typeof(SVsFileChangeEx), fileChanges.Object);

            monitor = new Mock<IFileStatusMonitor>();
            services.AddService(typeof(IFileStatusMonitor), monitor.Object);

            return services;
        }

        static bool AlwaysClean(SccDocumentData data, out bool dirty)
        {
            dirty = false;
            return true;
        }

        static bool AlwaysDirty(SccDocumentData data, out bool dirty)
        {
            dirty = true;
            return true;
        }
    }
}
