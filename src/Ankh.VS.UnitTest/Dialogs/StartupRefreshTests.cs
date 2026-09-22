using System;
using System.Reflection;
using System.Threading;
using Ankh;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Services;
using Ankh.UI;
using Ankh.VS;
using Ankh.VS.Services;
using Ankh.VS.SolutionExplorer;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class StartupRefreshTests
    {
        sealed class TestContext : AnkhServiceContainer
        {
            public object Shell;
            public object ImageService;
            public object SelectionMonitor;
            public override object GetService(Type type)
            {
                if (type == typeof(SVsShell)) return Shell;
                if (type == typeof(SVsImageService)) return ImageService;
                if (type == typeof(SVsShellMonitorSelection)) return SelectionMonitor;
                return base.GetService(type);
            }
        }
        sealed class Events : AnkhServiceEvents
        {
            public Events(IAnkhServiceProvider context) : base(context) { }
        }

        [Test]
        public void StartupWaitsForShellThenRefreshesOnceAndCoalescesSolutionEvents()
        {
            using (var context = new TestContext())
            {
                var events = new Events(context);
                context.AddService(typeof(AnkhServiceEvents), events);
                context.AddService(typeof(IAnkhServiceEvents), events);
                var package = new Mock<IAnkhPackage>();
                context.AddService(typeof(IAnkhPackage), package.Object);
                var states = new Mock<IAnkhCommandStates>();
                states.SetupGet(x => x.UIShellAvailable).Returns(true);
                states.SetupGet(x => x.SccProviderActive).Returns(false);
                context.AddService(typeof(IAnkhCommandStates), states.Object);
                var shell = new Mock<IVsShell>();
                object initialized = false;
                shell.Setup(x => x.GetProperty(-9053, out initialized)).Returns(0);
                context.Shell = shell.Object;
                var selectionMonitor = new Mock<IVsMonitorSelection>();
                uint selectionCookie = 23;
                selectionMonitor
                    .Setup(x => x.AdviseSelectionEvents(It.IsAny<IVsSelectionEvents>(), out selectionCookie))
                    .Returns(0);
                context.SelectionMonitor = selectionMonitor.Object;
                var mapper = new Mock<IProjectFileMapper>();
                var files = new[] { "C:\\solution\\one.cs", "C:\\solution\\two.cs" };
                mapper.Setup(x => x.GetAllFilesOfAllProjects()).Returns(files);
                context.AddService(typeof(IProjectFileMapper), mapper.Object);
                var monitor = new Mock<IFileStatusMonitor>();
                context.AddService(typeof(IFileStatusMonitor), monitor.Object);
                var pending = new Mock<IPendingChangesManager>();
                context.AddService(typeof(IPendingChangesManager), pending.Object);
                var documents = new Mock<IAnkhOpenDocumentTracker>();
                context.AddService(typeof(IAnkhOpenDocumentTracker), documents.Object);
                int themes = 0;
                events.ThemeChanged += delegate { themes++; };
                using (var service = new StartupRefreshService(context))
                {
                    ((IAnkhServiceImplementation)service).OnInitialize();
                    selectionMonitor.Verify(
                        x => x.AdviseSelectionEvents(service, out selectionCookie),
                        Times.Once);
                    var idle = new AnkhIdleArgs(context, 0);
                    service.OnIdle(idle);
                    Assert.That(themes, Is.Zero);
                    ((IAnkhServiceEvents)events).OnRuntimeStarted(EventArgs.Empty);
                    service.OnIdle(idle);
                    Assert.That(themes, Is.Zero);
                    initialized = true;
                    shell.Setup(x => x.GetProperty(-9053, out initialized)).Returns(0);
                    service.OnIdle(idle);
                    Assert.That(themes, Is.EqualTo(1));
                    pending.Verify(x => x.FullRefresh(true), Times.Never);
                    states.SetupGet(x => x.SccProviderActive).Returns(true);
                    service.OnIdle(idle);
                    service.OnIdle(idle);
                    Assert.That(themes, Is.EqualTo(1));
                    monitor.Verify(x => x.ScheduleSvnStatus(files), Times.Once);
                    documents.Verify(x => x.RefreshDirtyState(), Times.Once);
                    pending.Verify(x => x.FullRefresh(true), Times.Once);
                    ((IAnkhServiceEvents)events).OnSolutionOpened(EventArgs.Empty);
                    ((IAnkhServiceEvents)events).OnSolutionOpened(EventArgs.Empty);
                    service.OnIdle(idle);
                    pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));

                    Guid solutionExplorerGuid = new Guid(ToolWindowGuids.SolutionExplorer);
                    var solutionExplorerFrame = new Mock<IVsWindowFrame>();
                    solutionExplorerFrame
                        .Setup(x => x.GetGuidProperty(
                            (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                            out solutionExplorerGuid))
                        .Returns(0);

                    // Multiple activation notifications before idle must collapse
                    // into a single full refresh.
                    service.OnElementValueChanged(
                        (uint)VSConstants.VSSELELEMID.SEID_WindowFrame,
                        null,
                        solutionExplorerFrame.Object);
                    service.OnElementValueChanged(
                        (uint)VSConstants.VSSELELEMID.SEID_WindowFrame,
                        null,
                        solutionExplorerFrame.Object);
                    service.OnIdle(idle);
                    pending.Verify(x => x.FullRefresh(true), Times.Exactly(3));
                    monitor.Verify(x => x.ScheduleSvnStatus(files), Times.Exactly(3));
                    documents.Verify(x => x.RefreshDirtyState(), Times.Exactly(3));

                    Guid otherGuid = Guid.NewGuid();
                    var otherFrame = new Mock<IVsWindowFrame>();
                    otherFrame
                        .Setup(x => x.GetGuidProperty(
                            (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                            out otherGuid))
                        .Returns(0);
                    service.OnElementValueChanged(
                        (uint)VSConstants.VSSELELEMID.SEID_WindowFrame,
                        null,
                        otherFrame.Object);
                    service.OnIdle(idle);
                    pending.Verify(x => x.FullRefresh(true), Times.Exactly(3));
                }
                selectionMonitor.Verify(
                    x => x.UnadviseSelectionEvents(selectionCookie),
                    Times.Once);
                package.Verify(x => x.UnregisterIdleProcessor(It.IsAny<IAnkhIdleProcessor>()), Times.Once);
            }
        }


        [Test]
        public void SolutionExplorerFrameDetectionRejectsNullFailuresAndOtherWindows()
        {
            Assert.That(StartupRefreshService.IsSolutionExplorerFrame(null), Is.False);

            Guid failedGuid = Guid.Empty;
            var failedFrame = new Mock<IVsWindowFrame>();
            failedFrame
                .Setup(x => x.GetGuidProperty(
                    (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                    out failedGuid))
                .Returns(-1);
            Assert.That(StartupRefreshService.IsSolutionExplorerFrame(failedFrame.Object), Is.False);

            Guid otherGuid = Guid.NewGuid();
            var otherFrame = new Mock<IVsWindowFrame>();
            otherFrame
                .Setup(x => x.GetGuidProperty(
                    (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                    out otherGuid))
                .Returns(0);
            Assert.That(StartupRefreshService.IsSolutionExplorerFrame(otherFrame.Object), Is.False);

            Guid solutionExplorerGuid = new Guid(ToolWindowGuids.SolutionExplorer);
            var solutionExplorerFrame = new Mock<IVsWindowFrame>();
            solutionExplorerFrame
                .Setup(x => x.GetGuidProperty(
                    (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                    out solutionExplorerGuid))
                .Returns(0);
            Assert.That(StartupRefreshService.IsSolutionExplorerFrame(solutionExplorerFrame.Object), Is.True);
        }

        [Test]
        public void ImageServiceLookupRetriesAfterEarlyStartupMiss()
        {
            using (var context = new TestContext())
            using (var mapper = new FileIconMapper(context))
            {
                var property = typeof(FileIconMapper).GetProperty("ImageService", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(property.GetValue(mapper), Is.Null);
                var imageService = new Mock<IVsImageService2>();
                context.ImageService = imageService.Object;
                Assert.That(property.GetValue(mapper), Is.SameAs(imageService.Object));
            }
        }
    }
}
