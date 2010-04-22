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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using EnvDTE;
using System.IO;
using Microsoft.VsSDK.IntegrationTestLibrary;


namespace IntegrationTests
{
	[TestClass]
	public class SolutionTests
	{
		#region fields
		private delegate void ThreadInvoker();
		private TestContext _testContext;
		#endregion

		#region properties
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return _testContext; }
			set { _testContext = value; }
		}
		#endregion


		#region ctors
		public SolutionTests()
		{
		}

		#endregion

        [TestInitialize]
        public void Initialize()
        {
            UIThreadInvoker.Initialize();
        }

		[TestMethod]
		[HostType("VS IDE")]
		public void CreateEmptySolution()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)delegate()
			{
				TestUtils testUtils = new TestUtils();
				testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
				testUtils.CreateEmptySolution(TestContext.TestDir, "CreateEmptySolution");
			});
		}

	}
}
