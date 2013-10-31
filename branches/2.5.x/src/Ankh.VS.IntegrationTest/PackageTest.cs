// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Ankh;

namespace IntegrationTestProject
{
	/// <summary>
	/// Integration test for package validation
	/// </summary>
	[TestClass]
	public class PackageTest
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

		[TestMethod]
		[HostType("VS IDE")]
		public void PackageLoadTest()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)delegate()
			{

				//Get the Shell Service
				IVsShell shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsShell)) as IVsShell;
				Assert.IsNotNull(shellService);

				//Validate package load
				IVsPackage package;
                Guid packageGuid = AnkhId.PackageGuid;
				Assert.IsTrue(0 == shellService.LoadPackage(ref packageGuid, out package));
				Assert.IsNotNull(package, "Package failed to load");

			});
		}
	}
}
