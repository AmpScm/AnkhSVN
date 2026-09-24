using System;
using System.Collections.Generic;
using System.Reflection;
using Ankh;
using Ankh.Commands;
using Ankh.Services;
using Ankh.VS.Selection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Selection
{
    [TestFixture]
    public class CommandStateBehaviorTests
    {
        static readonly string[] ContextProperties = {
            "CodeWindow", "Debugging", "DesignMode", "Dragging", "EmptySolution",
            "FullScreenMode", "NoSolution", "SolutionBuilding", "SolutionExists",
            "SolutionHasMultipleProjects", "SolutionHasSingleProject", "SccProviderActive",
            "SccManagerLoaded", "SccEnlistingInProject", "SccEnableOpenFromScc",
            "NotBuildingAndNotDebugging", "SolutionOrProjectUpgrading", "DataSourceWindowAutoVisible",
            "ToolboxInitialized", "SolutionExistsAndNotBuildingAndNotDebugging",
            "SolutionExistsAndFullyLoaded", "SolutionOpening", "ProjectRetargeting",
            "HistoricalDebugging", "SolutionHasImmersiveProject", "FirstLaunchSetup",
            "OsWindows8OrHigher", "BackgroundProjectLoad", "SolutionExplorerActive",
            "ClassViewerActive", "PendingChangesActive"
        };

        [TestCaseSource(nameof(ContextProperties))]
        public void ContextStateIsCachedAndUpdatedOnlyForItsCookie(string propertyName)
        {
            using (var services = new AnkhServiceContainer())
            {
                var monitor = new Mock<IVsMonitorSelection>(MockBehavior.Strict);
                uint cookie = 42;
                int active = 1;
                monitor.Setup(m => m.GetCmdUIContextCookie(ref It.Ref<Guid>.IsAny, out cookie)).Returns(VSConstants.S_OK);
                monitor.Setup(m => m.IsCmdUIContextActive(cookie, out active)).Returns(VSConstants.S_OK);
                services.AddService(typeof(IVsMonitorSelection), monitor.Object);
                using (var state = new CommandState(services))
                {
                    PropertyInfo property = typeof(CommandState).GetProperty(propertyName);
                    Assert.That(property.GetValue(state), Is.True);
                    Assert.That(property.GetValue(state), Is.True);
                    ChangeContext(state, 99, false);
                    Assert.That(property.GetValue(state), Is.True, "An unrelated context must not change this command state.");
                    ChangeContext(state, cookie, false);
                    Assert.That(property.GetValue(state), Is.False);
                    ChangeContext(state, cookie, true);
                    Assert.That(property.GetValue(state), Is.True);
                    monitor.Verify(m => m.GetCmdUIContextCookie(ref It.Ref<Guid>.IsAny, out cookie), Times.Once);
                    monitor.Verify(m => m.IsCmdUIContextActive(cookie, out active), Times.Once);
                }
            }
        }

        [TestCase(VSConstants.S_OK, 0, false)]
        [TestCase(VSConstants.S_OK, 1, true)]
        [TestCase(VSConstants.E_FAIL, 1, false)]
        public void FailedOrInactiveContextDoesNotEnableCommands(int result, int active, bool expected)
        {
            using (var services = new AnkhServiceContainer())
            {
                var monitor = new Mock<IVsMonitorSelection>();
                uint cookie = 5;
                monitor.Setup(m => m.GetCmdUIContextCookie(ref It.Ref<Guid>.IsAny, out cookie)).Returns(VSConstants.S_OK);
                monitor.Setup(m => m.IsCmdUIContextActive(cookie, out active)).Returns(result);
                services.AddService(typeof(IVsMonitorSelection), monitor.Object);
                using (var state = new CommandState(services))
                    Assert.That(state.SolutionExists, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ContextsSharingACookieShareCachedUpdates()
        {
            using (var services = new AnkhServiceContainer())
            {
                var monitor = new Mock<IVsMonitorSelection>();
                uint cookie = 5;
                int active = 0;
                monitor.Setup(m => m.GetCmdUIContextCookie(ref It.Ref<Guid>.IsAny, out cookie)).Returns(VSConstants.S_OK);
                monitor.Setup(m => m.IsCmdUIContextActive(cookie, out active)).Returns(VSConstants.S_OK);
                services.AddService(typeof(IVsMonitorSelection), monitor.Object);
                using (var state = new CommandState(services))
                {
                    Assert.That(state.CodeWindow, Is.False);
                    Assert.That(state.DesignMode, Is.False);
                    ChangeContext(state, cookie, true);
                    Assert.That(state.CodeWindow, Is.True);
                    Assert.That(state.DesignMode, Is.True);
                    monitor.Verify(m => m.IsCmdUIContextActive(cookie, out active), Times.Once);
                }
            }
        }

        [Test]
        public void FailedCookieLookupFallsBackToInactiveContext()
        {
            using (var services = new AnkhServiceContainer())
            {
                var monitor = new Mock<IVsMonitorSelection>();
                uint cookie = 0;
                int active = 0;
                monitor.Setup(m => m.GetCmdUIContextCookie(ref It.Ref<Guid>.IsAny, out cookie)).Returns(VSConstants.E_FAIL);
                monitor.Setup(m => m.IsCmdUIContextActive(0, out active)).Returns(VSConstants.E_FAIL);
                services.AddService(typeof(IVsMonitorSelection), monitor.Object);
                using (var state = new CommandState(services))
                    Assert.That(state.CodeWindow, Is.False);
            }
        }

        [Test]
        public void ShellActivationRequiresInitializedNonZombieShell()
        {
            using (var services = new AnkhServiceContainer())
            {
                var events = new Mock<IAnkhServiceEvents>();
                services.AddService(typeof(IAnkhServiceEvents), events.Object);
                using (var state = new CommandState(services))
                {
                    int zombie = (int)__VSSPROPID.VSSPROPID_Zombie;
                    Assert.That(state.UIShellAvailable, Is.True);
                    state.OnShellPropertyChange(zombie, true);
                    Assert.That(state.UIShellAvailable, Is.False);
                    state.OnShellPropertyChange(zombie, "invalid");
                    state.OnShellPropertyChange(-9053, true);
                    Assert.That(state.UIShellAvailable, Is.False);
                    events.Verify(e => e.OnUIShellActivate(It.IsAny<EventArgs>()), Times.Never);
                    state.OnShellPropertyChange(zombie, false);
                    state.OnShellPropertyChange(-9053, "invalid");
                    state.OnShellPropertyChange(12345, true);
                    events.Verify(e => e.OnUIShellActivate(It.IsAny<EventArgs>()), Times.Never);
                    Assert.That(state.OnShellPropertyChange(-9053, true), Is.EqualTo(VSConstants.S_OK));
                    Assert.That(state.UIShellAvailable, Is.True);
                    events.Verify(e => e.OnUIShellActivate(It.IsAny<EventArgs>()), Times.Once);
                }
            }
        }

        [Test]
        public void ShellActivationWithoutEventServiceIsHarmless()
        {
            using (var services = new AnkhServiceContainer())
            using (var state = new CommandState(services))
                Assert.That(state.OnShellPropertyChange(-9053, true), Is.EqualTo(VSConstants.S_OK));
        }

        [TestCase(VSConstants.E_FAIL, 1)]
        [TestCase(VSConstants.S_OK, 0)]
        public void UnavailableSccManagerCannotClaimOtherProviderIsActive(int result, int installed)
        {
            using (var services = new AnkhServiceContainer())
            {
                var manager = new Mock<IVsSccManager2>();
                manager.As<SVsSccManager>();
                manager.Setup(m => m.IsInstalled(out installed)).Returns(result);
                services.AddService(typeof(SVsSccManager), manager.Object);
                using (var state = new CommandState(services))
                    Assert.That(state.GetRawOtherSccProviderActive(), Is.False);
            }
        }

        [Test]
        public void MissingSccManagerCannotClaimOtherProviderIsActive()
        {
            using (var services = new AnkhServiceContainer())
            using (var state = new CommandState(services))
                Assert.That(state.GetRawOtherSccProviderActive(), Is.False);
        }

        static void ChangeContext(CommandState state, uint cookie, bool active)
        {
            // Deliver the event normally raised by the VS selection service.
            typeof(CommandState).GetMethod("OnCmdUIContextChanged", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(state, new object[] { null, new CmdUIContextChangeEventArgs(cookie, active) });
        }
    }
}
