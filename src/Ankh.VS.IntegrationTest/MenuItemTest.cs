using System;
using System.Globalization;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using AnkhSvn.Ids;

namespace AnkhSvn_IntegrationTestProject
{
	[TestClass()]
	public class MenuItemTest
	{
		private delegate void ThreadInvoker();

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

        [TestInitialize]
        public void Initialize()
        {
            UIThreadInvoker.Initialize();
        }

		/// <summary>
		///A test for lauching the command and closing the associated dialogbox
		///</summary>
		[TestMethod, Ignore] // show Dialog
		[HostType("VS IDE")]
		public void LaunchCommand()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)delegate()
			{
                CommandID menuItemCmd = new CommandID(AnkhId.CommandSetGuid, (int)AnkhSvn.Ids.AnkhCommand.Checkout);

				// Create the DialogBoxListener Thread.
				string expectedDialogBoxText = string.Format(CultureInfo.CurrentCulture, "{0}\n\nInside {1}.MenuItemCallback()", "AnkhSvn", "AnkhSvn.AnkhSvn.AnkhSvnPackage");
				DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDCANCEL, expectedDialogBoxText);
                
				try
				{
					purger.Start();

					TestUtils testUtils = new TestUtils();
					testUtils.ExecuteCommand(menuItemCmd);
				}
				finally
				{
					Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The dialog box has not shown");
				}
			});
		}

	}
}
