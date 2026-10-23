using System;
using System.Reflection;
using System.Threading;
using Ankh;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Services;
using Ankh.UI;
using Ankh.Selection;
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

        sealed class Harness : IDisposable
        {
            public readonly TestContext Context = new TestContext();
            public readonly Mock<IAnkhCommandStates> States = new Mock<IAnkhCommandStates>();
            public readonly Mock<IVsShell> Shell = new Mock<IVsShell>();
            public readonly Mock<IVsMonitorSelection> Selection = new Mock<IVsMonitorSelection>(MockBehavior.Strict);
            public readonly Mock<IAnkhPackage> Package = new Mock<IAnkhPackage>();
            public readonly Mock<IPendingChangesManager> Pending = new Mock<IPendingChangesManager>();
            public readonly Mock<IFileStatusMonitor> Monitor = new Mock<IFileStatusMonitor>();
            public readonly Mock<IAnkhOpenDocumentTracker> Documents = new Mock<IAnkhOpenDocumentTracker>();
            public readonly Events Events;
            public readonly StartupRefreshService Service;
            public readonly string[] Files = { @"C:\solution\one.cs", @"D:\other-wc\two.cs" };
            public long Now;
            public int Themes;

            public Harness()
            {
                Events = new Events(Context);
                Context.AddService(typeof(AnkhServiceEvents), Events);
                Context.AddService(typeof(IAnkhServiceEvents), Events);
                Context.AddService(typeof(IAnkhPackage), Package.Object);
                States.SetupGet(x => x.UIShellAvailable).Returns(true);
                States.SetupGet(x => x.SccProviderActive).Returns(true);
                States.SetupGet(x => x.SolutionExists).Returns(true);
                Context.AddService(typeof(IAnkhCommandStates), States.Object);
                Context.Shell = Shell.Object;
                Context.SelectionMonitor = Selection.Object;
                uint cookie = 23;
                Shell.Setup(x => x.AdviseBroadcastMessages(It.IsAny<IVsBroadcastMessageEvents>(), out cookie)).Returns(0);
                SetInitialized(true);
                var mapper = new Mock<IProjectFileMapper>();
                mapper.Setup(x => x.GetAllFilesOfAllProjects()).Returns(Files);
                mapper.Setup(x => x.GetAllSccProjects()).Returns(new SccProject[0]);
                Context.AddService(typeof(IProjectFileMapper), mapper.Object);
                Context.AddService(typeof(IFileStatusMonitor), Monitor.Object);
                Context.AddService(typeof(IPendingChangesManager), Pending.Object);
                Context.AddService(typeof(IAnkhOpenDocumentTracker), Documents.Object);
                Events.ThemeChanged += delegate { Themes++; };
                Service = new StartupRefreshService(Context, () => Now);
                ((IAnkhServiceImplementation)Service).OnInitialize();
            }

            public void SetInitialized(bool value)
            {
                object initialized = value;
                Shell.Setup(x => x.GetProperty(-9053, out initialized)).Returns(0);
            }
            public void Start() { ((IAnkhServiceEvents)Events).OnRuntimeStarted(EventArgs.Empty); Idle(); }
            public void Idle() { Service.OnIdle(new AnkhIdleArgs(Context, 0)); }
            public void Activate(bool active) { Service.OnBroadcastMessage(0x001C, active ? new IntPtr(1) : IntPtr.Zero, IntPtr.Zero); }
            public void Dispose() { ((IDisposable)Service).Dispose(); Context.Dispose(); }
        }

        [Test]
        public void StartupWaitsForShellAndProviderAndCoalescesLifecycleEvents()
        {
            using (var h = new Harness())
            {
                h.SetInitialized(false);
                h.States.SetupGet(x => x.SccProviderActive).Returns(false);
                h.Idle();
                h.Start();
                Assert.That(h.Themes, Is.Zero);
                h.SetInitialized(true);
                h.Idle();
                Assert.That(h.Themes, Is.EqualTo(1));
                h.Pending.Verify(x => x.FullRefresh(true), Times.Never);
                h.States.SetupGet(x => x.SccProviderActive).Returns(true);
                h.Idle();
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                ((IAnkhServiceEvents)h.Events).OnSolutionOpened(EventArgs.Empty);
                ((IAnkhServiceEvents)h.Events).OnSccProviderActivated(EventArgs.Empty);
                h.Idle();
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
                h.Monitor.Verify(x => x.ScheduleSvnStatus(h.Files), Times.Exactly(2));
                h.Documents.Verify(x => x.RefreshDirtyState(), Times.Exactly(2));
                Assert.That(h.Themes, Is.EqualTo(1));
            }
        }

        [Test]
        public void NavigationAndDuplicateActivationNeverScheduleFullScans()
        {
            using (var h = new Harness())
            {
                h.Start();
                Assert.That(h.Service, Is.Not.InstanceOf<IVsSelectionEvents>());
                for (int i = 0; i < 100; i++)
                {
                    h.Now += 1000;
                    h.Activate(true);
                    h.Service.OnBroadcastMessage(0x0006, new IntPtr(1), IntPtr.Zero); // WM_ACTIVATE, not app activation
                    h.Idle();
                }
                h.Selection.VerifyNoOtherCalls();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                h.Monitor.Verify(x => x.ScheduleSvnStatus(h.Files), Times.Once);
            }
        }

        [Test]
        public void ReturningFromExternalToolRefreshesAllProjectsOnceAfterSettling()
        {
            using (var h = new Harness())
            {
                h.Start();
                h.Now = 10000;
                h.Activate(false);
                h.Idle();
                h.Activate(true);
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                h.Now += StartupRefreshService.ActivationDelayMilliseconds;
                h.Idle();
                h.Activate(true);
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
                h.Monitor.Verify(x => x.ScheduleSvnStatus(h.Files), Times.Exactly(2));
                h.Monitor.Verify(x => x.ScheduleGlyphOnlyUpdate(It.IsAny<System.Collections.Generic.IEnumerable<SccProject>>()), Times.Exactly(2));
                h.Documents.Verify(x => x.RefreshDirtyState(), Times.Exactly(2));
            }
        }

        [Test]
        public void RapidSwitchingDefersButNeverDropsFinalRefresh()
        {
            using (var h = new Harness())
            {
                h.Start();
                h.Now = 100;
                h.Activate(false);
                h.Activate(true);
                h.Now = 700;
                h.Idle(); // within minimum interval
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                h.Activate(false);
                h.Now = 3000;
                h.Idle(); // must not catch up while inactive
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                h.Activate(true);
                h.Now += StartupRefreshService.ActivationDelayMilliseconds;
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
                h.Now += 100;
                h.Activate(false);
                h.Activate(true);
                h.Now += StartupRefreshService.ActivationDelayMilliseconds;
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
                h.Now += StartupRefreshService.RefreshIntervalMilliseconds;
                h.Idle(); // no new event needed to flush the deferred request
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(3));
            }
        }

        [Test]
        public void UnavailableProviderRetainsCatchupAndSolutionCloseCancelsIt()
        {
            using (var h = new Harness())
            {
                h.Start();
                h.Activate(false);
                h.Activate(true);
                h.Now = 10000;
                h.States.SetupGet(x => x.SccProviderActive).Returns(false);
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
                h.States.SetupGet(x => x.SccProviderActive).Returns(true);
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
                h.Activate(false);
                h.Activate(true);
                ((IAnkhServiceEvents)h.Events).OnSolutionClosed(EventArgs.Empty);
                h.Now += 10000;
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
            }
        }

        [Test]
        public void LifecycleRefreshSubsumesPendingApplicationCatchup()
        {
            using (var h = new Harness())
            {
                h.Start();
                h.Activate(false);
                h.Activate(true);
                ((IAnkhServiceEvents)h.Events).OnSolutionOpened(EventArgs.Empty);
                h.Idle();
                h.Now = 10000;
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Exactly(2));
            }
        }

        [Test]
        public void ApplicationActivationWithoutSolutionDoesNotScan()
        {
            using (var h = new Harness())
            {
                h.Start();
                h.States.SetupGet(x => x.SolutionExists).Returns(false);
                h.Activate(false);
                h.Activate(true);
                h.Now = 10000;
                h.Idle();
                h.Pending.Verify(x => x.FullRefresh(true), Times.Once);
            }
        }

        [Test]
        public void BroadcastSubscriptionIsReleased()
        {
            var h = new Harness();
            h.Start();
            h.Dispose();
            h.Shell.Verify(x => x.UnadviseBroadcastMessages(23), Times.Once);
            h.Package.Verify(x => x.UnregisterIdleProcessor(h.Service), Times.Once);
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
